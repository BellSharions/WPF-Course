using DevExpress.Mvvm;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TaskArchive.App;
using TaskArchive.App.Context;
using TasksArchive.App.Model;
using TasksArchive.App.ViewModel;
using TasksArchive.App.Views;
using TasksArchive.Model;

namespace TasksArchive.ViewModel
{
    public class MainViewModel : BaseVM
    {
        private Tasks _selectedTask;
        private readonly DbContext _dbContext;
        public ObservableCollection<Tasks> Taskss { get; set; }
        public ICollectionView TaskssView { get; set; }
        public Page MainContent { get; set; }
        public Tasks SelectedTasks {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                RaisePropertyChanged(nameof(SelectedTasks));
            }
        }


        private string _SearchText { get; set; }
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;
                TaskssView.Filter = (obj) =>
                {
                    if (obj is Tasks Tasks)
                    {
                        switch (SearchText.FirstOrDefault())
                        {
                            case '@': return Tasks.KeyWords.FirstOrDefault(s => s.Value.ToLower().Contains(SearchText.Remove(0, 1).ToLower())) != null;
                            case '#': return Tasks.Tematic?.ToLower().Contains(SearchText.Remove(0, 1).ToLower()) == true;
                            //case '!': return Tasks.Channel?.ToLower().Contains(SearchText.Remove(0, 1).ToLower()) == true;

                            default: return Tasks.Name.ToLower().Contains(SearchText.ToLower());
                        }
                    }

                    return false;
                };
                TaskssView.Refresh();

            }
        }
        public MainViewModel()
        {
            _dbContext = DbContext.GetInstance();
            OverlayService.GetInstance().Show = (str) =>
            {
                OverlayService.GetInstance().Text = str;
            };


            Taskss = File.Exists("TaskssData.json") ? JsonConvert.DeserializeObject<ObservableCollection<Tasks>>(File.ReadAllText("TaskssData.json")) : new ObservableCollection<Tasks>();
            Taskss.CollectionChanged += (s, e) =>
            {
                File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));
            };
            BindingOperations.EnableCollectionSynchronization(Taskss, new object());
            TaskssView = CollectionViewSource.GetDefaultView(Taskss);

        }
        public ICommand Import
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Taskss = File.Exists("TaskssData.json") ? JsonConvert.DeserializeObject<ObservableCollection<Tasks>>(File.ReadAllText("TaskssData.json")) : new ObservableCollection<Tasks>();
                    
                    _dbContext.Conn.Open();
                    var command2 = _dbContext.Conn.CreateCommand();
                    command2.CommandText = "SELECT description FROM tasks where userID = @ID";
                    command2.Parameters.AddWithValue("@ID", UserContext.GetInstance().User.Id);
                    var result = command2.ExecuteReaderAsync().Result;
                    result.ReadAsync();
                        if (File.Exists("TaskssData.json"))
                            File.WriteAllText("TaskssData.json", result.GetString(0));
                        Taskss = File.Exists("TaskssData.json") ? JsonConvert.DeserializeObject<ObservableCollection<Tasks>>(File.ReadAllText("TaskssData.json")) : new ObservableCollection<Tasks>();
                        _dbContext.Conn.Close();
                    _dbContext.Conn.Open();
                    var command3 = _dbContext.Conn.CreateCommand();
                    command3.CommandText = "UPDATE datainformation SET ImportDate = CURDATE() WHERE userID = @UserID";
                    command3.Parameters.AddWithValue("@UserID", UserContext.GetInstance().User.Id);
                    command3.ExecuteNonQueryAsync();
                    _dbContext.Conn.Close();
                    TaskssView.Refresh();
                });
            }
        }

        public ICommand Export
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));
                    _dbContext.Conn.Open();
                    var command2 = _dbContext.Conn.CreateCommand();
                    command2.CommandText = "UPDATE tasks SET Description = @desc where userID = @ID";
                    command2.Parameters.AddWithValue("@desc", File.ReadAllText("TaskssData.json"));
                    command2.Parameters.AddWithValue("@ID", UserContext.GetInstance().User.Id);
                    command2.ExecuteNonQueryAsync();
                    _dbContext.Conn.Close();

                    TaskssView = CollectionViewSource.GetDefaultView(Taskss);
                    TaskssView.Refresh();
                });
            }
        }
        public ICommand Sort
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    

                    if (TaskssView.SortDescriptions.Count > 0)
                    {
                        TaskssView.SortDescriptions.Clear();
                    }
                    else
                    {
                        TaskssView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }
                });
            }
        }
        public ICommand DeleteTasks
        {
            get
            {
                return new DelegateCommand<Tasks>((Tasks) =>
                {
                    Taskss.Remove(Tasks);

                    File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));
                    SelectedTasks = Taskss.FirstOrDefault();

                }, (Tasks)=> Tasks != null);
            }
        }
        public ICommand AddItem
        {
            get
            {
                return new DelegateCommand<Tasks>(async =>
                {
                    var w = new EditTasksWindow();
                    var vm = new EditTasksViewModel
                    {
                        TasksInfo = new Tasks(),
                    };
                    w.DataContext = vm;
                    w.ShowDialog(); 
                    if (vm.TasksInfo.Name == null && vm.TasksInfo.Descrition == null && vm.TasksInfo.Channel == null)
                    {
                        MessageBox.Show("Введите данные");
                        return;
                    }
                    Taskss.Add(vm.TasksInfo);
                    File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));

                    TaskssView = CollectionViewSource.GetDefaultView(Taskss);
                    TaskssView.Refresh();
                });
            }
        }
        public ICommand TematicClick
        {
            get
            {
                return new DelegateCommand<string>((tematic) =>
                {
                    if (tematic != null)
                    {
                        SearchText = "#" + tematic;
                    }
                });
            }
        }
        public ICommand GoToUrl
        {
            get
            {
                return new DelegateCommand<string>((url) =>
                {
                    if (new Uri(url).IsFile)
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + url));
                    }
                    else
                    {
                        Process.Start(url);
                    }

                    
                });
            }
        }
        public ICommand EditTasks
        {
            get
            {
                return new DelegateCommand<Tasks>((Tasks) =>
                {
                    var w = new EditTasksWindow();
                    var vm = new EditTasksViewModel
                    {
                        TasksInfo = Tasks,
                    };
                    w.DataContext = vm;
                    w.ShowDialog();
                    File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));

                }, (Tasks) => Tasks != null);
            }
        }
        public ICommand OpenImage
        {
            get
            {
                return new DelegateCommand<string>((image) =>
                {
                    if (image != null)
                    {
                        var iv = new ImageViewer()
                        {
                            DataContext = new ImageViewerViewModel
                            {
                                Image = image
                            }
                        };
                        iv.ShowDialog();
                    }
                });
            }
        }
        public ICommand KeyWordClick
        {
            get
            {
                return new DelegateCommand<KeyWordItem>((word) =>
                {
                    if (word != null)
                    {
                        SearchText = "@" + word.Value;
                    }
                });
            }
        }
        public ICommand ChannelClick
        {
            get
            {
                return new DelegateCommand<string>((channel) =>
                {
                    if (channel != null)
                    {
                        SearchText = "!" + channel;
                    }
                });
            }
        }
        public ICommand DataClick
        {
            get
            {
                return new DelegateCommand<DateTime>((date) =>
                {
                    SearchText = "$" + date.Date.ToShortDateString();

                });
            }
        }
    }
}
