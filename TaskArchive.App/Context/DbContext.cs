using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TasksArchive.Model;
using System.Configuration;
using TaskArchive.App.Context.Roles;
using Newtonsoft.Json;
using System.IO;

namespace TaskArchive.App.Context
{
    public class DbContext : IDisposable
    {
        private static readonly Lazy<DbContext> lazy = new Lazy<DbContext>(() => new DbContext());
        private static readonly Lazy<DbContext> Instance = lazy;

        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private readonly UserContext _userContext;

        public readonly MySqlConnection Conn;

        public ObservableCollection<Tasks> Taskss;
        private ObservableCollection<User> Users;


        public static DbContext GetInstance()
        {
            return Instance.Value;
        }

        private DbContext()
        {
            _userContext = UserContext.GetInstance();

            Conn = new MySqlConnection(_connectionString);
            Taskss = new ObservableCollection<Tasks>();
            Users = new ObservableCollection<User>();

            GetTasks();
            GetUsers();
        }

        public void AddUser(User user)
        {
            if (Users.Any(p => p.Id == user.Id))
            {
                MessageBox.Show("Пользователь с таким ID уже  существует");
                return;
            }
            Users.Add(user);
            try
            {
                Conn.Open();
                var command = Conn.CreateCommand();
                command.CommandText = $"INSERT INTO USERS (userID, username, password) values (@Id, @UserName, @PassWord)";
                command.Parameters.AddWithValue("@UserName", user.Name);
                command.Parameters.AddWithValue("@PassWord", user.PassWord);
                command.Parameters.AddWithValue("@Id", user.Id);
                command.ExecuteNonQuery();
                Conn.Close();
                if (user.Role != User.Roles.User) return;
                Conn.Open();
                var command2 = Conn.CreateCommand();
                command2.CommandText = "INSERT INTO ROLES (userID, Role) values (@UserID, @Role)";
                command2.Parameters.AddWithValue("@UserID", user.Id);
                command2.Parameters.AddWithValue("@Role", user.Role.ToString());
                command2.ExecuteNonQuery();
                Conn.Close();
                MessageBox.Show("Добавлен", "Success");
            }
            catch (Exception ex)
            {
                Conn.Close();
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                if (ex.InnerException?.Message != null)
                    message.AppendLine(ex.InnerException.Message);
                MessageBox.Show(message.ToString(), "Error");
            }
        }

        private void GetUsers()
        {
            try
            {
                Conn.Open();
                var command = Conn.CreateCommand();
                command.CommandText = "SELECT * FROM USERS";
                var result = command.ExecuteReaderAsync().Result;
                while (result.ReadAsync().Result)
                {
                    Users.Add(new User()
                    {
                        Id = result.GetString(result.GetOrdinal("userID")),
                        Name = result.GetString(result.GetOrdinal("username"))
                    });
                }
                Conn.Close();
            }
            catch (Exception ex)
            {
                Conn.Close();
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                if (ex.InnerException?.Message != null)
                    message.AppendLine(ex.InnerException.Message);
                MessageBox.Show(message.ToString(), "Error");
            }
        }
        public void DeleteUser(User user)
        {
            foreach (var i in Users)
                if (i.Id == user.Id)
                {
                    Users.Remove(i);
                    try
                    {
                        Conn.Open();
                        var command = Conn.CreateCommand();
                        command.CommandText = $"call `DeleteUser` ('{user.Id}')";
                        var cols = command.ExecuteNonQueryAsync().Result;
                        Conn.Close();
                        if (cols != 0)
                            MessageBox.Show("Удалено", "Success");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Conn.Close();
                        var message = new StringBuilder();
                        message.AppendLine(ex.Message);
                        if (ex.InnerException?.Message != null)
                            message.AppendLine(ex.InnerException.Message);
                        MessageBox.Show(message.ToString(), "Error");
                    }

                    return;
                }
        }




        #region TaskWork

        public void AddTask(Tasks task)
        {
            Taskss.Add(task);
            try
            {
                Conn.Open();
                var command = Conn.CreateCommand();
                command.CommandText = "INSERT INTO tasks(userID, Name, Description, Status, Tematic) values (@UserID, @TaskName, @TaskDesc, @Status, @Tematic)";
                command.Parameters.AddWithValue("@UserID", UserContext.GetInstance().User.Id);
                command.Parameters.AddWithValue("@TaskName", task.Name);
                command.Parameters.AddWithValue("@TaskDesc", JsonConvert.SerializeObject(File.ReadAllText("TaskssData.json")));
                command.Parameters.AddWithValue("@Status", task.Status);
                command.Parameters.AddWithValue("@Tematic", task.Tematic);
                command.ExecuteNonQueryAsync();
                Conn.Close();
                Conn.Open();
                var command2 = Conn.CreateCommand();
                command2.CommandText = "INSERT INTO datainformation(userID, ExportDate) values (@UserID, CURDATE())";
                command2.Parameters.AddWithValue("@UserID", UserContext.GetInstance().User.Id);
                command2.ExecuteNonQueryAsync();
                Conn.Close();
            }
            catch (Exception ex)
            {
                Conn.Close();
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                if (ex.InnerException?.Message != null)
                    message.AppendLine(ex.InnerException.Message);
                MessageBox.Show(message.ToString(), "Error");
            }
        }

        public void DeleteTask(Tasks task)
        {
            foreach (var i in Taskss)
                if (i.Name == task.Name && i.Descrition == task.Descrition)
                {
                    Taskss.Remove(i);
                    try
                    {
                        Conn.Open();
                        var command = Conn.CreateCommand();
                        command.CommandText = "DELETE FROM TASKS WHERE сравнить с полями";
                        command.Parameters.AddWithValue("@TaskName", task.Name);
                        command.Parameters.AddWithValue("@TaskDesc", task.Descrition);
                        command.ExecuteNonQueryAsync();
                        Conn.Close();
                    }
                    catch (Exception ex)
                    {
                        Conn.Close();
                        var message = new StringBuilder();
                        message.AppendLine(ex.Message);
                        if (ex.InnerException?.Message != null)
                            message.AppendLine(ex.InnerException.Message);
                        MessageBox.Show(message.ToString(), "Error");
                    }
                    return;
                }
        }

        private void GetTasks()
        {
            try
            {
                Taskss.Clear();
                Conn.Open();
                var command = Conn.CreateCommand();
                command.CommandText = "SELECT * FROM TASKS";
                var result = command.ExecuteReaderAsync().Result;
                while (result.ReadAsync().Result)
                {
                    Taskss.Add(new Tasks()
                    {
                        Name = result.GetString(result.GetOrdinal("Name")),
                        Descrition = result.GetString(result.GetOrdinal("Description"))
                        //сделать для остальных надо тоже
                    });
                }
                Conn.Close();
            }
            catch (Exception ex)
            {
                Conn.Close();
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                if (ex.InnerException?.Message != null)
                    message.AppendLine(ex.InnerException.Message);
                MessageBox.Show(message.ToString(), "Error");
            }
        }

        #endregion TaskWork


        public void Dispose()
        {
            Conn?.Close();
        }
    }
}
