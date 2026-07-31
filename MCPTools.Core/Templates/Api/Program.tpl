using System.Data;
using Microsoft.Data.SqlClient;
using {{Namespace}}.Api.Middleware;
using {{Namespace}}.Application.Managers;
using {{Namespace}}.Application.Services;
using {{Namespace}}.Domain.Repositories;
using {{Namespace}}.Infrastructure.Data;
using {{Namespace}}.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("{{Database}}")
        ?? throw new InvalidOperationException("Connection string '{{Database}}' was not found.");

    return new SqlConnection(connectionString);
});

builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<I{{RepositoryName}}, {{RepositoryName}}>();
builder.Services.AddScoped<I{{ServiceName}}, {{ServiceName}}>();
builder.Services.AddScoped<I{{ManagerName}}, {{ManagerName}}>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
