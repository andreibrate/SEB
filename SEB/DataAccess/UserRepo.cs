using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SEB.DataAccess.Interfaces;
using SEB.Models;

namespace SEB.DataAccess
{
    public class UserRepo : IUserRepo
    {
        private readonly string _connectionString;
        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // tokenize username
        private string GenerateToken(string username)
        {
            return $"{username}-sebToken";
        }

        public void RegisterUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Id, Username, Password, Elo, Token)
                VALUES (@Id, @Username, @Password, 100, @Token);
            ";

            var token = GenerateToken(user.Username);
            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Token", token);

            command.ExecuteNonQuery();
        }

        public User? LoginUser(string username, string password)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token
                FROM Users
                WHERE Username = @username AND Password = @password;
            ";

            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetGuid(0);
                var userName = reader.GetString(1);
                var pass = reader.GetString(2);
                var elo = reader.GetInt32(3);
                var token = reader.GetString(4);
                return new User(id, userName, pass, null, null, elo, new List<Exercise>(), token);
            }
            return null;
        }

        public User? GetUserById(Guid userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token
                FROM Users
                WHERE Id = @id;
            ";
            command.Parameters.AddWithValue("id", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetGuid(0);
                var username = reader.GetString(1);
                var password = reader.GetString(2);
                var elo = reader.GetInt32(3);
                var token = reader.GetString(4);

                return new User(id, username, password, null, null, elo, new List<Exercise>(), token);
            }

            return null;
        }

        public User? GetUserByUsername(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token
                FROM Users
                WHERE Username = @username;
            ";
            command.Parameters.AddWithValue("username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetGuid(0);
                var userName = reader.GetString(1);
                var password = reader.GetString(2);
                var elo = reader.GetInt32(3);
                var token = reader.GetString(4);

                return new User(id, userName, password, null, null, elo, new List<Exercise>(), token);
            }

            return null;
        }


        public User? GetUserByToken(string token)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token
                FROM Users
                WHERE Token = @token;
            ";
            command.Parameters.AddWithValue("token", token);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetGuid(0);
                var username = reader.GetString(1);
                var password = reader.GetString(2);
                var elo = reader.GetInt32(3);
                var userToken = reader.GetString(4);

                return new User(id, username, password, null, null, elo, new List<Exercise>(), userToken);
            }

            return null;
        }


        public void UpdateUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            // make sure user has a token
            if (string.IsNullOrWhiteSpace(user.Token))
            {
                user.Token = GenerateToken(user.Username);
            }

            command.CommandText = @"
                UPDATE Users
                SET Username = @Username, Password = @Password, Elo = @Elo, Token = @Token
                WHERE Id = @Id;
            ";

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Elo", user.Elo);
            command.Parameters.AddWithValue("@Token", user.Token);

            command.ExecuteNonQuery();

        }

    }
}
