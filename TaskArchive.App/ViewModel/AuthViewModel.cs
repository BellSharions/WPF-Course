using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
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
                        UserContext.GetInstance().User = new User {
                            Id = result.GetString(result.GetOrdinal("userID")),
                            Name = result.GetString(result.GetOrdinal("username"))
                        };
                        switch (result.GetString(result.GetOrdinal("role")))
                        {
                            case "user":
                                UserContext.GetInstance().User.Role = Roles.User;
                                break;
                            case "admin":
                                UserContext.GetInstance().User.Role = Roles.Admin;
                                break;
                        }
                    }
                    _dbContext.Conn.Close();

                    //_dbContext.Conn.Open();
                    //var command2 = _dbContext.Conn.CreateCommand();
                    //command2.CommandText = $"SELECT Description FROM TASKS WHERE ID = '{UserContext.GetInstance().User.Id}';";
                    //var result2 = command2.ExecuteReaderAsync().Result;
                    //while (result2.ReadAsync().Result)
                    //{
                    //    UserContext.GetInstance().User.Taskss = null;
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
                mainWindow.Show();
                Application.Current.MainWindow?.Close();
            });
            }
        } } }
