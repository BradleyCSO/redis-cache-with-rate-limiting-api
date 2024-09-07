using Npgsql;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Middleware;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Services;

namespace RedisCacheWithRateLimitingWebAPI.MainAPI;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IDatabaseService, DatabaseService>(provider =>
        {
            string? connectionString = configuration.GetConnectionString("Postgres");
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            return new DatabaseService(connection, provider.GetRequiredService<ILogger<DatabaseService>>());
        });

        services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        IServiceScope scope = app.ApplicationServices.CreateScope();
        IDatabaseService databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        databaseService.CreatePreferencesTableIfNotExists();
        scope.Dispose();

        app.UseWhen(httpContext => httpContext.Request.Method == "GET",
            builder => builder.UseMiddleware<ReadFromCacheMiddleware>());
        app.UseMiddleware<RateLimitingMiddleware>();
        app.UseWhen(httpContext => httpContext.Request.Method == "GET",
            builder => builder.UseMiddleware<ResponseCachingMiddleware>());

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}