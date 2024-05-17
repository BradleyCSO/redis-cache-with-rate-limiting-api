namespace PreferencesApi.Models.Requests;
public class DeletePreferenceRequest
{
    public required string Region { get; set; }
    public required int Id { get; set; }
}