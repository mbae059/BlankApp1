using BlankApp1.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class ProfileService : IProfileService
    {
        public Profile profile { get; private set; }

        public bool IsLoggedIn { get; private set; }

        public void Login(string userName)
        {
            profile = new Profile { UserName = userName };
            IsLoggedIn = true;
        }

        public void Logout()
        {
            profile = null;
            IsLoggedIn = false;
        }
    }
}
