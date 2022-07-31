using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Authentication;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
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
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IFollowingService, FollowingService>();

        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IFollowingRepository, FollowingRepository>();

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}