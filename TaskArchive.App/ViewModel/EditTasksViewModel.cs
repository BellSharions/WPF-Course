using DevExpress.Mvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TasksArchive.App.Model;
using TasksArchive.Model;

namespace TasksArchive.App.ViewModel
{
    class EditTasksViewModel : BaseVM
    {
        //комманды для изменения содержания дел(ключевые слова)
        private Tasks _tasksInfo { get; set; }
        public Tasks TasksInfo { get => _tasksInfo; set { _tasksInfo = value; RaisePropertyChanged(nameof(TasksInfo)); } }

        public DelegateCommand AddKeyWord
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    TasksInfo.KeyWords.Add(new KeyWordItem(""));
                });
            }
        }

        public DelegateCommand<KeyWordItem> DeleteKeyWord
        {
            get
            {
                return new DelegateCommand<KeyWordItem>((keyword) =>
                {
                    if (keyword != null)
                    {
                        TasksInfo.KeyWords.Remove(keyword);
                    }
                });
            }
        }

        public DelegateCommand<string> StatusLogic
        {
            get
            {
                return new DelegateCommand<string>(obj =>
                {
                    TasksInfo.StatusText = obj;
                });
            }
        }
        public DelegateCommand<Window> Save
        {
            get
            {
                return new DelegateCommand<Window>((w) =>
                {
                    foreach (var key in TasksInfo.KeyWords)
                    {
                        if (DataBase.GetInstance().KeyWords.FirstOrDefault(s=> key.Value == s) == null)
                        {
                            DataBase.GetInstance().KeyWords.Add(key.Value);
                        }
                    }
                    w?.Close();
                });
            }
        }

        public ICommand AddImage
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var opd = new OpenFileDialog();
                    opd.Multiselect = true;
                    opd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                    if (opd.ShowDialog() == true)
                    {
                        foreach (var item in opd.FileNames)
                        {
                            TasksInfo.Images.Add(item);
                        }
                    }
                });
            }
        }

        public ICommand RemoveImage
        {
            get
            {
                return new DelegateCommand<string>((image) =>
                {
                    if (image != null)
                    {
                        TasksInfo.Images.Remove(image);
                    }
                });
            }
        }

        public ICommand UpdateTasksInfo
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var info = TasksInfo;
                    TasksInfo.Name = info.Name;
                    TasksInfo.Channel = info.Channel;
                    TasksInfo.Descrition = info.Descrition;
                    TasksInfo.Status = info.Status;
                });
            }
        }

    }
}
