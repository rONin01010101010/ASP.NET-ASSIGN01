using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using COMP2139_assign01.Areas.User.Models;
using NUnit.Framework;

namespace COMP2139_assign01.Tests.Areas.User.Models
{
    [TestFixture]
    public class ResetPasswordViewModelTests
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
            var model = new ResetPasswordViewModel { 
                Email = string.Empty,
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123",
                Code = "ResetToken123"
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
            var model = new ResetPasswordViewModel { 
                Email = null,
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123",
                Code = "ResetToken123"
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
            var model = new ResetPasswordViewModel { 
                Email = "invalid-email-format",
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123",
                Code = "ResetToken123"
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
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com"
            };
            _context = new ValidationContext(model);
            _context.MemberName = "Email";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.Email, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Email_DisplayName_ShouldBe_Email()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("Email");
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
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                Code = "ResetToken123"
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
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = null,
                ConfirmPassword = null,
                Code = "ResetToken123"
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
        public void Password_ValidationFails_WhenTooShort()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = "short", // Less than 6 characters
                ConfirmPassword = "short",
                Code = "ResetToken123"
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Password") && 
                                          r.ErrorMessage.Contains("6", System.StringComparison.OrdinalIgnoreCase)), 
                       Is.True);
        }
        
        [Test]
        public void Password_ValidationFails_WhenTooLong()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = new string('a', 101), // More than 100 characters
                ConfirmPassword = new string('a', 101),
                Code = "ResetToken123"
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("Password") && 
                                          r.ErrorMessage.Contains("100", System.StringComparison.OrdinalIgnoreCase)), 
                       Is.True);
        }
        
        [Test]
        public void Password_ValidationSucceeds_WithValidLength()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Password = "NewPassword123"
            };
            _context = new ValidationContext(model);
            _context.MemberName = "Password";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.Password, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Password_HasDataTypeAttribute_WithPasswordType()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("Password");
            var dataTypeAttribute = propertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;
            
            // Assert
            Assert.That(dataTypeAttribute, Is.Not.Null);
            Assert.That(dataTypeAttribute.DataType, Is.EqualTo(DataType.Password));
        }
        
        [Test]
        public void Password_HasStringLengthAttribute_WithCorrectValues()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("Password");
            var stringLengthAttribute = propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as StringLengthAttribute;
            
            // Assert
            Assert.That(stringLengthAttribute, Is.Not.Null);
            Assert.That(stringLengthAttribute.MaximumLength, Is.EqualTo(100));
            Assert.That(stringLengthAttribute.MinimumLength, Is.EqualTo(6));
            Assert.That(stringLengthAttribute.ErrorMessage, Does.Contain("must be at least").And.Contains("and at max"));
        }
        
        [Test]
        public void Password_DisplayName_ShouldBe_NewPassword()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("Password");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("New password"));
        }
        
        #endregion
        
        #region ConfirmPassword Tests
        
        [Test]
        public void ConfirmPassword_ValidationFails_WhenNotMatchingPassword()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = "NewPassword123",
                ConfirmPassword = "DifferentPassword",
                Code = "ResetToken123"
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("ConfirmPassword") && 
                                          r.ErrorMessage.Contains("match", System.StringComparison.OrdinalIgnoreCase)), 
                       Is.True);
        }
        
        [Test]
        public void ConfirmPassword_ValidationSucceeds_WhenMatchingPassword()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };
            _context = new ValidationContext(model);
            
            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(model.ConfirmPassword, 
                                                       new ValidationContext(model) { MemberName = "ConfirmPassword" },
                                                       validationResults);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(validationResults.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void ConfirmPassword_HasDataTypeAttribute_WithPasswordType()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("ConfirmPassword");
            var dataTypeAttribute = propertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;
            
            // Assert
            Assert.That(dataTypeAttribute, Is.Not.Null);
            Assert.That(dataTypeAttribute.DataType, Is.EqualTo(DataType.Password));
        }
        
        [Test]
        public void ConfirmPassword_HasCompareAttribute_WithPasswordProperty()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("ConfirmPassword");
            var compareAttribute = propertyInfo.GetCustomAttributes(typeof(CompareAttribute), false).FirstOrDefault() as CompareAttribute;
            
            // Assert
            Assert.That(compareAttribute, Is.Not.Null);
            Assert.That(compareAttribute.OtherProperty, Is.EqualTo("Password"));
            Assert.That(compareAttribute.ErrorMessage, Does.Contain("do not match"));
        }
        
        [Test]
        public void ConfirmPassword_DisplayName_ShouldBe_ConfirmNewPassword()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("ConfirmPassword");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Confirm new password"));
        }
        
        #endregion
        
        #region Code Tests
        
        [Test]
        public void Code_CanBeSet_AndRetrieved()
        {
            // Arrange
            var expectedValue = "SomeResetToken123";
            var model = new ResetPasswordViewModel { Code = expectedValue };
            
            // Act & Assert
            Assert.That(model.Code, Is.EqualTo(expectedValue));
        }
        
        [Test]
        public void Code_HasNoValidationAttributes()
        {
            // Arrange
            var propertyInfo = typeof(ResetPasswordViewModel).GetProperty("Code");
            var validationAttributes = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), false);
            
            // Assert
            Assert.That(validationAttributes.Length, Is.EqualTo(0), "Code should have no validation attributes");
        }
        
        #endregion
        
        [Test]
        public void Model_ValidationSucceeds_WithAllRequiredFields()
        {
            // Arrange
            var model = new ResetPasswordViewModel { 
                Email = "test@example.com",
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123",
                Code = "ResetToken123"
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