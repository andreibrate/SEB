using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace SEB.DataAccess
{
    public class DB_Manager
    {
        public static void InitializeDatabase(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Users (
                                Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                Username VARCHAR(50) NOT NULL UNIQUE,
                                Password VARCHAR(255) NOT NULL,
                                Elo INT NOT NULL DEFAULT 100,
                                Token VARCHAR(100)
                            );

                            CREATE TABLE IF NOT EXISTS Exercises (
                                Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                                Count INT NOT NULL,
                                Duration INTERVAL NOT NULL,
                                Timestamp TIMESTAMPTZ DEFAULT NOW()
                            );

                            CREATE TABLE IF NOT EXISTS Tournaments (
                                Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                StartTime TIMESTAMPTZ DEFAULT NOW(),
                                WinnerId UUID REFERENCES Users(Id),
                                IsDraw BOOLEAN DEFAULT FALSE
                            );

                            CREATE TABLE IF NOT EXISTS TournamentParticipants (
                                TournamentId UUID NOT NULL REFERENCES Tournaments(Id) ON DELETE CASCADE,
                                UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                                ExerciseId UUID NOT NULL REFERENCES Exercises(Id) ON DELETE CASCADE,
                                EloChange INT NOT NULL,
                                PRIMARY KEY (TournamentId, UserId)
                            );

                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Unable to initialize database. Reason: {ex.Message}");
            }
        }

        public static void CleanupTables(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            DO $$
                            BEGIN
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'tournamentparticipants') THEN
                                    EXECUTE 'TRUNCATE TABLE TournamentParticipants RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'tournaments') THEN
                                    EXECUTE 'TRUNCATE TABLE Tournaments RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'exercises') THEN
                                    EXECUTE 'TRUNCATE TABLE Exercises RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'users') THEN
                                    EXECUTE 'TRUNCATE TABLE Users RESTART IDENTITY CASCADE';
                                END IF;
                            END $$;
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Unable to clean up database tables. Reason: {ex.Message}");
            }
        }
    }
}
