namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Requests;

public class CreatePreferenceRequest
{
    public required string Region { get; set; }

    public required string PublicName { get; set; }

    public required string PrivateName { get; set; }

    public bool? IsPreference { get; set; }

    public string? Ordering { get; set; }

    public bool? IsPublic { get; set; }

    public int? CategoryId { get; set; }
}