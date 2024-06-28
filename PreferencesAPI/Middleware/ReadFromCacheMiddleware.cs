namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Middleware;

public class ReadFromCacheMiddleware(IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        HttpRequestMessage? httpRequestMessage = new HttpRequestMessage
            (HttpMethod.Get, $"http://localhost:5169{context.Request.Path.Value}{context.Request.QueryString}");

        HttpClient? httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

        if (response.IsSuccessStatusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync(), System.Text.Encoding.UTF8);
        }
        else
            await next(context); // Go to the next middleware in the pipeline
    }
}