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
using TaskArchive.App.Views;
using TasksArchive.Model;
using static TaskArchive.App.Context.Roles.User;

namespace TaskArchive.App.ViewModel
{
    class RegisterViewModel : BaseVM
    {
        private readonly DbContext _dbContext;

        private int _userID;
        public int UserID
        {
            get => _userID;
            set
            {
                _userID = value;
                RaisePropertyChanged(nameof(UserID));
            }
        }
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged(nameof(UserName));
            }
        }
        private string _passWord;
        public string PassWord
        {
            get => _passWord;
            set
            {
                _passWord = value;
                RaisePropertyChanged(nameof(PassWord));
            }
        }

        private Roles _role;
        public Roles Role
        {
            get => _role;
            set
            {
                _role = Roles.User;
                RaisePropertyChanged(nameof(Role));
            }
        }

        public RegisterViewModel()
        {
            _dbContext = DbContext.GetInstance();
        }

        public ICommand RegisterCommand
        {

            get
            {
                return new DelegateCommand<PasswordBox>(obj =>
                {
                    if (!(obj is PasswordBox passwordBox) || string.IsNullOrEmpty(passwordBox?.Password) || string.IsNullOrEmpty(UserName))
                    {
                        MessageBox.Show("Проверьте введенные данные", "Error");
                        return;
                    }
                    try
                    {
                        _dbContext.Conn.Open();
                        var command = _dbContext.Conn.CreateCommand();
                        command.CommandText = "SELECT Count(*) FROM USERS";
                        var result = command.ExecuteScalarAsync().Result;
                        _dbContext.Conn.Close();
                        PassWord = obj.Password;
                        UserID = 1;
                        _dbContext.Conn.Open();
                        var command2 = _dbContext.Conn.CreateCommand();
                        command2.CommandText = $"INSERT INTO users (userID, username, password, role) values (@userID, @username, @passwoed, @role)";
                        command2.Parameters.AddWithValue("@userID", UserID);
                        command2.Parameters.AddWithValue("@username", UserName);
                        command2.Parameters.AddWithValue("@password", PassWord);
                        command2.Parameters.AddWithValue("@role", Role.ToString());
                        command2.ExecuteNonQueryAsync();
                        _dbContext.Conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        _dbContext.Conn.Close();
                        return;
                    }
                    var mainWindow = new AuthWindow();
                    mainWindow.Show();
                    Application.Current.MainWindow?.Close();
                });
            }
        }
    }
}
