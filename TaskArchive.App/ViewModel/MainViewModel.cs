using DevExpress.Mvvm;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TasksArchive.App.Model;
using TasksArchive.App.ViewModel;
using TasksArchive.App.Views;
using TasksArchive.Model;

namespace TasksArchive.ViewModel
{
    public class MainViewModel : BaseVM
    {
        private Tasks _selectedTask;
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

        

        //public string Name { get => SelectedTasks.Name; set { SelectedTasks.Name = value; RaisePropertyChanged(nameof(Name)); } }
        //public string Channel { get; set; }
        //public string Descrition { get => SelectedTasks.Descrition; set { SelectedTasks.Descrition = value; RaisePropertyChanged(nameof(Descrition)); } }
        //public string Tematic { get; set; }
        //public ObservableCollection<KeyWordItem> KeyWords { get; set; } = new ObservableCollection<KeyWordItem>();
        //public double Size { get; set; }
        //public DateTime PublishData { get; set; }
        //public string Comment { get; set; }
        //public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();


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
                            case '$':
                                if (DateTime.TryParse(SearchText.Remove(0, 1), out DateTime date))
                                    return Tasks.PublishData.Date == date.Date;
                                return false;

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
                    File.WriteAllText("TaskssData.json", JsonConvert.SerializeObject(Taskss));
                    Taskss.Add(vm.TasksInfo);
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
