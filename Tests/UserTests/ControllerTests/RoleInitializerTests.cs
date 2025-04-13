using COMP2139_assign01.Areas.User;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace COMP2139_assign01.Tests
{
    public class RoleInitializerTests
    {
        [Fact]
        public async Task InitializeAsync_CreatesRolesAndAdminUser_WhenTheyDoNotExist()
        {
            // Arrange
            var serviceProvider = CreateServiceProviderMock();
            var roleManager = GetMockRoleManager();
            var userManager = GetMockUserManager();

            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();

            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
            
            serviceProvider.Setup(x => x.GetRequiredService<RoleManager<IdentityRole>>()).Returns(roleManager.Object);
            serviceProvider.Setup(x => x.GetRequiredService<UserManager<ApplicationUser>>()).Returns(userManager.Object);

            // Setup RoleManager
            roleManager.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(false);
            roleManager.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(false);
            roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            // Setup UserManager
            userManager.Setup(x => x.FindByEmailAsync("admin@example.com")).ReturnsAsync((ApplicationUser)null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await RoleInitializer.InitializeAsync(serviceProvider.Object);

            // Assert
            roleManager.Verify(x => x.RoleExistsAsync("Admin"), Times.Once);
            roleManager.Verify(x => x.RoleExistsAsync("User"), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "Admin")), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == "User")), Times.Once);
            
            userManager.Verify(x => x.FindByEmailAsync("admin@example.com"), Times.Once);
            userManager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u => u.Email == "admin@example.com" && u.EmailConfirmed == true), 
                "Admin@123"), 
                Times.Once);
            userManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"), Times.Once);
        }

        [Fact]
        public async Task InitializeAsync_DoesNotCreateRolesOrUser_WhenTheyAlreadyExist()
        {
            // Arrange
            var serviceProvider = CreateServiceProviderMock();
            var roleManager = GetMockRoleManager();
            var userManager = GetMockUserManager();

            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();

            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
            
            serviceProvider.Setup(x => x.GetRequiredService<RoleManager<IdentityRole>>()).Returns(roleManager.Object);
            serviceProvider.Setup(x => x.GetRequiredService<UserManager<ApplicationUser>>()).Returns(userManager.Object);

            // Setup RoleManager - roles already exist
            roleManager.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(true);
            roleManager.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);
            
            // Setup UserManager - admin user already exists
            var existingAdminUser = new ApplicationUser
            {
                Id = "admin-id",
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };
            
            userManager.Setup(x => x.FindByEmailAsync("admin@example.com")).ReturnsAsync(existingAdminUser);

            // Act
            await RoleInitializer.InitializeAsync(serviceProvider.Object);

            // Assert
            roleManager.Verify(x => x.RoleExistsAsync("Admin"), Times.Once);
            roleManager.Verify(x => x.RoleExistsAsync("User"), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
            
            userManager.Verify(x => x.FindByEmailAsync("admin@example.com"), Times.Once);
            userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        private Mock<IServiceProvider> CreateServiceProviderMock()
        {
            return new Mock<IServiceProvider>();
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
        }
    }
}