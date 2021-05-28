using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class AdminViewModel : BaseVM
    {
        private readonly DbContext _dbContext;
        private readonly ComboBox _groupsComboBox;
        public ObservableCollection<User> Users;
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

        public List<string> Role { get; }

        private string _selectedRole;
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                RaisePropertyChanged(nameof(SelectedRole));
            }
        }
        public AdminViewModel()
        {
            _dbContext = DbContext.GetInstance();

            Role = new List<string>()
            {
                User.Roles.Admin.ToString(),
                User.Roles.User.ToString()
            };
            SelectedRole = Role[0];
            UserName = "Имя";
            UserID = 0;
            PassWord = "Пароль";
            Users = new ObservableCollection<User>();
        }

        public AdminViewModel(ComboBox groupsComboBox)
        {
            _dbContext = DbContext.GetInstance();

            _groupsComboBox = groupsComboBox;

            Role = new List<string>()
            {
                User.Roles.Admin.ToString(),
                User.Roles.User.ToString()
            };
            SelectedRole = Role[0];
            UserName = "Имя";
            UserID = 0;
            PassWord = "Пароль";
            Users = new ObservableCollection<User>();
        }
        public ICommand StartApp
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                });
            }
        }
        public ICommand CommitCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _dbContext.AddUser(new User()
                    {
                        Id = UserID.ToString(),
                        Name = UserName,
                        Role = User.Roles.User,
                        PassWord = PassWord
                    });
                });
            }
        }
        public ICommand DeleteUserCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    try
                    {
                        _dbContext.Conn.Open();
                        var command = _dbContext.Conn.CreateCommand();
                        command.CommandText = $"DELETE FROM users WHERE userID = @userID";
                        command.Parameters.AddWithValue("@userID", UserID);
                        command.ExecuteNonQueryAsync();
                        _dbContext.Conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        _dbContext.Conn.Close();
                        return;
                    }
                });
            }
        }
        public string this[string columnName]
        {
            get
            {
                var error = string.Empty;
                switch (columnName)
                {
                    case nameof(UserName) when UserName == null: return error;

                    case nameof(UserName):
                        if (!Regex.IsMatch(UserName, @"^[А-Я\x20][а-яА-Я\x20]*$"))
                            return "Неверное имя";
                        break;
                }

                return error;
            }
        }

        public string Error { get; }
    }
}
