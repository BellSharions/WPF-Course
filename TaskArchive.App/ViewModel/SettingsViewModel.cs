using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TasksArchive.Model;

namespace TasksArchive.App.ViewModel
{
    class SettingsViewModel : BaseVM
    {
        //думаю тут разместить настройки различные
        public string HexMedium { get; set; }


        public ICommand Save
        {
            get
            {
                return new DelegateCommand(() =>
                {

                    var color = (Application.Current.Resources["DarkBaseBrush"] as SolidColorBrush);
                    

                });
            }
        }

    }
}
