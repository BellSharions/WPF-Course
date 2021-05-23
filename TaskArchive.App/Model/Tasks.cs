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
        //тут главный класс с делами. надо пофиксить тут что-то, чтобы всё заработало, точно не могу найти проблему
        //private string _name;
        //private string _channel; //чтобы понимать, чей таск. channel вместо юзера чтобы не путать имена(возможно потом изменю/удалю, когда найду норм решение)
        //private string _description;
        //private string _tematic;
        //private string _comment;
        //private ObservableCollection<KeyWordItem> _keyWords;
        //private ObservableCollection<string> _images;

        public string Name { get; set; }
        public string Channel { get; set; }
        public string Descrition { get; set; }
        public string Tematic { get; set; }
        public ObservableCollection<KeyWordItem> KeyWords { get; set; } = new ObservableCollection<KeyWordItem>();
        public double Size { get; set; }
        public DateTime PublishData { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public enum StatusCheck { Ready = 1 , InProgress = 2, OnHold = 3 }
        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();

        
    }
}
