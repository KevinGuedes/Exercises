using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Questao5.Application.RequestPipeline;
using Questao5.Application.Services;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Domain.Interfaces.Services;
using Questao5.Infrastructure.Database.Repositories;
using Questao5.Infrastructure.Sqlite;
using Questao5.Presentation.Middlewares;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandlerMiddleware>();

builder.Services
    .AddMediatR(cfg =>
        cfg
            .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            .AddOpenBehavior(typeof(ValidationBehavior<,>))
    )
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

SqlMapper.AddTypeHandler(new GuidTypeHandler());
builder.Services
    .AddSingleton(new DatabaseConfig { ConnectionString = builder.Configuration.GetConnectionString("Database")! })
    .AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>()
    .AddScoped<ISqlConnectionManager, SqlConnectionManager>()
    .AddScoped<IIdempotencyRepository, IdempotencyRepository>()
    .AddScoped<IAccountRepository, AccountRepository>()
    .AddScoped<ITransferRepository, TransferRepository>()
    .AddSingleton<ISerializerService, SerializerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Financial API";
        options.ConfigObject.TryItOutEnabled = true;
        options.ConfigObject.DisplayRequestDuration = true;
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Services.GetService<IDatabaseBootstrap>()!.Setup();

app.Run();

public partial class Program { }