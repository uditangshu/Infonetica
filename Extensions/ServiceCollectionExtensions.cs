using ConfigurableWorkflowEngine.Core.Interfaces;
using ConfigurableWorkflowEngine.Infrastructure.Repositories;
using ConfigurableWorkflowEngine.Services;
using ConfigurableWorkflowEngine.Features.WorkflowDefinitions;
using ConfigurableWorkflowEngine.Features.WorkflowInstances;
using ConfigurableWorkflowEngine.Features.WorkflowActions;

namespace ConfigurableWorkflowEngine.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IWorkflowRepository, InMemoryWorkflowRepository>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IWorkflowValidationService, WorkflowValidationService>();

        // Definition handlers
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowDefinitions.Create.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowDefinitions.GetAll.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowDefinitions.GetById.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowDefinitions.UpdateStates.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowDefinitions.UpdateActions.Handler>();
        
        // Instance handlers
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowInstances.Create.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowInstances.GetAll.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowInstances.GetById.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowInstances.GetByDefinition.Handler>();
        
        // Action handlers
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowActions.Execute.Handler>();
        services.AddScoped<ConfigurableWorkflowEngine.Features.WorkflowActions.GetAvailable.Handler>();

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