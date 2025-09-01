using System.ComponentModel.DataAnnotations;
using learning.Application.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace learning.Domain.Exceptions;

public class GlobalExceptionHandler : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        int statusCode;
        Response<object> response;

        switch (context.Exception)
        {
            case ValidationException ex:
                statusCode = StatusCodes.Status400BadRequest;
                response = new Response<object>(ex.Message);
                break;
            
            case InvalidOperationException ex:
                statusCode = StatusCodes.Status400BadRequest;
                response = new Response<object>(ex.Message);
                break;

            case UnauthorizedAccessException ex:
                statusCode = StatusCodes.Status401Unauthorized;
                response = new Response<object>(ex.Message);
                break;

            case CustomException ex:
                statusCode = ex.StatusCode;
                response = ex.Data;
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                response = new Response<object>("An unexpected error occurred");
                break;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }

}