using System.Net;
using System.Text;

namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Middleware;

public class ResponseCachingMiddleware(IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // So we can store the response we get from our controller action result
        using MemoryStream? swapStream = new();
        context.Response.Body = swapStream;

        // Invoke the controller action
        await next(context);

        // We only want to write non-empty responses to the cache
        if (context.Response.StatusCode == 200)
        {
            string responseBody = await new StreamReader(swapStream).ReadToEndAsync();

            HttpRequestMessage? httpRequestMessage = new HttpRequestMessage
                (HttpMethod.Post, $"http://localhost:5169{context.Request.Path.Value}{context.Request.QueryString}");
            httpRequestMessage.Content = new StringContent(responseBody, Encoding.UTF8, "application/json");

            HttpClient? httpClient = httpClientFactory.CreateClient();
            HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Failed to write entry to cache.");
            }

            context.Response.StatusCode = StatusCodes.Status201Created;
        }
    }
}