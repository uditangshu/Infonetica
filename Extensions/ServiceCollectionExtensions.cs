using ConfigurableWorkflowEngine.Core.Interfaces;
using ConfigurableWorkflowEngine.Infrastructure.Repositories;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
    {
        // Register repository
        services.AddSingleton<IWorkflowRepository, InMemoryWorkflowRepository>();
        
        // Register services
        services.AddScoped<IWorkflowValidationService, WorkflowValidationService>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        
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