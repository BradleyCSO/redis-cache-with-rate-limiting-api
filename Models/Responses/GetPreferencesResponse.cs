namespace PreferencesApi.Models.Responses;

public class GetPreferencesResponse
{
    public int Id { get; set; }
    public string? Region { get; set; }
    public string? PublicName { get; set; }
    public string? PrivateName { get; set; }
    public bool? IsPreference { get; set; }
    public string? Order { get; set; }
    public bool? IsPublic { get; set; }
    public int? CategoryId { get; set; }
}