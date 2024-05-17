using System.Net;

namespace PreferencesApi.Middleware;

public class ResponseCachingMiddleware (IHttpClientFactory httpClientFactory, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context); // Invoke the controller action

				// We only want to write non-empty responses to the cache
        if (context.Response.StatusCode == 200)
        {
						HttpRequestMessage? httpRequestMessage = new HttpRequestMessage
								(HttpMethod.Post, $"http://localhost:5169{context.Request.Path.Value}{context.Request.QueryString}");

						HttpClient? httpClient = httpClientFactory.CreateClient();
						HttpResponseMessage? response = await httpClient.SendAsync(httpRequestMessage);

						if (response.StatusCode == HttpStatusCode.BadRequest)
						{
								context.Response.StatusCode = StatusCodes.Status400BadRequest;
								await context.Response.WriteAsync("Failed to write entry to cache.");
						}
						else
								context.Response.StatusCode = StatusCodes.Status201Created;
				}
    }
}