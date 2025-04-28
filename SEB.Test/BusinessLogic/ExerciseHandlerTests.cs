using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SEB.BusinessLogic;
using SEB.DataAccess.Interfaces;
using SEB.Models.Enums;
using SEB.Models;

namespace SEB.Test.BusinessLogic
{
    public class ExerciseHandlerTests
    {
        private Mock<IExerciseRepo> _exerciseRepoMock;
        private ExerciseHandler _exerciseHandler;

        [SetUp]
        public void Setup()
        {
            _exerciseRepoMock = new Mock<IExerciseRepo>();
            _exerciseHandler = new ExerciseHandler(_exerciseRepoMock.Object);
        }

        [Test]
        public void AddExercise_Should_Call_AddExercise_In_Repo()
        {
            // Arrange
            var exercise = new Exercise
            {
                UserId = Guid.NewGuid(),
                Type = ExerciseTypes.PushUps,
                Count = 30,
                Duration = 120
            };

            // Act
            _exerciseHandler.AddExercise(exercise);

            // Assert
            _exerciseRepoMock.Verify(repo => repo.AddExercise(It.IsAny<Exercise>()), Times.Once);
        }

        [Test]
        public void AddExercise_Should_Throw_Exception_When_Count_Is_NonPositive()
        {
            // Arrange
            var exercise = new Exercise
            {
                UserId = Guid.NewGuid(),
                Type = ExerciseTypes.PushUps,
                Count = 0, // Invalid: Count = 0
                Duration = 60
            };

            // Act + Assert
            Assert.Throws<Exception>(() => _exerciseHandler.AddExercise(exercise));
        }

        [Test]
        public void AddExercise_Should_Throw_Exception_When_Duration_Is_NonPositive()
        {
            // Arrange
            var exercise = new Exercise
            {
                UserId = Guid.NewGuid(),
                Type = ExerciseTypes.PushUps,
                Count = 10,
                Duration = 0 // Invalid: Duration = 0
            };

            // Act + Assert
            Assert.Throws<Exception>(() => _exerciseHandler.AddExercise(exercise));
        }

        [Test]
        public void GetExercisesByUserId_Should_Return_Exercises()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exercises = new List<Exercise>
                {
                    new Exercise { Count = 30, Duration = 120 },
                    new Exercise { Count = 20, Duration = 90 }
                };

            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(userId)).Returns(exercises);

            // Act
            var result = _exerciseHandler.GetExercisesByUserId(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void AddExercise_Should_Save_Exercise_With_Correct_Fields()
        {
            // Arrange
            var exercise = new Exercise
            {
                UserId = Guid.NewGuid(),
                Type = ExerciseTypes.Burpees,
                Count = 15,
                Duration = 60
            };

            // Act
            _exerciseHandler.AddExercise(exercise);

            // Assert
            _exerciseRepoMock.Verify(repo => repo.AddExercise(It.Is<Exercise>(e =>
                e.Type == ExerciseTypes.Burpees &&
                e.Count == 15 &&
                e.Duration == 60
            )), Times.Once);
        }
    }
}
