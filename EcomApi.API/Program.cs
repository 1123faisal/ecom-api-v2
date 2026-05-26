using System.Text;
using System.Text.Json.Serialization;
using EcomApi.API.Middleware;
using EcomApi.API.Options;
using EcomApi.Application;
using EcomApi.Infrastructure;
using EcomApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers — serialize enums as strings globally
builder
    .Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (doc, ctx, ct) =>
        {
            doc.Components ??= new OpenApiComponents();
            doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Description = "Enter Your JWT Token Below.",
            };
            return Task.CompletedTask;
        }
    );

    options.AddOperationTransformer(
        (op, ctx, ct) =>
        {
            var hasAuthorize = ctx
                .Description.ActionDescriptor.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Any();
            if (hasAuthorize)
            {
                op.Security ??= [];
                op.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer")] = [],
                    }
                );
            }
            return Task.CompletedTask;
        }
    );

    // Scalar uses empty string as a generic placeholder when no example is set.
    // This transformer injects a sensible example value for every integer schema
    // so the generated curl snippets show real numbers instead of "".
    options.AddSchemaTransformer(
        (schema, ctx, ct) =>
        {
            if (schema.Type == JsonSchemaType.Integer && schema.Example == null)
                schema.Example = System.Text.Json.Nodes.JsonValue.Create(1);
            return Task.CompletedTask;
        }
    );
});

// Typed options — validated at startup
builder
    .Services.AddOptions<AdminOptions>()
    .Bind(builder.Configuration.GetSection(AdminOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Application + infrastructure
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Auth — re-uses the same JwtOptions already bound inside AddApplication
var jwtSection = builder.Configuration.GetSection("Jwt");
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
        };
    });
builder.Services.AddAuthorization();

// Health checks — /healthz/live for liveness, /healthz/ready for readiness
builder
    .Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        tags: ["ready"]
    )
    .AddRedis(
        sp => sp.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>(),
        name: "redis",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
        tags: ["ready"],
        timeout: TimeSpan.FromSeconds(1)
    );

// CORS — AllowAll is fine in development; configure AllowedOrigins in production
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "DefaultCors",
        policy =>
        {
            if (allowedOrigins is { Length: > 0 })
                policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
            else
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// app pipeline
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz/live");
app.MapHealthChecks(
    "/healthz/ready",
    new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
    }
);

// Apply pending EF Core migrations on startup (with retry for Docker health checks)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var retries = 5;

    while (retries > 0)
    {
        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database is ready.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning(
                ex,
                "Database not ready — retrying ({Retries} attempts left).",
                retries
            );
            await Task.Delay(3000);
        }
    }
}

app.Run();
