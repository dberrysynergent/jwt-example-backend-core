using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthBackend.Models;

namespace UserAuthBackend.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        bool IsAuthenticated(User user);
        IEnumerable<User> GetAll();
    }
}
