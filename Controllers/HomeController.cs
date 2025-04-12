using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace COMP2139_assign01.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            _logger.LogInformation("User accessing home page. RequestId: {RequestId}", requestId);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in Index action. RequestId: {RequestId}", requestId);
            return RedirectToAction(nameof(Error));
        }
    }

    public IActionResult Privacy()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            _logger.LogInformation("User accessing privacy page. RequestId: {RequestId}", requestId);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in Privacy action. RequestId: {RequestId}", requestId);
            return RedirectToAction(nameof(Error));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;
            var path = exceptionFeature?.Path;

            if (exception != null)
            {
                _logger.LogError(exception, "An unhandled exception occurred at {Path}. RequestId: {RequestId}", 
                    path, requestId);
                
                // Log additional request information that may help with debugging
                _logger.LogDebug("Request details for error: Method={Method}, ContentType={ContentType}, QueryString={QueryString}. RequestId: {RequestId}",
                    HttpContext.Request.Method,
                    HttpContext.Request.ContentType,
                    HttpContext.Request.QueryString,
                    requestId);
            }
            else
            {
                _logger.LogWarning("Error page accessed directly without an exception. RequestId: {RequestId}", requestId);
            }

            return View(new ErrorViewModel
            {
                RequestId = requestId,
                ErrorMessage = "Sorry, an unexpected error occurred. Our technical team has been notified.",
                StatusCode = 500
            });
        }
        catch (Exception ex)
        {
            // Fallback error handling if Error action itself fails
            _logger.LogError(ex, "Error occurred within the Error action. RequestId: {RequestId}", requestId);
            
            // Use a more direct approach to return an error view
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                ErrorMessage = "Sorry, an unexpected error occurred.",
                StatusCode = 500
            });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCode(int statusCode)
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            _logger.LogInformation("Handling status code {StatusCode}. RequestId: {RequestId}", 
                statusCode, requestId);
            
            var errorViewModel = new ErrorViewModel
            {
                RequestId = requestId
            };

            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("OriginalPath"))
            {
                originalPath = HttpContext.Items["OriginalPath"] as string;
            }
            else
            {
                originalPath = HttpContext.Request.Path;
            }

            switch (statusCode)
            {
                case 400:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = "Bad request. Please check your input and try again.";
                    _logger.LogWarning("400 error occurred. Path: {Path}. RequestId: {RequestId}", 
                        originalPath, requestId);
                    return View("BadRequest", errorViewModel);

                case 401:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = "You are not authorized to access this resource.";
                    _logger.LogWarning("401 error occurred. Path: {Path}. RequestId: {RequestId}", 
                        originalPath, requestId);
                    return View("Unauthorized", errorViewModel);

                case 403:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = "You don't have permission to access this resource.";
                    _logger.LogWarning("403 error occurred. Path: {Path}. RequestId: {RequestId}", 
                        originalPath, requestId);
                    return View("Forbidden", errorViewModel);

                case 404:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = "Sorry, the page you requested could not be found.";
                    _logger.LogWarning("404 error occurred. Path: {Path}. RequestId: {RequestId}", 
                        originalPath, requestId);
                    return View("NotFound", errorViewModel);

                case 500:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = "A server error occurred. Please try again later.";
                    _logger.LogError("500 error occurred. Path: {Path}. RequestId: {RequestId}", 
                        originalPath, requestId);
                    return View("ServerError", errorViewModel);

                default:
                    errorViewModel.StatusCode = statusCode;
                    errorViewModel.ErrorMessage = $"An error occurred. Status code: {statusCode}";
                    _logger.LogWarning("Status code error {StatusCode} occurred. Path: {Path}. RequestId: {RequestId}",
                        statusCode, originalPath, requestId);
                    return View("Error", errorViewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred within StatusCode action for code {StatusCode}. RequestId: {RequestId}", 
                statusCode, requestId);
            
            // Fallback to basic error view if handling the status code fails
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                ErrorMessage = "Sorry, an unexpected error occurred.",
                StatusCode = 500
            });
        }
    }
}