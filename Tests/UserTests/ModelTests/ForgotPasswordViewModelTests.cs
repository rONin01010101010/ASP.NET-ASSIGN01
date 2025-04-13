using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using COMP2139_assign01.Areas.User.Models;
using NUnit.Framework;

namespace COMP2139_assign01.Tests.Areas.User.Models
{
    [TestFixture]
    public class ForgotPasswordViewModelTests
    {
        private ValidationContext _context;
        private List<ValidationResult> _results;
        
        [SetUp]
        public void Setup()
        {
            _results = new List<ValidationResult>();
        }
        
        [Test]
        public void Email_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var model = new ForgotPasswordViewModel { Email = string.Empty };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Count, Is.EqualTo(1));
            Assert.That(_results[0].ErrorMessage, Does.Contain("required").IgnoreCase);
        }
        
        [Test]
        public void Email_Required_ValidationFails_WhenNull()
        {
            // Arrange
            var model = new ForgotPasswordViewModel { Email = null };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Count, Is.EqualTo(1));
            Assert.That(_results[0].ErrorMessage, Does.Contain("required").IgnoreCase);
        }
        
        [Test]
        public void Email_ValidationFails_WithInvalidFormat()
        {
            // Arrange
            var model = new ForgotPasswordViewModel { Email = "invalid-email-format" };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Count, Is.EqualTo(1));
            Assert.That(_results[0].ErrorMessage, Does.Contain("valid").IgnoreCase);
        }
        
        [Test]
        public void Email_ValidationSucceeds_WithValidEmail()
        {
            // Arrange
            var model = new ForgotPasswordViewModel { Email = "test@example.com" };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void DisplayName_ShouldBe_Email()
        {
            // Arrange
            var propertyInfo = typeof(ForgotPasswordViewModel).GetProperty("Email");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Email"));
        }
    }
}