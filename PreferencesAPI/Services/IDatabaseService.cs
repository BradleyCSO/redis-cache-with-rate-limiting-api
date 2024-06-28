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