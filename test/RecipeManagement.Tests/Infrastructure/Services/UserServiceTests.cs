using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RecipeManagement.Domain;
using RecipeManagement.Infrastructure.Cryptography;
using RecipeManagement.Infrastructure.Exceptions;
using RecipeManagement.Infrastructure.Services;
using RecipeManagement.Tests.Extensions;

namespace RecipeManagement.Tests.Infrastructure.Services
{
    [TestFixture]
    public class UserServiceTests : ServiceTestBase
    {
        private UserService _userService;
        private Mock<ICryptographyService> _cryptographyServiceMock;

        [SetUp]
        public void Setup()
        {
            _cryptographyServiceMock = new Mock<ICryptographyService>();
            _userService = new UserService(RecipeManagementContext, _cryptographyServiceMock.Object);
        }

        [Test]
        public void AddUserTest()
        {
            //Arrange
            _cryptographyServiceMock.Setup(service => service.CreateSalt()).Returns("salt");
            _cryptographyServiceMock.Setup(service => service.CreatePasswordHash("password", "salt"))
                .Returns("hashedPassword");

            //Act
            User user = _userService.AddUser("Name", "Surname", "name.surname@email.com", "0721234567", "password");

            //Assert
            Assert.That(user.UserId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(user.Name, Is.EqualTo("Name"));
            Assert.That(user.Surname, Is.EqualTo("Surname"));
            Assert.That(user.CellphoneNumber, Is.EqualTo("0721234567"));
            Assert.That(user.EmailAddress, Is.EqualTo("name.surname@email.com"));
            Assert.That(RecipeManagementContext.Users.Count(), Is.EqualTo(1));
            Assert.That(user.PasswordSalt, Is.EqualTo("salt"));
            Assert.That(user.Password, Is.EqualTo("hashedPassword"));

            _cryptographyServiceMock.Verify(service => service.CreateSalt(), Times.Once());
            _cryptographyServiceMock.Verify(service => service.CreatePasswordHash("password", "salt"), Times.Once());
            RecipeManagementContext.VerifySave();
        }

        [Test]
        public void AddUserTest_Given_That_User_With_The_Same_Email_Exists_Then_Exception_Should_Be_Thrown()
        {
            //Arrange
            const string emailAddress = "name.surname@email.com";
            RecipeManagementContext.PrepareTestData(context =>
            {
                context.Users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    EmailAddress = emailAddress
                });
            });
            //Act
            var exception = Assert.Throws<RecipeManagementException>(() =>
                _userService.AddUser("Name", "Surname", "name.surname@email.com", "0721234567", "Password123"));

            //Assert
            Assert.That(exception.Message, Is.EqualTo("User with specified email address already exists"));
        }

        [Test]
        public void UpdateUserTest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Name = "Old Name",
                Surname = "Old Surname",
                EmailAddress = "old-email@email.com",
                CellphoneNumber = "0720000000"
            };

            RecipeManagementContext.PrepareTestData(context => { context.Users.Add(user); });

            //Act
            _userService.UpdateUser(userId, "Name", "Surname", "name.surname@email.com", "0721234567");

            //Assert
            Assert.That(user.UserId, Is.EqualTo(userId));
            Assert.That(user.Name, Is.EqualTo("Name"));
            Assert.That(user.Surname, Is.EqualTo("Surname"));
            Assert.That(user.CellphoneNumber, Is.EqualTo("0721234567"));
            Assert.That(user.EmailAddress, Is.EqualTo("name.surname@email.com"));
            Assert.That(RecipeManagementContext.Users.Count(), Is.EqualTo(1));

            RecipeManagementContext.VerifySave();
        }

        [Test]
        public void UpdateUserTest_Given_That_User_With_The_Same_Email_Exists_Then_Exception_Should_Be_Thrown()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Name = "Old Name",
                Surname = "Old Surname",
                EmailAddress = "old-email@email.com",
                CellphoneNumber = "0720000000"
            };

            RecipeManagementContext.PrepareTestData(context =>
            {
                context.Users.Add(user);
                context.Users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    Name = "Other Name",
                    Surname = "Other Surname",
                    EmailAddress = "name.surname@email.com",
                    CellphoneNumber = "0721111111"
                });
            });

            //Act
            var exception = Assert.Throws<RecipeManagementException>(() =>
                _userService.UpdateUser(userId, "Name", "Surname", "name.surname@email.com", "0721234567"));

            //Assert
            Assert.That(exception.Message, Is.EqualTo("User with specified email address already exists"));
        }
    }
}