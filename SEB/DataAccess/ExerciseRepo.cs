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
    public class ExerciseRepo : IExerciseRepo
    {
        private readonly string _connectionString;
        public ExerciseRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddExercise(Exercise exercise)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Exercises (Id, UserId, Type, Count, Duration)
                VALUES (@Id, @UserId, @Type, @Count, @Duration);
            ";

            command.Parameters.AddWithValue("@Id", exercise.Id == Guid.Empty ? Guid.NewGuid() : exercise.Id);
            command.Parameters.AddWithValue("@UserId", exercise.UserId);
            command.Parameters.AddWithValue("@Type", exercise.Type.ToString());
            command.Parameters.AddWithValue("@Count", exercise.Count);
            command.Parameters.AddWithValue("@Duration", exercise.Duration);

            command.ExecuteNonQuery();
        }
        public List<Exercise> GetExercisesByUserId(Guid userId)
        {
            var exercises = new List<Exercise>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, Type, Count, Duration
                FROM Exercises
                WHERE UserId = @UserId;
            ";

            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var exercise = new Exercise
                {
                    Id = reader.GetGuid(0),
                    UserId = reader.GetGuid(1),
                    Type = Enum.Parse<ExerciseTypes>(reader.GetString(2)),
                    Count = reader.GetInt32(3),
                    Duration = reader.GetInt32(4)
                };
                exercises.Add(exercise);
            }
            return exercises;
        }
    }
}
