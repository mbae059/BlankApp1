using BlankApp1.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface IProfileService
    {
        Profile profile { get; }
        void Login(string userName);
        void Logout();
        bool IsLoggedIn { get; }
    }
}
