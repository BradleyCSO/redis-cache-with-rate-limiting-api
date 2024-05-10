using Npgsql;
using PreferencesApi.Models.Requests;
using PreferencesApi.Models.Responses;

namespace PreferencesApi.Services;

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
            query.Parameters.AddWithValue("isPreference", createPreferenceRequest.IsPreference);
            query.Parameters.AddWithValue("ordering", createPreferenceRequest.Ordering);
            query.Parameters.AddWithValue("isPublic", createPreferenceRequest.IsPublic);
            query.Parameters.AddWithValue("categoryId", createPreferenceRequest.CategoryId);

            object? createdUserId = await query.ExecuteScalarAsync();

            await connection.CloseAsync();

            return createdUserId != null ? Convert.ToInt32(createdUserId) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
        }
        finally
        {
            await connection.CloseAsync();
        }

        return null;
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


            if (updatePreferenceRequest.Region != null)
                query.Parameters.AddWithValue("@region", updatePreferenceRequest.Region);

            if (updatePreferenceRequest.PublicName != null)
                query.Parameters.AddWithValue("@publicName", updatePreferenceRequest.PublicName);

            if (updatePreferenceRequest.PrivateName != null)
                query.Parameters.AddWithValue("@privateName", updatePreferenceRequest.PrivateName);

            query.Parameters.AddWithValue("@isPreference", updatePreferenceRequest.IsPreference ?? false);
            query.Parameters.AddWithValue("@ordering", updatePreferenceRequest.Ordering ?? "0");
            query.Parameters.AddWithValue("@isPublic", updatePreferenceRequest.IsPublic ?? false);
            query.Parameters.AddWithValue("@categoryId", updatePreferenceRequest.CategoryId ?? 0);
            query.Parameters.AddWithValue("@id", updatePreferenceRequest.Id);

            object? updatedPreferenceId = await query.ExecuteScalarAsync();

            await connection.CloseAsync();

            return updatedPreferenceId != null ? Convert.ToInt32(updatedPreferenceId) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating preference");
        }
        finally
        {
            await connection.CloseAsync();
        }

        return null;
    }

    public async Task<List<GetPreferencesResponse>> GetPreferencesAsync(GetPreferencesRequest getPreferencesRequest)
    {
        await connection.OpenAsync();

        NpgsqlCommand query = new NpgsqlCommand("SELECT * FROM preferences WHERE region = @region", connection);
        query.Parameters.AddWithValue("region", getPreferencesRequest.Region);

        NpgsqlDataReader reader = await query.ExecuteReaderAsync();
        List<GetPreferencesResponse> listOfPreferences = new List<GetPreferencesResponse>();

        while (reader.Read())
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

        await connection.CloseAsync();

        return listOfPreferences;
    }

    public async Task<bool> DeletePreferenceAsync(DeletePreferenceRequest deletePreferenceRequest)
    {
        await connection.OpenAsync();

        NpgsqlCommand query = new NpgsqlCommand("DELETE FROM preferences WHERE region = @region AND id = @id", connection);
        query.Parameters.AddWithValue("region", deletePreferenceRequest.Region);
        query.Parameters.AddWithValue("id", deletePreferenceRequest.Id);

        int affectedRows = await query.ExecuteNonQueryAsync();

        await connection.CloseAsync();

        // Check if any rows were affected
        bool isDeleted = affectedRows > 0;

        return isDeleted;
    }
}