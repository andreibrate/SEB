using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.Models;

namespace SEB.DataAccess.Interfaces
{
    public interface IUserRepo
    {
        void RegisterUser(User user);
        User? LoginUser(string username, string password);
        User? GetUserById(Guid userId);
        User? GetUserByUsername(string username);
        User? GetUserByToken(string token);
        void UpdateUser(User user);
    }
}
