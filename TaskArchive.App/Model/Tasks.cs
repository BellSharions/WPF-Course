using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TasksArchive.App.Model;

namespace TasksArchive.Model
{
    public class Tasks
    {
        public string Name { get; set; }
        public string Channel { get; set; }
        public string Descrition { get; set; }
        public string Tematic { get; set; }
        public ObservableCollection<KeyWordItem> KeyWords { get; set; } = new ObservableCollection<KeyWordItem>();
        public int Status { get; set; }
        public string StatusText { get; set; }
        public enum StatusCheck { Ready = 1 , InProgress = 2, OnHold = 3 }
        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();

        
    }
}
