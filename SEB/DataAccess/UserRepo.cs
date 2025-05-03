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
                return new User(id, userName, pass, null, null, null, elo, new List<Exercise>(), token);
            }
            return null;
        }

        public User? GetUserById(Guid userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token, Name, Bio, Image
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
                var name = reader.IsDBNull(5) ? null : reader.GetString(5);
                var bio = reader.IsDBNull(6) ? null : reader.GetString(6);
                var image = reader.IsDBNull(7) ? null : reader.GetString(7);

                return new User(id, username, password, name, bio, image, elo, new List<Exercise>(), token);
            }

            return null;
        }

        public User? GetUserByUsername(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token, Name, Bio, Image
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
                var name = reader.IsDBNull(5) ? null : reader.GetString(5);
                var bio = reader.IsDBNull(6) ? null : reader.GetString(6);
                var image = reader.IsDBNull(7) ? null : reader.GetString(7);

                return new User(id, userName, password, name, bio, image, elo, new List<Exercise>(), token);
            }

            return null;
        }


        public User? GetUserByToken(string token)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, Password, Elo, Token, Name, Bio, Image
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
                var name = reader.IsDBNull(5) ? null : reader.GetString(5);
                var bio = reader.IsDBNull(6) ? null : reader.GetString(6);
                var image = reader.IsDBNull(7) ? null : reader.GetString(7);


                return new User(id, username, password, name, bio, image, elo, new List<Exercise>(), userToken);
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Password, Elo, Token, Name, Bio, Image FROM Users;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Elo = reader.GetInt32(3),
                    Token = reader.GetString(4),
                    Name = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Bio = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Image = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Exercises = new List<Exercise>()
                });
            }

            return users;
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
                SET Username = @Username,
                    Password = @Password,
                    Elo = @Elo,
                    Token = @Token,
                    Name = @Name,
                    Bio = @Bio,
                    Image = @Image
                WHERE Id = @Id;
            ";

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Elo", user.Elo);
            command.Parameters.AddWithValue("@Token", user.Token);
            command.Parameters.AddWithValue("@Name", user.Name ?? string.Empty);
            command.Parameters.AddWithValue("@Bio", user.Bio ?? string.Empty);
            command.Parameters.AddWithValue("@Image", user.Image ?? string.Empty);

            command.ExecuteNonQuery();

        }

    }
}
