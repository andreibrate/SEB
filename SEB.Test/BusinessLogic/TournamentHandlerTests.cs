using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SEB.BusinessLogic;
using SEB.DataAccess.Interfaces;
using SEB.Models;

namespace SEB.Test.BusinessLogic
{
    public class TournamentHandlerTests
    {
        private Mock<ITournamentRepo> _tournamentRepoMock;
        private Mock<IUserRepo> _userRepoMock;
        private Mock<IExerciseRepo> _exerciseRepoMock;
        private TournamentHandler _tournamentHandler;

        [SetUp]
        public void Setup()
        {
            _tournamentRepoMock = new Mock<ITournamentRepo>();
            _userRepoMock = new Mock<IUserRepo>();
            _exerciseRepoMock = new Mock<IExerciseRepo>();

            _tournamentHandler = new TournamentHandler(_tournamentRepoMock.Object, _exerciseRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public void FinishTournament_Should_Update_Winner_Elo_Correctly()
        {
            // Arrange
            var tournamentId = Guid.NewGuid();
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            var tournament = new Tournament(new List<Guid> { user1Id, user2Id }) { Id = tournamentId };

            _tournamentRepoMock.Setup(r => r.GetTournamentById(tournamentId)).Returns(tournament);
            _tournamentRepoMock.Setup(r => r.GetParticipants(tournamentId)).Returns(new List<Guid> { user1Id, user2Id });

            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(user1Id))
                .Returns(new List<Exercise> { new Exercise { Count = 30 } }); // 30 pushups
            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(user2Id))
                .Returns(new List<Exercise> { new Exercise { Count = 20 } }); // 20 pushups

            var user1 = new User { Id = user1Id, Username = "User1", Elo = 100 };
            var user2 = new User { Id = user2Id, Username = "User2", Elo = 100 };

            _userRepoMock.Setup(r => r.GetUserById(user1Id)).Returns(user1);
            _userRepoMock.Setup(r => r.GetUserById(user2Id)).Returns(user2);

            // Act
            _tournamentHandler.FinishTournament(tournamentId);

            // Assert
            Assert.AreEqual(102, user1.Elo); // Winner +2 elo
            Assert.AreEqual(99, user2.Elo);  // Loser -1 elo
        }

        [Test]
        public void FinishTournament_Should_Handle_Draw_Correctly()
        {
            // Arrange
            var tournamentId = Guid.NewGuid();
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            var tournament = new Tournament(new List<Guid> { user1Id, user2Id }) { Id = tournamentId };

            _tournamentRepoMock.Setup(r => r.GetTournamentById(tournamentId)).Returns(tournament);
            _tournamentRepoMock.Setup(r => r.GetParticipants(tournamentId)).Returns(new List<Guid> { user1Id, user2Id });

            // Both users have the SAME total pushups
            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(user1Id))
                .Returns(new List<Exercise> { new Exercise { Count = 25 } }); // 25 pushups
            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(user2Id))
                .Returns(new List<Exercise> { new Exercise { Count = 25 } }); // 25 pushups

            var user1 = new User { Id = user1Id, Username = "User1", Elo = 100 };
            var user2 = new User { Id = user2Id, Username = "User2", Elo = 100 };

            _userRepoMock.Setup(r => r.GetUserById(user1Id)).Returns(user1);
            _userRepoMock.Setup(r => r.GetUserById(user2Id)).Returns(user2);

            // Act
            _tournamentHandler.FinishTournament(tournamentId);

            // Assert
            Assert.AreEqual(101, user1.Elo); // Both winners get +1
            Assert.AreEqual(101, user2.Elo); // Both winners get +1

            _tournamentRepoMock.Verify(r => r.SetWinners(tournamentId, It.IsAny<List<Guid>>(), true), Times.Once); // isDraw = true

            _userRepoMock.Verify(r => r.UpdateUser(user1), Times.Once);
            _userRepoMock.Verify(r => r.UpdateUser(user2), Times.Once);
        }

        [Test]
        public void FinishTournament_Should_Throw_Exception_If_Tournament_Not_Found()
        {
            // Arrange
            var tournamentId = Guid.NewGuid();

            // Setup: tournament repo returns null (not found)
            _tournamentRepoMock.Setup(r => r.GetTournamentById(tournamentId)).Returns((Tournament)null);

            // Act + Assert
            Assert.Throws<Exception>(() => _tournamentHandler.FinishTournament(tournamentId));
        }

        [Test]
        public void CreateTournament_Should_Create_Tournament_With_Participants()
        {
            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var participantIds = new List<Guid> { user1Id, user2Id };

            // Act
            var tournament = _tournamentHandler.CreateTournament(participantIds);

            // Assert
            Assert.IsNotNull(tournament);
            Assert.AreEqual(2, tournament.ParticipantIds.Count);

            // Verify CreateTournament in repo was called
            _tournamentRepoMock.Verify(repo => repo.CreateTournament(It.IsAny<Tournament>()), Times.Once);

            // Verify AddParticipant called twice (once per user)
            _tournamentRepoMock.Verify(repo => repo.AddParticipant(tournament.Id, user1Id), Times.Once);
            _tournamentRepoMock.Verify(repo => repo.AddParticipant(tournament.Id, user2Id), Times.Once);
        }

        [Test]
        public void FinishTournament_Should_Handle_No_Exercises_Safely()
        {
            // Arrange
            var tournamentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var tournament = new Tournament(new List<Guid> { userId }) { Id = tournamentId };

            _tournamentRepoMock.Setup(r => r.GetTournamentById(tournamentId)).Returns(tournament);
            _tournamentRepoMock.Setup(r => r.GetParticipants(tournamentId)).Returns(new List<Guid> { userId });
            _exerciseRepoMock.Setup(r => r.GetExercisesByUserId(userId)).Returns(new List<Exercise>()); // No exercises

            var user = new User { Id = userId, Username = "User1", Elo = 100 };
            _userRepoMock.Setup(r => r.GetUserById(userId)).Returns(user);

            // Act + Assert
            Assert.DoesNotThrow(() => _tournamentHandler.FinishTournament(tournamentId));
        }

        [Test]
        public void GetAllTournaments_Should_Return_All_Tournaments()
        {
            // Arrange
            var tournaments = new List<Tournament> { new Tournament(), new Tournament() };
            _tournamentRepoMock.Setup(r => r.GetAllTournaments()).Returns(tournaments);

            // Act
            var result = _tournamentHandler.GetAllTournaments();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
