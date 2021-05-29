using DevExpress.Mvvm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskArchive.App.Context;
using TaskArchive.App.Context.Roles;
using TaskArchive.App.Model;
using TaskArchive.App.Views;
using TasksArchive.App;
using TasksArchive.Model;
using static TaskArchive.App.Context.Roles.User;

namespace TaskArchive.App.ViewModel
{
    class AuthViewModel : BaseVM
    {
        private string _login;
        private readonly DbContext _dbContext;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                RaisePropertyChanged(nameof(Login));
            }
        }

        public AuthViewModel()
        {
            _dbContext = DbContext.GetInstance();
        }
        public ICommand RegisterCommandWindow
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var w = new RegisterWindow();
                    var vm = new RegisterViewModel
                    {};
                    w.DataContext = vm;
                    foreach(var i in Application.Current.Windows)
                        if (i.GetType() == typeof(AuthWindow))
                        {
                            var tmp = (Window)i;
                            tmp.Close();
                            break;
                        }
                    w.ShowDialog();
                });
            }
        }
        public ICommand AuthCommand {

            get
            {
                return new DelegateCommand<PasswordBox>(obj =>
            {
                if (!(obj is PasswordBox passwordBox) || string.IsNullOrEmpty(passwordBox?.Password) || string.IsNullOrEmpty(Login))
                {
                    MessageBox.Show("Проверьте введенные данные", "Error");
                    return;
                }
                try
                {
                    _dbContext.Conn.Open();
                    var command = _dbContext.Conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM USERS WHERE username = @Login";
                    command.Parameters.AddWithValue("@Login", Login);
                    var result = command.ExecuteReaderAsync().Result;
                    if (!result.HasRows)
                    {
                        MessageBox.Show("Пользователь не найден", "Error");
                        _dbContext.Conn.Close();
                        return;
                    }
                    while (result.ReadAsync().Result)
                    {
                        if (passwordBox.Password != result.GetString(result.GetOrdinal("password")))
                            {
                            MessageBox.Show("Неправильный пароль", "Error");
                            _dbContext.Conn.Close();
                            return;
                        }
                        UserContext.GetInstance().User = new User {
                            Id = result.GetString(result.GetOrdinal("userID")),
                            Name = result.GetString(result.GetOrdinal("username"))
                        };
                    }
                    command.Connection.Close();
                    _dbContext.Conn.Close();
                    _dbContext.Conn.Open();
                    var command2 = _dbContext.Conn.CreateCommand();
                    command2.CommandText = $"SELECT * FROM ROLES WHERE userID = @ID";
                    command2.Parameters.AddWithValue("@ID", UserContext.GetInstance().User.Id);
                    var result2 = command2.ExecuteReaderAsync().Result;
                    result2.ReadAsync();
                    switch (result2.GetString(1))
                    {
                        case "User":
                            UserContext.GetInstance().User.Role = Roles.User;
                            break;
                        case "Admin":
                            UserContext.GetInstance().User.Role = Roles.Admin;
                            break;
                    }
                    _dbContext.Conn.Close();
                    //_dbContext.Conn.Open();
                    //var command3 = _dbContext.Conn.CreateCommand();
                    //command3.CommandText = "SELECT description FROM tasks where userID = @ID";
                    //command3.Parameters.AddWithValue("@ID", UserContext.GetInstance().User.Id);
                    //var result3 = command3.ExecuteReaderAsync().Result;
                    //while (result3.ReadAsync().Result)
                    //{
                    //    if (File.Exists("TaskssData.json"))
                    //        File.WriteAllText("TaskssData.json", result3.GetString(0));
                    //    UserContext.GetInstance().User.Taskss = File.Exists("TaskssData.json") ? JsonConvert.DeserializeObject<ObservableCollection<Tasks>>(File.ReadAllText("TaskssData.json")) : new ObservableCollection<Tasks>();
                    //}
                    //_dbContext.Conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _dbContext.Conn.Close();
                    return;
                }
                var mainWindow = new MainWindow();
                var adminWindow = new AdminWindow();
                if (UserContext.GetInstance().User.Role == Roles.Admin)
                {
                    adminWindow.Show();
                    Application.Current.MainWindow?.Close();
                }
                else
                {
                    mainWindow.Show();
                    Application.Current.MainWindow?.Close();
                }
            });
            }
        } } }
