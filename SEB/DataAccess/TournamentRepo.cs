using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.DataAccess.Interfaces;
using SEB.Models;
using Npgsql;
using SEB.Models.Enums;

namespace SEB.DataAccess
{
    public class TournamentRepo : ITournamentRepo
    {
        private readonly string _connectionString;
        public TournamentRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTournament(Tournament tournament)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Tournaments (Id, StartTime, Status, IsDraw)
                VALUES (@Id, @StartTime, @Status, @IsDraw);
            ";

            command.Parameters.AddWithValue("@Id", tournament.Id);
            command.Parameters.AddWithValue("@StartTime", tournament.StartTime);
            command.Parameters.AddWithValue("@Status", tournament.Status.ToString());
            command.Parameters.AddWithValue("@IsDraw", tournament.IsDraw);

            command.ExecuteNonQuery();
        }

        public Tournament? GetTournamentById(Guid tournamentId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, StartTime, Status, IsDraw
                FROM Tournaments
                WHERE Id = @Id;
            ";
            command.Parameters.AddWithValue("@Id", tournamentId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var tournament = new Tournament
                {
                    Id = reader.GetGuid(0),
                    StartTime = reader.GetDateTime(1),
                    Status = Enum.Parse<TournamentStatus>(reader.GetString(2)),
                    IsDraw = reader.GetBoolean(3)
                };

                return tournament;
            }

            return null;
        }

        public List<Tournament> GetAllTournaments()
        {
            var tournaments = new List<Tournament>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, StartTime, Status, IsDraw
                FROM Tournaments;
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var tournament = new Tournament
                {
                    Id = reader.GetGuid(0),
                    StartTime = reader.GetDateTime(1),
                    Status = Enum.Parse<TournamentStatus>(reader.GetString(2)),
                    IsDraw = reader.GetBoolean(3)
                };

                tournaments.Add(tournament);
            }

            return tournaments;
        }

        public void AddParticipant(Guid tournamentId, Guid userId, Guid exerciseId, int eloChange = 0)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TournamentParticipants (TournamentId, UserId, ExerciseId, EloChange)
                VALUES (@TournamentId, @UserId, @ExerciseId, @EloChange);
            ";

            command.Parameters.AddWithValue("@TournamentId", tournamentId);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ExerciseId", exerciseId);
            command.Parameters.AddWithValue("@EloChange", eloChange);

            command.ExecuteNonQuery();
        }


        public List<Guid> GetParticipants(Guid tournamentId)
        {
            var participants = new List<Guid>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT UserId
                FROM TournamentParticipants
                WHERE TournamentId = @TournamentId;
            ";
            command.Parameters.AddWithValue("@TournamentId", tournamentId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                participants.Add(reader.GetGuid(0));
            }

            return participants;
        }

        public void SetWinners(Guid tournamentId, List<Guid> winnerIds, bool isDraw)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // update tournament table
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Tournaments
                SET Status = 'Ended',
                    IsDraw = @IsDraw
                WHERE Id = @TournamentId;
            ";
            command.Parameters.AddWithValue("@IsDraw", isDraw);
            command.Parameters.AddWithValue("@TournamentId", tournamentId);
            command.ExecuteNonQuery();
        }

        public void UpdateStatus(Guid tournamentId, TournamentStatus status)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Tournaments
                SET Status = @Status
                WHERE Id = @TournamentId;
            ";
            command.Parameters.AddWithValue("@Status", status.ToString());
            command.Parameters.AddWithValue("@TournamentId", tournamentId);

            command.ExecuteNonQuery();
        }

    }
}
