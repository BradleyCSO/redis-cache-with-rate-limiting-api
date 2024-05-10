using System.Net;

namespace PreferencesApi.Middleware;

public class RateLimitingMiddleware (IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get, $"http://localhost:5170{context.Request.Path.Value}");

        HttpClient? httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests!");
        }
        else
            await next(context);
    }
}