using ConfigurableWorkflowEngine.Core.Interfaces;
using ConfigurableWorkflowEngine.Infrastructure.Repositories;
using ConfigurableWorkflowEngine.Services;
using System.Reflection;

namespace ConfigurableWorkflowEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<IWorkflowRepository, InMemoryWorkflowRepository>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IWorkflowValidationService, WorkflowValidationService>();

        // Register all handlers from the Features assembly
        services.Scan(scan => scan
            .FromAssembly(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.InNamespaces("ConfigurableWorkflowEngine.Features"))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return services;
    }
    
    public static IServiceCollection AddWorkflowConfiguration(this IServiceCollection services)
    {
        // Configure JSON serialization
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null; // Use exact property names
            options.SerializerOptions.WriteIndented = true;
        });
        
        return services;
    }
} 