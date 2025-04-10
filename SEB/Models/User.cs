using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SEB.Models
{
    public class User
    {
        // standard constructor
        public User()
        {
            Username = string.Empty;
            Password = string.Empty;
            Elo = 100; // default Elo rating
            Exercises = new List<Exercise>();
        }

        // constructor for registration
        public User(string username, string password)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            Elo = 100; // default Elo rating
            Exercises = new List<Exercise>();
        }

        // constructor for loading from DB
        public User(Guid id, string username, string password, int elo, List<Exercise> exercises, string token)
        {
            Id = id;
            Username = username;
            Password = password;
            Elo = elo;
            Exercises = exercises;
            Token = token;
        }

        // --- PROPERTIES ---
        public Guid Id { get; set; }
        public string Username { get; set; } // unique
        public string Password { get; set; } // hashed
        public int Elo { get; set; }
        public List<Exercise> Exercises { get; set; } // history
        public string? Token { get; set; }  = string.Empty;
    }
}
