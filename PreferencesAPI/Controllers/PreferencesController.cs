using Microsoft.AspNetCore.Mvc;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Exceptions;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Requests;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Models.Responses;
using RedisCacheWithRateLimitingWebAPI.MainAPI.Services;

namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PreferencesController(IDatabaseService databaseService, ILogger<PreferencesController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePreference([FromBody] CreatePreferenceRequest createPreferenceRequest)
    {
        try
        {
            int? createPreferenceAsync = await databaseService.CreatePreferenceAsync(createPreferenceRequest);

            if (createPreferenceAsync != null)
                return new ObjectResult(new { createPreferenceAsync }) { StatusCode = 201 };
        }
        catch (PreferencesConflictException)
        {
            return new StatusCodeResult(409);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create preference");
        }

        return new StatusCodeResult(304);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceRequest updatePreferenceRequest)
    {
        try
        {
            int? updatePreferenceAsync = await databaseService.UpdatePreferenceAsync(updatePreferenceRequest);

            if (updatePreferenceAsync != null)
                return new ObjectResult(new { updatePreferenceAsync }) { StatusCode = 204 };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update preference");
        }

        return new StatusCodeResult(304);
    }

    [HttpGet]
    public async Task<IActionResult> GetPreferences([FromQuery] GetPreferencesRequest getPreferencesRequest)
    {
        try
        {
            List<GetPreferencesResponse> preferences = await databaseService.GetPreferencesAsync(getPreferencesRequest);

            if (preferences.Any())
                return new ObjectResult(new { preferences }) { StatusCode = 200 };

            return NotFound(preferences);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get preferences");
        }

        return new StatusCodeResult(500);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePreference(DeletePreferenceRequest deletePreferenceRequest)
    {
        try
        {
            bool isDeleted = await databaseService.DeletePreferenceAsync(deletePreferenceRequest);

            if (isDeleted)
                return new ObjectResult(new { isDeleted }) { StatusCode = 204 };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete preference");
        }

        return new StatusCodeResult(304);
    }
}