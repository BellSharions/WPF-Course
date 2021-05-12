using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskArchive.App.Context.Roles
{
    class Admin : User
    {
        public Admin() : base(Roles.User)
        {
        }

        public Admin(User user)
        {
            Name = user.Name;
            Id = user.Id;
            Role = user.Role;
            Image = user.Image;
        }
    }
}
