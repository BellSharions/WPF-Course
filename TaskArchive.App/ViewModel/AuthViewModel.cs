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
using TasksArchive.App;
using TasksArchive.Model;

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
                    command.CommandText = $"SELECT * FROM USERS WHERE ID = @Login";
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
                        var hasher = new Encryption();
                        var salt = result.GetString(result.GetOrdinal("salt"));
                        var hash = result.GetString(result.GetOrdinal("hash"));

                        var testHash = hasher.Encrypt(passwordBox.Password, salt);

                        if (testHash != hash)
                        {
                            MessageBox.Show("Доступ недоступен", "Error");
                            _dbContext.Conn.Close();
                            return;
                        }

                        UserContext.GetInstance().User = new User { Id = result.GetString(result.GetOrdinal("id")) };
                    }
                    _dbContext.Conn.Close();

                    _dbContext.Conn.Open();
                    var command2 = _dbContext.Conn.CreateCommand();
                    command2.CommandText = $"SELECT Taskss FROM TASKS WHERE ID = '{UserContext.GetInstance().User.Id}';";
                    var result2 = command2.ExecuteReaderAsync().Result;
                    while (result2.ReadAsync().Result)
                    {
                        UserContext.GetInstance().User.Taskss = null;
                    }
                    _dbContext.Conn.Close();
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
