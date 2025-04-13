using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using COMP2139_assign01.Areas.User;

namespace COMP2139_assign01.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            string requestPath = context.Request.Path;
            string queryString = context.Request.QueryString.ToString();
            string userId = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";
            
            var errorId = Guid.NewGuid().ToString();
            
            switch (exception)
            {
                case ApplicationException _:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogWarning(exception, 
                        "Application error occurred. Error ID: {ErrorId}, Path: {Path}, User: {User}", 
                        errorId, requestPath + queryString, userId);
                    break;
                    
                case KeyNotFoundException _:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogWarning(exception, 
                        "Resource not found. Error ID: {ErrorId}, Path: {Path}, User: {User}", 
                        errorId, requestPath + queryString, userId);
                    break;
                    
                case UnauthorizedAccessException _:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    _logger.LogWarning(exception, 
                        "Unauthorized access attempt. Error ID: {ErrorId}, Path: {Path}, User: {User}", 
                        errorId, requestPath + queryString, userId);
                    break;
                    
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception, 
                        "Unhandled exception. Error ID: {ErrorId}, Path: {Path}, User: {User}", 
                        errorId, requestPath + queryString, userId);
                    break;
            }
            
            // For API requests, return a JSON response
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var response = JsonSerializer.Serialize(new { 
                    error = new { 
                        id = errorId,
                        message = "An error occurred. Please try again later.",
                        statusCode = context.Response.StatusCode
                    }
                });
                
                await context.Response.WriteAsync(response);
            }
            else
            {
                // For regular requests, redirect to the error page
                context.Items["OriginalPath"] = requestPath;
                context.Items["ErrorId"] = errorId;
                context.Items["Exception"] = exception;
                
                // Redirect to the appropriate error page
                context.Response.Redirect($"/Home/StatusCode?statusCode={context.Response.StatusCode}&errorId={errorId}");
            }
        }
    }
}