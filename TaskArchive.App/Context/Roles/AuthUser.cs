using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskArchive.App.Context.Roles;

namespace TaskArchive.App.Context.Roles
{
    class AuthUser : User
    {
        private readonly DbContext _dbContext;

        public AuthUser() : base(Roles.User)
        {
        }

        public AuthUser(User user) : base(Roles.User)
        {
            Name = user.Name;
            Id = user.Id;
            Image = user.Image;
            _dbContext = DbContext.GetInstance();
        }
        private string GetTasksNum()
        {
            try
            {
                string output = null;
                _dbContext.Conn.Open();
                var command = _dbContext.Conn.CreateCommand();
                command.CommandText = $"SELECT count(*) FROM USERS WHERE userID = '{Id}';";
                var result = command.ExecuteReaderAsync().Result;
                while (result.ReadAsync().Result)
                {
                    output = result.GetValue(0).ToString();
                }
                _dbContext.Conn.Close();
                return output;
            }
            catch (Exception ex)
            {
                _dbContext.Conn.Close();
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                if (ex.InnerException?.Message != null)
                    message.AppendLine(ex.InnerException.Message);
                MessageBox.Show(message.ToString(), "Error");
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
