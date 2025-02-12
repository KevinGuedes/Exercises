using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Questao5.Application.RequestPipeline;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Database.Repositories;
using Questao5.Infrastructure.Sqlite;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

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
    .AddScoped<ISqlConnectionManager, SqlConnectionManager>()
    .AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>()
    .AddScoped<IIdempotencyRepository, IdempotencyRepository>()
    .AddScoped<IAccountRepository, AccountRepository>()
    .AddScoped<ITransferRepository, TransferRepository>();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Services.GetService<IDatabaseBootstrap>()!.Setup();

app.Run();