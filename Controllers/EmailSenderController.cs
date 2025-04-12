using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using COMP2139_assign01.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace COMP2139_assign01.Controllers
{
    public class EmailSenderController : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSenderController> _logger;
        private const int MaxRetries = 3;
        
        public EmailSenderController(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailSenderController> logger)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "Email recipient cannot be null or empty");
            
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty");
            
            if (string.IsNullOrEmpty(htmlMessage))
                throw new ArgumentNullException(nameof(htmlMessage), "Email message cannot be null or empty");
            
            // Validate email format
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid email format: {Email}", email);
                throw new FormatException($"The email address '{email}' is not in a valid format");
            }
            
            // Log email attempt
            _logger.LogInformation("Attempting to send email to {Recipient} with subject '{Subject}'", 
                email, subject);
            
            // Configure SMTP client
            SmtpClient client = null;
            
            try
            {
                // Validate SMTP settings
                ValidateSmtpSettings();
                
                client = new SmtpClient
                {
                    Host = _emailSettings.Host,
                    Port = _emailSettings.Port,
                    EnableSsl = _emailSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                    Timeout = 30000 // 30 seconds timeout
                };
                
                // Implement retry logic with exponential backoff
                int attempt = 0;
                bool emailSent = false;
                Exception lastException = null;
                
                while (attempt < MaxRetries && !emailSent)
                {
                    try
                    {
                        attempt++;
                        
                        using (var message = new MailMessage())
                        {
                            message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                            message.Subject = subject;
                            message.Body = htmlMessage;
                            message.IsBodyHtml = true;
                            message.To.Add(new MailAddress(email));
                            
                            // Set priority
                            message.Priority = MailPriority.Normal;
                            
                            // Add headers for tracking
                            message.Headers.Add("X-MessageID", Guid.NewGuid().ToString());
                            
                            _logger.LogDebug("Sending email attempt {Attempt}/{MaxRetries}", attempt, MaxRetries);
                            await client.SendMailAsync(message);
                            
                            emailSent = true;
                            _logger.LogInformation("Email sent successfully to {Recipient} after {Attempts} attempt(s)", 
                                email, attempt);
                        }
                    }
                    catch (SmtpException ex) when (IsTransientSmtpError(ex))
                    {
                        lastException = ex;
                        _logger.LogWarning(ex, "Transient SMTP error on attempt {Attempt}/{MaxRetries}. Will retry. Error: {ErrorMessage}", 
                            attempt, MaxRetries, ex.Message);
                        
                        if (attempt < MaxRetries)
                        {
                            // Exponential backoff: 1s, 2s, 4s, etc.
                            int delayMs = (int)Math.Pow(2, attempt - 1) * 1000;
                            await Task.Delay(delayMs);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Non-transient error or other exception
                        lastException = ex;
                        _logger.LogError(ex, "Non-transient error on attempt {Attempt}/{MaxRetries}. Error: {ErrorMessage}", 
                            attempt, MaxRetries, ex.Message);
                        break; // Exit the retry loop for non-transient errors
                    }
                }
                
                // If all attempts failed, throw the last exception
                if (!emailSent)
                {
                    _logger.LogError("Failed to send email to {Recipient} after {MaxRetries} attempts", 
                        email, MaxRetries);
                    
                    if (lastException != null)
                        throw new EmailSendException($"Failed to send email after {MaxRetries} attempts", lastException);
                    else
                        throw new EmailSendException($"Failed to send email after {MaxRetries} attempts");
                }
            }
            catch (SmtpException ex)
            {
                LogSmtpError(ex, email);
                throw new EmailSendException("SMTP error occurred while sending email", ex);
            }
            catch (EmailSendException)
            {
                // Just re-throw as we've already logged it
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {Recipient}: {ErrorMessage}", 
                    email, ex.Message);
                throw new EmailSendException("An unexpected error occurred while sending email", ex);
            }
            finally
            {
                client?.Dispose();
            }
        }
        
        private void ValidateSmtpSettings()
        {
            if (string.IsNullOrEmpty(_emailSettings.Host))
                throw new InvalidOperationException("SMTP host is not configured");
                
            if (_emailSettings.Port <= 0)
                throw new InvalidOperationException("SMTP port is not properly configured");
                
            if (string.IsNullOrEmpty(_emailSettings.UserName))
                throw new InvalidOperationException("SMTP username is not configured");
                
            if (string.IsNullOrEmpty(_emailSettings.Password))
                throw new InvalidOperationException("SMTP password is not configured");
                
            if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
                throw new InvalidOperationException("Sender email is not configured");
        }
        
        private bool IsTransientSmtpError(SmtpException ex)
        {
            // SMTP status codes that are considered transient/temporary
            var transientStatusCodes = new[] { 
                SmtpStatusCode.ServiceNotAvailable,
                SmtpStatusCode.MailboxBusy,
                SmtpStatusCode.LocalErrorInProcessing,
                SmtpStatusCode.InsufficientStorage,
                SmtpStatusCode.ExceededStorageAllocation
            };
            
            return transientStatusCodes.Contains(ex.StatusCode) || 
                   ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                   ex.InnerException is System.Net.Sockets.SocketException;
        }
        
        private void LogSmtpError(SmtpException ex, string recipient)
        {
            if (ex.StatusCode != SmtpStatusCode.GeneralFailure)
            {
                _logger.LogError(ex, "SMTP error sending email to {Recipient}: {StatusCode} - {ErrorMessage}", 
                    recipient, ex.StatusCode, ex.Message);
            }
            else if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "SMTP inner error sending email to {Recipient}: {ErrorMessage}", 
                    recipient, ex.InnerException.Message);
            }
            else
            {
                _logger.LogError(ex, "General SMTP failure sending email to {Recipient}: {ErrorMessage}", 
                    recipient, ex.Message);
            }
        }
    }
    
    // Custom exception for email sending failures
    public class EmailSendException : Exception
    {
        public EmailSendException(string message) : base(message) { }
        
        public EmailSendException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}