using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SEB.Models.Enums;

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
        public User(Guid id, string username, string password, string? name, string? bio, string? image, int elo, List<Exercise> exercises, string token)
        {
            Id = id;
            Username = username;
            Password = password;
            Name = name ?? string.Empty;    // default name
            Bio = bio ?? string.Empty;      // default bio
            Image = image ?? string.Empty;  // default image
            Elo = elo;
            Exercises = exercises;
            Token = token;
        }

        // --- PROPERTIES ---
        public Guid Id { get; set; }
        public string Username { get; set; } // unique
        public string Password { get; set; }
        public string? Name { get; set; }   // can be missing (null)
        public string? Bio { get; set; }    // can be missing (null)
        public string? Image { get; set; }  // can be missing (null)
        public int Elo { get; set; }
        public List<Exercise> Exercises { get; set; } // history
        public string? Token { get; set; }  = string.Empty;
        public Rank Rank => CalculateRank(); // always derived from Elo

        private Rank CalculateRank()
        {
            if (Elo < 100)
                return Rank.Loser;
            else if (Elo == 100)
                return Rank.Newbie;
            else if (Elo <= 105)
                return Rank.Advanced;
            else
                return Rank.Master;
        }

    }
}
