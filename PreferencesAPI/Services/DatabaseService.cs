using Npgsql;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Exceptions;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Requests;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Responses;

namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Services;

public interface IDatabaseService
{
    void CreatePreferencesTableIfNotExists();
    Task<int?> CreatePreferenceAsync(CreatePreferenceRequest createPreferenceRequest);
    Task<int?> UpdatePreferenceAsync(UpdatePreferenceRequest updatePreferenceRequest);
    Task<List<GetPreferencesResponse>> GetPreferencesAsync(GetPreferencesRequest getPreferencesRequest);
    Task<bool> DeletePreferenceAsync(DeletePreferenceRequest deletePreferenceRequest);
}

public class DatabaseService(NpgsqlConnection connection, ILogger<DatabaseService> logger) : IDatabaseService
{
    public void CreatePreferencesTableIfNotExists()
    {
        try
        {
            connection.Open();

            new NpgsqlCommand(
                "CREATE TABLE IF NOT EXISTS preferences (" +
                "id SERIAL PRIMARY KEY," +
                "region TEXT NOT NULL," +
                "publicName TEXT NOT NULL UNIQUE," +
                "privateName TEXT NOT NULL UNIQUE," +
                "isPreference TEXT NOT NULL, " +
                "ordering TEXT," +
                "isPublic BOOLEAN," +
                "categoryId INTEGER" +
                ")", connection).ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when trying to create preferences table");
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task<int?> CreatePreferenceAsync(CreatePreferenceRequest createPreferenceRequest)
    {
        try
        {
            await connection.OpenAsync();

            NpgsqlCommand query = new NpgsqlCommand(
                "INSERT INTO preferences (region, publicName, privateName, isPreference, ordering, isPublic, categoryId) " +
                "VALUES (@region, @publicName, @privateName, @isPreference, @ordering, @isPublic, @categoryId)" +
                "RETURNING id", connection);

            // Add parameters to the query
            query.Parameters.AddWithValue("region", createPreferenceRequest.Region);
            query.Parameters.AddWithValue("publicName", createPreferenceRequest.PublicName);
            query.Parameters.AddWithValue("privateName", createPreferenceRequest.PrivateName);
            query.Parameters.AddWithValue("isPreference", createPreferenceRequest.IsPreference ?? false);
            query.Parameters.AddWithValue("ordering", createPreferenceRequest.Ordering ?? "top");
            query.Parameters.AddWithValue("isPublic", createPreferenceRequest.IsPublic ?? false);
            query.Parameters.AddWithValue("categoryId", createPreferenceRequest.CategoryId ?? 0);

            object? createdUserId = await query.ExecuteScalarAsync();

            return createdUserId != null ? Convert.ToInt32(createdUserId) : null;
        }
        catch (PostgresException alreadyExistsException) when (alreadyExistsException.SqlState == "23505")
        {
            throw new PreferencesConflictException($"A preference record already exists for {alreadyExistsException.ConstraintName}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
            throw new PreferencesBadRequestException("Bad request");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<int?> UpdatePreferenceAsync(UpdatePreferenceRequest updatePreferenceRequest)
    {
        try
        {
            await connection.OpenAsync();

            NpgsqlCommand query = new NpgsqlCommand(
                "UPDATE preferences SET " +
                "region = @region, " +
                "publicName = @publicName, " +
                "privateName = @privateName, " +
                "isPreference = @isPreference, " +
                "ordering = @ordering, " +
                "isPublic = @isPublic, " +
                "categoryId = @categoryId " +
                "WHERE id = @id " +
                "RETURNING id", connection);

            query.Parameters.AddWithValue("@region", updatePreferenceRequest.Region ?? throw new InvalidOperationException());
            query.Parameters.AddWithValue("@publicName", updatePreferenceRequest.PublicName ?? throw new InvalidOperationException());
            query.Parameters.AddWithValue("@privateName", updatePreferenceRequest.PrivateName ?? throw new InvalidOperationException());
            query.Parameters.AddWithValue("@isPreference", updatePreferenceRequest.IsPreference ?? false);
            query.Parameters.AddWithValue("@ordering", updatePreferenceRequest.Ordering ?? "0");
            query.Parameters.AddWithValue("@isPublic", updatePreferenceRequest.IsPublic ?? false);
            query.Parameters.AddWithValue("@categoryId", updatePreferenceRequest.CategoryId ?? 0);
            query.Parameters.AddWithValue("@id", updatePreferenceRequest.Id);

            object? updatedPreferenceId = await query.ExecuteScalarAsync();

            return updatedPreferenceId != null ? Convert.ToInt32(updatedPreferenceId) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
            throw new PreferencesBadRequestException("Bad request");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<List<GetPreferencesResponse>> GetPreferencesAsync(GetPreferencesRequest getPreferencesRequest)
    {
        try
        {
            await connection.OpenAsync();

            NpgsqlCommand query = new NpgsqlCommand("SELECT * FROM preferences WHERE region = @region", connection);
            query.Parameters.AddWithValue("region", getPreferencesRequest.Region);

            NpgsqlDataReader reader = await query.ExecuteReaderAsync();
            List<GetPreferencesResponse> listOfPreferences = new List<GetPreferencesResponse>();

            while (await reader.ReadAsync())
            {
                listOfPreferences.Add(new GetPreferencesResponse
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Region = reader["region"].ToString(),
                    PublicName = reader["publicName"].ToString(),
                    PrivateName = reader["privateName"].ToString(),
                    IsPreference = Convert.ToBoolean(reader["isPreference"]),
                    Order = reader["ordering"].ToString(),
                    IsPublic = Convert.ToBoolean(reader["isPublic"]),
                    CategoryId = Convert.ToInt32(reader["categoryId"]),
                });
            }

            return listOfPreferences;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
            throw new PreferencesBadRequestException("Bad request");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<bool> DeletePreferenceAsync(DeletePreferenceRequest deletePreferenceRequest)
    {
        try
        {
            await connection.OpenAsync();

            NpgsqlCommand query = new NpgsqlCommand("DELETE FROM preferences WHERE region = @region AND id = @id", connection);
            query.Parameters.AddWithValue("region", deletePreferenceRequest.Region);
            query.Parameters.AddWithValue("id", deletePreferenceRequest.Id);

            int affectedRows = await query.ExecuteNonQueryAsync();

            // Check if any rows were affected
            bool isDeleted = affectedRows > 0;

            return isDeleted;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
            throw new PreferencesBadRequestException("Bad request");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}