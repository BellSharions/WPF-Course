using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TaskArchive.App.Context.Roles
{
    class User
    {
        public enum Roles
        {
            User,
            Admin
        }

        public Roles Role { get; set; }

        public BitmapFrame Image;
        public string Name { get; set; }
        public string Id { get; set; }

        public User(Roles role)
        {
            Role = role;
        }

        public User()
        {
        }
    }
}
