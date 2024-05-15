using System.Net;

namespace PreferencesApi.Middleware;

public class ReadFromCacheMiddleware(IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Only concerns GET requests, handle this as-is
        if (context.Request.Method != HttpMethods.Get)
            await next(context);

        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get, $"http://localhost:5000{context.Request.Path.Value}");

        HttpClient? httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.StatusCode == HttpStatusCode.OK)
            await context.Response.WriteAsJsonAsync(response.Content); // Write the response we got from the cache off the bat

        await next(context); // Go to the next RateLimiting middleware in the pipeline
    }
}