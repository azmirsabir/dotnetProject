using System.ComponentModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace learning.Model;

public class QueryParameters
{
    [SwaggerSchema("Sorting field, e.g., 'id' or '-id', write [-fieldname] for ascending order")]
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    
    public string? Include { get; set; }=string.Empty;

    // Dynamic filters

    [SwaggerSchema(
        Description = 
            "Dynamic filters passed as query parameters in key-value format.\n\n" +
            "Examples:\n" +
            "- Equal: `filters[fieldName1]=value`\n" +
            "- Like: `filters[fieldName2]=*Az*` , its like '%Az%'\n" +
            "- In: `filters[fieldName3]=1,3`, it's fieldname3 in(1,3)\n" +
            "- Between: `filters[createdDate]=2024-01-01,2024-01-31`\n\n" +
            "Note: In Swagger UI, provide the filters as a JSON object:\n" +
            "`{\n  \"fieldname2\": \"*Az*\",\n  \"fieldname2\": \"1,3\"\n}`"
    )]
    [DefaultValue(typeof(Dictionary<string, string>), "")]
    public Dictionary<string, string>? Filters { get; set; } = new();
}