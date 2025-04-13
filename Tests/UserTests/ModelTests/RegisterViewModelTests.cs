using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using COMP2139_assign01.Areas.User.Models;
using NUnit.Framework;

namespace COMP2139_assign01.Tests.Areas.User.Models
{
    [TestFixture]
    public class RegisterViewModelTests
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
            var model = new RegisterViewModel { 
                Email = string.Empty,
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Email = null,
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Email = "invalid-email-format",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
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
            var propertyInfo = typeof(RegisterViewModel).GetProperty("Email");
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
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = string.Empty,
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = null,
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "short", // Less than 6 characters
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = new string('a', 101), // More than 100 characters
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Password = "ValidPassword123"
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
            var propertyInfo = typeof(RegisterViewModel).GetProperty("Password");
            var dataTypeAttribute = propertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;
            
            // Assert
            Assert.That(dataTypeAttribute, Is.Not.Null);
            Assert.That(dataTypeAttribute.DataType, Is.EqualTo(DataType.Password));
        }
        
        [Test]
        public void Password_HasStringLengthAttribute_WithCorrectValues()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("Password");
            var stringLengthAttribute = propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as StringLengthAttribute;
            
            // Assert
            Assert.That(stringLengthAttribute, Is.Not.Null);
            Assert.That(stringLengthAttribute.MaximumLength, Is.EqualTo(100));
            Assert.That(stringLengthAttribute.MinimumLength, Is.EqualTo(6));
            Assert.That(stringLengthAttribute.ErrorMessage, Does.Contain("must be at least").And.Contains("and at max"));
        }
        
        [Test]
        public void Password_DisplayName_ShouldBe_Password()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("Password");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Password"));
        }
        
        #endregion
        
        #region ConfirmPassword Tests
        
        [Test]
        public void ConfirmPassword_ValidationFails_WhenNotMatchingPassword()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "DifferentPassword",
                FullName = "John Doe"
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
            var model = new RegisterViewModel { 
                Password = "Password123",
                ConfirmPassword = "Password123"
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
            var propertyInfo = typeof(RegisterViewModel).GetProperty("ConfirmPassword");
            var dataTypeAttribute = propertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;
            
            // Assert
            Assert.That(dataTypeAttribute, Is.Not.Null);
            Assert.That(dataTypeAttribute.DataType, Is.EqualTo(DataType.Password));
        }
        
        [Test]
        public void ConfirmPassword_HasCompareAttribute_WithPasswordProperty()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("ConfirmPassword");
            var compareAttribute = propertyInfo.GetCustomAttributes(typeof(CompareAttribute), false).FirstOrDefault() as CompareAttribute;
            
            // Assert
            Assert.That(compareAttribute, Is.Not.Null);
            Assert.That(compareAttribute.OtherProperty, Is.EqualTo("Password"));
            Assert.That(compareAttribute.ErrorMessage, Does.Contain("do not match"));
        }
        
        [Test]
        public void ConfirmPassword_DisplayName_ShouldBe_ConfirmPassword()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("ConfirmPassword");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Confirm password"));
        }
        
        #endregion
        
        #region FullName Tests
        
        [Test]
        public void FullName_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = string.Empty
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("FullName") && 
                                          r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                       Is.True);
        }
        
        [Test]
        public void FullName_Required_ValidationFails_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = null
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Any(r => r.MemberNames.Contains("FullName") && 
                                          r.ErrorMessage.Contains("required", System.StringComparison.OrdinalIgnoreCase)), 
                       Is.True);
        }
        
        [Test]
        public void FullName_ValidationSucceeds_WithValidValue()
        {
            // Arrange
            var model = new RegisterViewModel { 
                FullName = "John Doe"
            };
            _context = new ValidationContext(model);
            _context.MemberName = "FullName";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.FullName, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void FullName_DisplayName_ShouldBe_FullName()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("FullName");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Full Name"));
        }
        
        #endregion
        
        #region Optional Properties Tests
        
        [Test]
        public void Address_IsOptional_ValidationSucceeds_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                Address = null
            };
            _context = new ValidationContext(model);
            _context.MemberName = "Address";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.Address, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void City_IsOptional_ValidationSucceeds_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                City = null
            };
            _context = new ValidationContext(model);
            _context.MemberName = "City";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.City, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void State_IsOptional_ValidationSucceeds_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                State = null
            };
            _context = new ValidationContext(model);
            _context.MemberName = "State";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.State, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void ZipCode_IsOptional_ValidationSucceeds_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                ZipCode = null
            };
            _context = new ValidationContext(model);
            _context.MemberName = "ZipCode";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.ZipCode, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        #endregion
        
        #region PhoneNumber Tests
        
        [Test]
        public void PhoneNumber_IsOptional_ValidationSucceeds_WhenNull()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                PhoneNumber = null
            };
            _context = new ValidationContext(model);
            _context.MemberName = "PhoneNumber";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.PhoneNumber, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void PhoneNumber_ValidationFails_WithInvalidFormat()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                PhoneNumber = "invalid-phone-number"
            };
            _context = new ValidationContext(model);
            _context.MemberName = "PhoneNumber";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.PhoneNumber, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(_results.Count, Is.EqualTo(1));
            Assert.That(_results[0].ErrorMessage, Does.Contain("phone").IgnoreCase);
        }
        
        [Test]
        public void PhoneNumber_ValidationSucceeds_WithValidFormat()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                PhoneNumber = "123-456-7890"
            };
            _context = new ValidationContext(model);
            _context.MemberName = "PhoneNumber";
            
            // Act
            var isValid = Validator.TryValidateProperty(model.PhoneNumber, _context, _results);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void PhoneNumber_HasPhoneAttribute()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("PhoneNumber");
            var phoneAttribute = propertyInfo.GetCustomAttributes(typeof(PhoneAttribute), false).FirstOrDefault();
            
            // Assert
            Assert.That(phoneAttribute, Is.Not.Null);
        }
        
        #endregion
        
        #region Display Names Tests
        
        [Test]
        public void Address_DisplayName_ShouldBe_Address()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("Address");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Address"));
        }
        
        [Test]
        public void City_DisplayName_ShouldBe_City()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("City");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("City"));
        }
        
        [Test]
        public void State_DisplayName_ShouldBe_State()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("State");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("State"));
        }
        
        [Test]
        public void ZipCode_DisplayName_ShouldBe_ZipCode()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("ZipCode");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Zip Code"));
        }
        
        [Test]
        public void PhoneNumber_DisplayName_ShouldBe_PhoneNumber()
        {
            // Arrange
            var propertyInfo = typeof(RegisterViewModel).GetProperty("PhoneNumber");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Phone Number"));
        }
        
        #endregion
        
        [Test]
        public void Model_ValidationSucceeds_WithAllRequiredFields()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe"
            };
            _context = new ValidationContext(model);
            
            // Act
            var isValid = Validator.TryValidateObject(model, _context, _results, true);
            
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(_results.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Model_ValidationSucceeds_WithAllFields()
        {
            // Arrange
            var model = new RegisterViewModel { 
                Email = "test@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                FullName = "John Doe",
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                PhoneNumber = "123-456-7890"
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