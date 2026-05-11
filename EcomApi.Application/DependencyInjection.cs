using System;
using EcomApi.Application.Services.Implementations;
using EcomApi.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EcomApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}
