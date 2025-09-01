using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace learning.WebApi.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = await FormatRequest(context.Request);
        request = MaskSensitiveFields(request);
        
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();
    
        var response = await FormatResponse(context.Response);
        await responseBody.CopyToAsync(originalBodyStream);
        
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = context.User?.FindFirst(ClaimTypes.Name)?.Value;
        
        _logger.LogInformation("HTTP Request/Response Info: {@LogInfo}", new
        {
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            StatusCode = context.Response.StatusCode,
            Duration = stopwatch.ElapsedMilliseconds,
            User = "UserId: "+ userId+", UserName: "+ userName,
            RequestBody = request,
            ResponseBody = response
        });
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        string body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }
    
    private string MaskSensitiveFields(string requestBody)
    {
        try
        {
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
            if (json != null)
            {
                var sensitiveKeys = new[] { "password", "confirmPassword", "token" };
                foreach (var key in sensitiveKeys)
                {
                    if (json.ContainsKey(key))
                    {
                        json[key] = "***";
                    }
                }
                return JsonSerializer.Serialize(json);
            }
        }
        catch
        {
            // If not JSON or deserialization fails, just return the original body
        }

        return requestBody;
    }

}
