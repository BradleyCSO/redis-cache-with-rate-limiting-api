using PreferencesApi.Models.Requests;
using PreferencesApi.Models.Responses;

namespace PreferencesApi.Services;

public interface IDatabaseService
{
    void CreatePreferencesTableIfNotExists();
    Task<int?> CreatePreferenceAsync(CreatePreferenceRequest createPreferenceRequest);
    Task<int?> UpdatePreferenceAsync(UpdatePreferenceRequest updatePreferenceRequest);
    Task<List<GetPreferencesResponse>> GetPreferencesAsync(GetPreferencesRequest getPreferencesRequest);
    Task<bool> DeletePreferenceAsync(DeletePreferenceRequest deletePreferenceRequest);
}