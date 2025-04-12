namespace COMP2139_assign01.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // Add these properties to fix the errors
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
}