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
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        Username VARCHAR(50) NOT NULL UNIQUE,
                        Password VARCHAR(255) NOT NULL,
                        Elo INT NOT NULL DEFAULT 100,
                        Token VARCHAR(100),
                        Name VARCHAR(100),
                        Bio TEXT,
                        Image VARCHAR(8)
                    );

                    CREATE TABLE IF NOT EXISTS Exercises (
                        Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                        Type VARCHAR NOT NULL,
                        Count INT NOT NULL,
                        Duration INTERVAL NOT NULL,
                        Timestamp TIMESTAMPTZ DEFAULT NOW()
                    );

                    CREATE TABLE IF NOT EXISTS Tournaments (
                        Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        StartTime TIMESTAMPTZ DEFAULT NOW(),
                        IsActive BOOLEAN DEFAULT TRUE,
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
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Unable to initialize database. Reason: {ex.Message}");
            }
        }

        public static void CleanupTables(string connectionString)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                            DROP TABLE IF EXISTS TournamentParticipants;
                            DROP TABLE IF EXISTS Tournaments;
                            DROP TABLE IF EXISTS Exercises;
                            DROP TABLE IF EXISTS Users;
                        ";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Unable to clean up database tables. Reason: {ex.Message}");
            }
        }
    }
}

/* 
 * Create Docker container (first time):
 * docker run --name sebdb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres
 * 
 * Open new CMD to connect to DB:
 * docker exec -it sebdb bash
 * psql -h localhost -U postgres -p 5432
 * 
 * Create a new DB (otherwise it doesn't appear in pgAdmin):
 * CREATE DATABASE sebdb;
 * \c sebdb
 * 
 * Server explorer in Visual Studio -> "Connect to Database"
 */
