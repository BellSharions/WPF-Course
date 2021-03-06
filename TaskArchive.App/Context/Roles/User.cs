using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TasksArchive.Model;

namespace TaskArchive.App.Context.Roles
{
    public class User
    {
        public enum Roles
        {
            User,
            Admin
        }

        public Roles Role { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string PassWord { get; set; }

        public ObservableCollection<Tasks> Taskss;

        public User(Roles role)
        {
            Role = role;
        }

        public User()
        {
        }
    }
}
