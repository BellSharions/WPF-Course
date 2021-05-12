using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskArchive.App.Context.Roles;

namespace TaskArchive.App
{
    class UserContext
    {
        private static readonly Lazy<UserContext> Instance = new Lazy<UserContext>(() => new UserContext());

        public User User;

        public static UserContext GetInstance()
        {
            return Instance.Value;
        }

        private UserContext()
        {
            User = new User();
        }
    }
}
