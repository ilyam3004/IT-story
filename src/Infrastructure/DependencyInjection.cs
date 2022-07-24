using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces.Authentication;
using Infrastructure.Authentication;
using Application.Common.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Infrastructure.Presistence;
using Application.Common.Interfaces.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}