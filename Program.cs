using ConfigurableWorkflowEngine.Endpoints;
using ConfigurableWorkflowEngine.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

//added for swagger openAPI specs for my testing
builder.Services.AddSwaggerGen();

builder.Services.AddWorkflowServices();
builder.Services.AddWorkflowConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Configurable Workflow Engine API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// Map all endpoints
app.MapWorkflowDefinitionEndpoints();
app.MapWorkflowInstanceEndpoints();
app.MapWorkflowActionEndpoints();
app.MapHealthEndpoints();

app.Run(); 