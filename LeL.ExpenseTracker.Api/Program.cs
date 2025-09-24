using FluentValidation;
using LeL.ExpenseTracker.Api.Extensions;
using LeL.ExpenseTracker.Api.Pipelines;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()

    .AddAutoMapper(typeof(Program).Assembly)
    .AddMediatR(c => c.RegisterServicesFromAssembly(typeof(Program).Assembly))

    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))

    .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestResponseLoggingBehavior<,>))

    .ConfigureDbContext(builder.Configuration);

var app = builder.Build();

app.Run();

public partial class Program { }