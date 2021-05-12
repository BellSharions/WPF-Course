using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TasksArchive.Model;

namespace TasksArchive.App.Model
{
    public class DataBase : BaseVM
    {
        //тут я всякие штуки с локальной базой данных делал, скорее всего объединю с контекстом, чтобы это не просто с файликами работало
        private static DataBase _Instance = new DataBase();
        public static DataBase GetInstance() => _Instance;

        public ObservableCollection<string> KeyWords { get; set; }

        private DataBase()
        {
            KeyWords = File.Exists("KeyWordsData.json") ? JsonConvert.DeserializeObject<ObservableCollection<string>>(File.ReadAllText("KeyWordsData.json")) : new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(KeyWords, new object());
            KeyWords.CollectionChanged += (s, e) =>
            {
                File.WriteAllText("KeyWordsData.json", JsonConvert.SerializeObject(KeyWords));
            };

        }

    }
}
