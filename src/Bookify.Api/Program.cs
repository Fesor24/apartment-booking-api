using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Dapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

//builder.Services.AddHealthChecks()
//    .AddCheck<CustomSqlHealthCheck>("custom-sql");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigrations();

    //app.UseSwaggerUI(opts =>
    //{
    //    var descriptions = app.DescribeApiVersions();

    //    foreach(var description in descriptions)
    //    {
    //        var url = $"/swagger/{description.GroupName}/swagger.json";
    //        var name = description.GroupName.ToUpperInvariant();
    //        opts.SwaggerEndpoint(url, name);
    //    }
    //});
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

//public class CustomSqlHealthCheck(ISqlConnectionFactory sqlConnectionFactory) : IHealthCheck
//{
//    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            using var connection = sqlConnectionFactory.CreateConnection();

//            await connection.ExecuteScalarAsync("SELECT 1");

//            return HealthCheckResult.Healthy();
//        }
//        catch(Exception ex)
//        {
//            return HealthCheckResult.Unhealthy(exception: ex);
//        }
//    }
//}
