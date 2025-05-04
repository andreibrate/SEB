using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.BusinessLogic;
using SEB.DataAccess.Interfaces;
using SEB.Models;
using Moq;

namespace SEB.Test.BusinessLogic
{
    public class UserHandlerTests
    {
        private UserHandler _userHandler;
        private Mock<IUserRepo> _userRepoMock;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepo>();
            _userHandler = new UserHandler(_userRepoMock.Object);
        }

        [Test]
        public void Register_Should_Call_RegisterUser()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            _userHandler.Register(user);

            // Assert
            _userRepoMock.Verify(repo => repo.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public void Login_Should_Return_User_When_Credentials_Are_Correct()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Password = "password123",
                Token = "some-token"
            };
            _userRepoMock.Setup(repo => repo.LoginUser("testuser", "password123")).Returns(user);

            // Act
            var result = _userHandler.Login("testuser", "password123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public void Login_Should_Throw_Exception_When_Credentials_Are_Wrong()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.LoginUser("wronguser", "wrongpassword")).Returns((User)null);

            // Act + Assert
            Assert.Throws<Exception>(() => _userHandler.Login("wronguser", "wrongpassword"));
        }

        [Test]
        public void GetUserByUsername_Should_Return_User()
        {
            // Arrange
            var user = new User { Username = "testuser" };
            _userRepoMock.Setup(r => r.GetUserByUsername("testuser")).Returns(user);

            // Act
            var result = _userHandler.GetUserByUsername("testuser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public void GetUserByUsername_Should_Throw_When_Not_Found()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetUserByUsername("ghostuser")).Returns((User)null);

            // Act + Assert
            Assert.Throws<Exception>(() => _userHandler.GetUserByUsername("ghostuser"));
        }

        [Test]
        public void GetUserByToken_Should_Return_User()
        {
            // Arrange
            var user = new User { Token = "some-token" };
            _userRepoMock.Setup(r => r.GetUserByToken("some-token")).Returns(user);

            // Act
            var result = _userHandler.GetUserByToken("some-token");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("some-token", result.Token);
        }

        [Test]
        public void GetUserByToken_Should_Return_Null_When_Invalid()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetUserByToken("bad-token")).Returns((User)null);

            // Act
            var result = _userHandler.GetUserByToken("bad-token");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Register_Should_Throw_When_Username_Is_Taken()
        {
            // Arrange
            var user = new User { Username = "existingUser" };
            _userRepoMock.Setup(r => r.GetUserByUsername("existingUser")).Returns(user);

            // Act + Assert
            Assert.Throws<Exception>(() => _userHandler.Register(user));
        }


    }
}
