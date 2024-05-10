using System.Net;

namespace PreferencesApi.Middleware;

public class WriteToCacheMiddleware(IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Write to cache
        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post, $"http://localhost:5169{context.Request.Path.Value}");

        HttpClient? httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Failed to write entry to cache.");
        }
        else
            context.Response.StatusCode = StatusCodes.Status201Created;

        await next(context);
    }
}