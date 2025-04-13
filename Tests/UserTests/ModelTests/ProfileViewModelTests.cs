using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using COMP2139_assign01.Areas.User.Models;
using NUnit.Framework;

namespace COMP2139_assign01.Tests.Areas.User.Models
{
    [TestFixture]
    public class ProfileViewModelTests
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
            var model = new ProfileViewModel { 
                Email = string.Empty,
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
            var model = new ProfileViewModel { 
                Email = null,
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
            var model = new ProfileViewModel { 
                Email = "invalid-email-format",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
                FullName = "John Doe"
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
            var propertyInfo = typeof(ProfileViewModel).GetProperty("Email");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Email"));
        }
        
        #endregion
        
        #region FullName Tests
        
        [Test]
        public void FullName_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var propertyInfo = typeof(ProfileViewModel).GetProperty("FullName");
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var propertyInfo = typeof(ProfileViewModel).GetProperty("PhoneNumber");
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
            var propertyInfo = typeof(ProfileViewModel).GetProperty("Address");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Address"));
        }
        
        [Test]
        public void City_DisplayName_ShouldBe_City()
        {
            // Arrange
            var propertyInfo = typeof(ProfileViewModel).GetProperty("City");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("City"));
        }
        
        [Test]
        public void State_DisplayName_ShouldBe_State()
        {
            // Arrange
            var propertyInfo = typeof(ProfileViewModel).GetProperty("State");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("State"));
        }
        
        [Test]
        public void ZipCode_DisplayName_ShouldBe_ZipCode()
        {
            // Arrange
            var propertyInfo = typeof(ProfileViewModel).GetProperty("ZipCode");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Zip Code"));
        }
        
        [Test]
        public void PhoneNumber_DisplayName_ShouldBe_PhoneNumber()
        {
            // Arrange
            var propertyInfo = typeof(ProfileViewModel).GetProperty("PhoneNumber");
            var displayAttribute = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            
            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Phone Number"));
        }
        
        #endregion
        
        [Test]
        public void Model_ValidationSucceeds_WithRequiredFields()
        {
            // Arrange
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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
            var model = new ProfileViewModel { 
                Email = "test@example.com",
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