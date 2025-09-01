using System.Text.Json.Serialization;

namespace learning.Application.Model.Response;

public class Response<T>
{
    public string Message { get; set; } = string.Empty;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data {get;set;}
    
    public Response() { }

    public Response(string message, T? data = default)
    {
        Message = message;
        Data = data;
    }
}