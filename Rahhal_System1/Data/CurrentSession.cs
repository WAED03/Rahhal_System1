using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Data
{
    public static class CurrentSession
    {
        public static User LoggedInUser { get; private set; }

        public static void SetUser(User user)
        {
            LoggedInUser = user;
        }

        public static void Logout()
        {
            LoggedInUser = null;
        }

        public static bool IsAdmin()
        {
            return LoggedInUser != null && LoggedInUser.Role == "Admin";
        }

        public static bool IsLoggedIn()
        {
            return LoggedInUser != null;
        }
    }
}
