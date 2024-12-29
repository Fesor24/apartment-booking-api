using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Authentication.Models;
using Bookify.Infrastructure.Authorization;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();

        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        AddAuthorization(services);

        return services;
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(); // will use the custom configuration provided using the IConfigurationNamedOptions<JwtBearerOptions>

        services.Configure<Infrastructure.Authentication.AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.Configure<KeyCloakOptions>(configuration.GetSection("KeyCloak"));

        services.AddHttpClient<Application.Abstractions.Authentication.IAuthenticationService, Infrastructure.Authentication.AuthenticationService>((sp, client) =>
        {
            var keyCloakOptions = sp.GetRequiredService<IOptions<KeyCloakOptions>>().Value;

            client.BaseAddress = new Uri(keyCloakOptions.AdminUrl);
        })
           .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddScoped<IJwtService, JwtService>();
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                        throw new ArgumentNullException();

        services.AddDbContext<ApplicationDbContext>(opts =>
        {
            opts.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();

        // Tells the DI container to resolve an instance of ApplicationDbContext
        // The instance is then used when wherever IUniOfWork is injected
        // For this to work, ApplicationDbContext must implement the IUnitOfWork interface
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
    }
}
