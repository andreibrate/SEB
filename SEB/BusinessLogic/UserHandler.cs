using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.DataAccess.Interfaces;
using SEB.Models;

namespace SEB.BusinessLogic
{
    public class UserHandler
    {
        private readonly IUserRepo _userRepo;

        public UserHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public void Register(User user)
        {
            _userRepo.RegisterUser(user);
        }

        public User Login(string username, string password)
        {
            var user = _userRepo.LoginUser(username, password);
            if (user == null)
            {
                throw new Exception("Invalid username or password.");
            }
            return user;
        }

        public User GetUserById(Guid id)
        {
            var user = _userRepo.GetUserById(id);
            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }
            return user;
        }

        public User GetUserByUsername(string username)
        {
            var user = _userRepo.GetUserByUsername(username);
            if (user == null)
            {
                throw new Exception($"User with username {username} not found.");
            }
            return user;
        }

        public User GetUserByToken(string token)
        {
            var user = _userRepo.GetUserByToken(token);
            if (user == null)
            {
                throw new Exception("Invalid token.");
            }
            return user;
        }

        public List<User> GetAllUsers()
        {
            return _userRepo.GetAllUsers();
        }

        public void UpdateUser(User user)
        {
            _userRepo.UpdateUser(user);
        }

        public void UpdateUserElo(Guid userId, int elo)
        {
            var user = _userRepo.GetUserById(userId);
            if (user != null)
            {
                user.Elo = elo;
                _userRepo.UpdateUser(user);
            }
        }

        public void UpdateUserBio(Guid userId, string bio)
        {
            var user = _userRepo.GetUserById(userId);
            if (user != null)
            {
                user.Bio = bio;
                _userRepo.UpdateUser(user);
            }
        }

        public void UpdateUserImage(Guid userId, string image)
        {
            var user = _userRepo.GetUserById(userId);
            if (user != null)
            {
                user.Image = image;
                _userRepo.UpdateUser(user);
            }
        }

        public void UpdateUserExercises(Guid userId, List<Exercise> exercises)
        {
            var user = _userRepo.GetUserById(userId);
            if (user != null)
            {
                user.Exercises = exercises;
                _userRepo.UpdateUser(user);
            }
        }

    }
}
