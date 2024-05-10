namespace PreferencesApi.Models.Requests;

/// <summary>
/// https://developer.dotdigital.com/reference/delete-preference
/// </summary>
public class DeletePreferenceRequest
{
    public required string Region { get; set; }
    public required int Id { get; set; }
}