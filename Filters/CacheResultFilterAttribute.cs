using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace RedisCacheWithRateLimitingWebAPI.Filters;

public class CacheResultFilterAttribute (IHttpClientFactory httpClientFactory) : ResultFilterAttribute
{
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next(); // Get the response from our caching API

        if (context.HttpContext.Response.StatusCode == 404)
            return; // We don't want to cache empty responses, short circuit

        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage
            (HttpMethod.Post, $"http://localhost:5170{context.HttpContext.Request.Path.Value}");

        HttpClient? httpClient = httpClientFactory.CreateClient();
        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.HttpContext.Response.WriteAsync("Failed to write entry to cache.");
        }
        else
            context.HttpContext.Response.StatusCode = StatusCodes.Status201Created;
    }
}