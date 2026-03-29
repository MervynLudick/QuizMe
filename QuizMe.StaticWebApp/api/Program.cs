using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var storageConnection = Environment.GetEnvironmentVariable("QUIZ_STORAGE_CONNECTION") ?? string.Empty;

builder.Services
    .AddSingleton(new api.GameStorageService(storageConnection))
    .AddSingleton(new api.QuestionService(storageConnection))
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
