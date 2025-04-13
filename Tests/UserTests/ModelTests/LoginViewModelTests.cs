using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using COMP2139_assign01.Areas.User.Models;
using NUnit.Framework;

namespace COMP2139_assign01.Tests.Areas.User.Models
{
    [TestFixture]
    public class LoginViewModelTests
    {
        private ValidationContext _context;
        private List<ValidationResult> _results;
        
        [SetUp]
        public void Setup()
        {
            _results = new List<ValidationResult>();
        }
        
        #region Email Tests
        
        [Test]
        public void Email_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = string.Empty,
                Password = "ValidPassword123"
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Email") && 
                                         r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                      Is.True);
        }
        
        [Test]
        public void Email_Required_ValidationFails_WhenNull()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = null,
                Password = "ValidPassword123" 
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Email") && 
                                         r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                      Is.True);
        }
        
        [Test]
        public void Email_ValidationFails_WithInvalidFormat()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "invalid-email-format",
                Password = "ValidPassword123" 
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Email") && 
                                         r.ErrorMessage.Contains("valid", System.StringComparison.OrdinalIgnoreCase)), 
                      Is.True);
        }
        
        [Test]
        public void Email_ValidationSucceeds_WithValidEmail()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "test@example.com",
                Password = "ValidPassword123" 
            };
            _context = new ValidationContext(model);
            
            // Act - validate only the Email property
            _context.MemberName = "Email";
            var isValid = Validator.TryValidateProperty(model.Email, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Email_DisplayName_ShouldBe_Email()
        {
            // Arrange
            var propertyInfo = typeof(LoginViewModel).GetProperty("Email");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Email"));
        }
        
        #endregion
        
        #region Password Tests
        
        [Test]
        public void Password_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "test@example.com",
                Password = string.Empty 
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Password") && 
                                         r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                      Is.True);
        }
        
        [Test]
        public void Password_Required_ValidationFails_WhenNull()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "test@example.com",
                Password = null 
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Password") && 
                                         r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                      Is.True);
        }
        
        [Test]
        public void Password_ValidationSucceeds_WithValidPassword()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "test@example.com",
                Password = "ValidPassword123" 
            };
            _context = new ValidationContext(model);
            
            // Act - validate only the Password property
            _context.MemberName = "Password";
            var isValid = Validator.TryValidateProperty(model.Password, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Password_HasDataTypeAttribute_WithPasswordType()
        {
            // Arrange
            var propertyInfo = typeof(LoginViewModel).GetProperty("Password");
            var dataTypeAttribute = propertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;
            
            // Assert
            Assert.That(dataTypeAttribute, Is.Not.Null);
            Assert.That(dataTypeAttribute.DataType, Is.EqualTo(DataType.Password));
        }
        
        [Test]
        public void Password_DisplayName_ShouldBe_Password()
        {
            // Arrange
            var propertyInfo = typeof(LoginViewModel).GetProperty("Password");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Password"));
        }
        
        #endregion
        
        #region RememberMe Tests
        
        [Test]
        public void RememberMe_DefaultValue_ShouldBe_False()
        {
            // Arrange
            var model = new LoginViewModel();
            
            // Assert
            Assert.That(model.RememberMe, Is.False);
        }
        
        [Test]
        public void RememberMe_DisplayName_ShouldBe_RememberMeQuestion()
        {
            // Arrange
            var propertyInfo = typeof(LoginViewModel).GetProperty("RememberMe");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Remember me?"));
        }
        
        #endregion
        
        [Test]
        public void Model_ValidationSucceeds_WithAllValidProperties()
        {
            // Arrange
            var model = new LoginViewModel { 
                Email = "test@example.com",
                Password = "ValidPassword123",
                RememberMe = true
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
    }
}