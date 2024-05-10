namespace PreferencesApi.Models.Requests;

/// <summary>
/// https://developer.dotdigital.com/reference/get-preferences
/// </summary>
public class GetPreferencesRequest
{
    public required string Region { get; set; }
}