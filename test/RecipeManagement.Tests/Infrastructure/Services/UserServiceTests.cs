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
            Assert.That(user.DateCreated, Is.EqualTo(DateTime.Now).Within(1).Minutes);
            Assert.That(user.DateUpdated, Is.EqualTo(DateTime.Now).Within(1).Minutes);

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
                CellphoneNumber = "0720000000",
                DateCreated = DateTime.Now.AddDays(-1)
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
            Assert.That(user.DateCreated, Is.EqualTo(DateTime.Now.AddDays(-1)).Within(1).Minutes);
            Assert.That(user.DateUpdated, Is.EqualTo(DateTime.Now).Within(1).Minutes);

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

        [TestCase("password", true)]
        [TestCase("wrong password", false)]
        public void AuthenticateUserTest_Given_Email_And_Password_Then_True_Should_Be_Returned_If_Password_Match(
            string inputPassword, bool expectedResult)
        {
            //Arrange
            const string passwordSalt = "passwordSalt";

            var cryptographyService = new CryptographyService();
            string password = cryptographyService.CreatePasswordHash("password", passwordSalt);

            _cryptographyServiceMock.Setup(service => service.CreatePasswordHash("password", passwordSalt))
                .Returns(password);

            RecipeManagementContext.PrepareTestData(context =>
            {
                context.Users.Add(new User
                {
                    EmailAddress = "test@email.com",
                    Password = password,
                    PasswordSalt = passwordSalt
                });
            });

            //Act
            bool isAuthenticated = _userService.AuthenticateUser("test@email.com", inputPassword);

            //Assert
            Assert.That(isAuthenticated, Is.EqualTo(expectedResult));
        }

        [Test]
        public void
            AuthenticateUserTest_Given_Email_And_Password_And_User_Could_Not_Be_Found_With_Specified_Email_Then_False_Should_Be_Returned()
        {
            //Arrange
            const string email = "user@email.com";

            //Act
            bool isAuthenticated = _userService.AuthenticateUser(email, "password");

            //Assert
            Assert.That(isAuthenticated, Is.EqualTo(false));
        }

        [Test]
        public void AuthenticateUserTest_Given_Email_And_Password_And_User_Is_Not_Active_Then_False_Should_Be_Returned()
        {
            //Arrange
            const string email = "user@email.com";
            RecipeManagementContext.PrepareTestData(context =>
            {
                context.Users.Add(new User
                {
                    EmailAddress = email,
                    Password = "password",
                    PasswordSalt = "passwordSalt"
                });
            });

            //Act
            bool isAuthenticated = _userService.AuthenticateUser(email, "password");

            //Assert
            Assert.That(isAuthenticated, Is.EqualTo(false));
        }

        [Test]
        public void
            ChangePassword_Given_UserId_And_NewPassword_Then_Password_Should_Be_Changed_With_Newly_Generated_Salt()
        {
            //Arrange
            var userId = Guid.NewGuid();
            const string oldPassword = "OldPassword";
            const string newPassword = "NewPassword";
            const string oldSalt = "OldSalt";

            RecipeManagementContext.PrepareTestData(context =>
            {
                context.Users.Add(new User
                {
                    UserId = userId,
                    Password = oldPassword,
                    PasswordSalt = oldSalt
                });
            });

            _cryptographyServiceMock.Setup(service => service.CreateSalt()).Returns("Salt");
            _cryptographyServiceMock.Setup(s => s.CreatePasswordHash(newPassword, "Salt")).Returns("PasswordHash");

            //Act
            _userService.ChangePassword(userId, newPassword);

            //Assert
            Assert.That(RecipeManagementContext.Users.First().Password, Is.EqualTo("PasswordHash"));
            Assert.That(RecipeManagementContext.Users.First().PasswordSalt, Is.EqualTo("Salt"));
            Assert.That(RecipeManagementContext.Users.First().DateUpdated, Is.EqualTo(DateTime.Now).Within(1).Minutes);

            //Verify
            _cryptographyServiceMock.Verify(v => v.CreatePasswordHash(newPassword, "Salt"));
            RecipeManagementContext.VerifySave();
        }
    }
}