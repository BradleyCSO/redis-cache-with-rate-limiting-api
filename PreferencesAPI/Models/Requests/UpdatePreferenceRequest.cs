namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Requests;

public class UpdatePreferenceRequest
{
    public int Id { get; set; }
    public string? Region { get; set; }

    public string? PublicName { get; set; }

    public string? PrivateName { get; set; }

    public bool? IsPreference { get; set; }

    public string? Ordering { get; set; }

    public bool? IsPublic { get; set; }

    public int? CategoryId { get; set; }
}