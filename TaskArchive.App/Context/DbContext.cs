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

        public static DbContext GetInstance()
        {
            return Instance.Value;
        }

        private DbContext()
        {
            _userContext = UserContext.GetInstance();

            Conn = new MySqlConnection(_connectionString);
            Taskss = new ObservableCollection<Tasks>();

            GetTasks();
        }

        public void UpdateUserImage(string image, string user)
        {
            try
            {
                Conn.Open();
                var command = Conn.CreateCommand();
                command.CommandText = "UPDATE USERS SET Image = @ImagePath WHERE ID = @UserID";
                command.Parameters.AddWithValue("@ImagePath", image);
                command.Parameters.AddWithValue("@UserID", user);
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
        }



        #region TaskWork

        public void AddTask(Tasks task)
        {
            //проверочку сделать
            Taskss.Add(task);
            try
            {
                Conn.Open();
                var command = Conn.CreateCommand();
                //написать запросы к бд, чтобы добавляло
                command.CommandText = "";
                //command.Parameters.AddWithValue("@UserID", AuthUser.Id);
                command.Parameters.AddWithValue("@TaskName", task.Name);
                command.Parameters.AddWithValue("@TaskDesc", task.Descrition);
                //и другое
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
