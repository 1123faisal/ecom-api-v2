using System.Text;
using EcomApi.API.Middleware;
using EcomApi.Application;
using EcomApi.Infrastructure;
using EcomApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
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
});

// Application + infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Auth
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
        };
    });
builder.Services.AddAuthorization();

// Cors for Development
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// app pipeline
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// global error handler
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// migration and data seed.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 5;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            System.Console.WriteLine("Database Ready.");
            break;
        }
        catch (System.Exception ex)
        {
            retries--;
            System.Console.WriteLine(
                $"Database not ready - retrying ({retries} left): {ex.Message}"
            );
            Thread.Sleep(3000);
        }
    }
}

app.Run();
