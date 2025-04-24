using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // Implementation for registering a user
        }

        public User? LoginUser(string username, string password)
        {
            // Implementation for logging in a user
            return null;
        }

        public User? GetUserById(Guid userId)
        {
            // Implementation for getting a user by ID
            return null;
        }

        public User? GetUserByUsername(string username)
        {
            // Implementation for getting a user by username
            return null;
        }

        public User? GetUserByToken(string token)
        {
            // Implementation for getting a user by token
            return null;
        }

        public void UpdateUser(User user)
        {
            // Implementation for updating a user
        }

    }
}
