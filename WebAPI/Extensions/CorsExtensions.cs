namespace WebAPI.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("AllowLocalhost5173",
            policyBuilder => policyBuilder
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod())
        );

        return services;
    }
}