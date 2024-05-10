using Npgsql;
using PreferencesApi.Middleware;
using PreferencesApi.Services;

namespace PreferencesApi;

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

        app.UseMiddleware<ReadFromCacheMiddleware>();
        app.UseMiddleware<RateLimitingMiddleware>();
        app.UseMiddleware<WriteToCacheMiddleware>();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}