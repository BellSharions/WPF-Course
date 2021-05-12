using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksArchive.Model;

namespace TasksArchive.App.Model
{
    public class KeyWordItem : BaseVM
    {
        public string Value { get; set; }

        public KeyWordItem(string value)
        {
            Value = value;
        }
    }
}
