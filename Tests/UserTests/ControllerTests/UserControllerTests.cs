using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using COMP2139_assign01.Areas.User;
using COMP2139_assign01.Areas.User.Models;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using Xunit;

namespace COMP2139_assign01.Tests.Areas.User
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly UserController.AccountController _controller;
        private readonly Mock<IUrlHelper> _mockUrlHelper;

        public AccountControllerTests()
        {
            _mockUserManager = MockUserManager<ApplicationUser>();
            _mockSignInManager = MockSignInManager();
            _mockEmailSender = new Mock<IEmailSender>();
            
            _controller = new UserController.AccountController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockEmailSender.Object
            );
            
            _mockUrlHelper = new Mock<IUrlHelper>();
            _controller.Url = _mockUrlHelper.Object;
            
            // Setup temp data
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), 
                Mock.Of<ITempDataProvider>());
        }

        #region Register Tests

        [Fact]
        public void Register_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();
            
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_Post_WithValidModel_CreatesUserAndRedirects()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "Test User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            _mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("token");
            
            _mockUrlHelper.Setup(m => m.Action(It.IsAny<UrlActionContext>()))
                .Returns("callbackUrl");
            
            // Act
            var result = await _controller.Register(model);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RegisterConfirmation", redirectResult.ActionName);
            
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "Password123!"), Times.Once);
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
            _mockEmailSender.Verify(m => m.SendEmailAsync(
                "test@example.com", 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task Register_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new RegisterViewModel();
            _controller.ModelState.AddModelError("Email", "Required");
            
            // Act
            var result = await _controller.Register(model);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Register_Post_WhenUserCreationFails_ReturnsViewWithErrors()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "Test User"
            };
            
            var errors = new List<IdentityError> 
            {
                new IdentityError { Description = "Error 1" }
            };
            
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));
            
            // Act
            var result = await _controller.Register(model);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            
            // Verify that the error was added to ModelState
            Assert.False(_controller.ModelState.IsValid);
        }

        #endregion

        #region Login Tests

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Arrange
            var returnUrl = "/home";
            
            // Act
            var result = _controller.Login(returnUrl);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(returnUrl, viewResult.ViewData["ReturnUrl"]);
        }

        [Fact]
        public async Task Login_Post_WithValidCredentials_RedirectsToReturnUrl()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };
            
            var returnUrl = "/home";
            
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            
            _mockUrlHelper.Setup(m => m.IsLocalUrl(It.IsAny<string>())).Returns(true);
            
            // Act
            var result = await _controller.Login(model, returnUrl);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
            
            _mockSignInManager.Verify(m => m.PasswordSignInAsync(
                "test@example.com", 
                "Password123!", 
                true, 
                true), 
                Times.Once);
        }

        [Fact]
        public async Task Login_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new LoginViewModel();
            _controller.ModelState.AddModelError("Email", "Required");
            
            // Act
            var result = await _controller.Login(model);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };
            
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            
            // Act
            var result = await _controller.Login(model);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            
            // Verify that the error was added to ModelState
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_Post_WithLockedOutAccount_RedirectsToLockout()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);
            
            // Act
            var result = await _controller.Login(model);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Lockout", redirectResult.ActionName);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_Post_SignsOutAndRedirectsToHome()
        {
            // Act
            var result = await _controller.Logout();
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            
            _mockSignInManager.Verify(m => m.SignOutAsync(), Times.Once);
        }

        #endregion

        #region Password Reset Tests

        [Fact]
        public void ForgotPassword_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.ForgotPassword();
            
            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_Post_WithValidEmail_SendsResetEmailAndRedirects()
        {
            // Arrange
            var model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };
            
            var user = new ApplicationUser { Email = "test@example.com" };
            
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            
            _mockUserManager.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(true);
            
            _mockUserManager.Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("token");
            
            _mockUrlHelper.Setup(m => m.Action(It.IsAny<UrlActionContext>()))
                .Returns("callbackUrl");
            
            // Act
            var result = await _controller.ForgotPassword(model);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", redirectResult.ActionName);
            
            _mockEmailSender.Verify(m => m.SendEmailAsync(
                "test@example.com", 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public void ResetPassword_Get_WithCode_ReturnsViewWithModel()
        {
            // Arrange
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("resetToken"));
            
            // Act
            var result = _controller.ResetPassword(code);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(code, model.Code);
        }

        [Fact]
        public async Task ResetPassword_Post_WithValidModelAndCode_ResetsPasswordAndRedirects()
        {
            // Arrange
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("resetToken"));
            
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                Code = code
            };
            
            var user = new ApplicationUser { Email = "test@example.com" };
            
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            
            _mockUserManager.Setup(m => m.ResetPasswordAsync(
                It.IsAny<ApplicationUser>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            // Act
            var result = await _controller.ResetPassword(model);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResetPasswordConfirmation", redirectResult.ActionName);
            
            _mockUserManager.Verify(m => m.ResetPasswordAsync(
                user, 
                "resetToken", 
                "NewPassword123!"), 
                Times.Once);
        }

        #endregion

        #region Profile Tests

        [Fact]
        public async Task Profile_Get_ReturnsViewWithUserData()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@example.com",
                FullName = "Test User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            
            // Act
            var result = await _controller.Profile();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            
            Assert.Equal("test@example.com", model.Email);
            Assert.Equal("Test User", model.FullName);
            Assert.Equal("123 Test St", model.Address);
            Assert.Equal("1234567890", model.PhoneNumber);
        }

        [Fact]
        public async Task Profile_Post_WithValidModel_UpdatesUserAndRedirects()
        {
            // Arrange
            var model = new ProfileViewModel
            {
                Email = "test@example.com",
                FullName = "Updated User",
                Address = "456 New St",
                PhoneNumber = "0987654321"
            };
            
            var user = new ApplicationUser
            {
                Email = "test@example.com",
                FullName = "Test User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            
            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            
            // Act
            var result = await _controller.Profile(model);
            
            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
            
            // Verify user was updated with new values
            Assert.Equal("Updated User", user.FullName);
            Assert.Equal("456 New St", user.Address);
            Assert.Equal("0987654321", user.PhoneNumber);
            
            _mockUserManager.Verify(m => m.UpdateAsync(user), Times.Once);
            Assert.Equal("Your profile has been updated", _controller.TempData["StatusMessage"]);
        }

        #endregion

        #region Helper Methods

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        private Mock<SignInManager<ApplicationUser>> MockSignInManager()
        {
            return new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null,
                null,
                null,
                null);
        }

        #endregion
    }
}