using System.Net;

namespace PreferencesApi.Middleware;

public class ReadFromCacheMiddleware(IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage
            (HttpMethod.Get, $"http://localhost:5169{context.Request.Path.Value}{context.Request.QueryString}");

        HttpClient? httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.StatusCode == HttpStatusCode.OK)
            await context.Response.WriteAsJsonAsync(response.Content.ToString()); // Short circuit: write the response we got from the cache off the bat
        else
            await next(context); // Go to the next RateLimiting middleware in the pipeline
    }
}