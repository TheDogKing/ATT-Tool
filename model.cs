using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace ATT_Tool
{
    public class model : INotifyPropertyChanged
    {
        string sname;
        string stype;
        string smname;

        public model parent { get; set; }
        public int spacecount { get; set; }
        public string name { get { return sname; } set { sname = value; smname = sname.Trim('/'); OnPropertyChanged("name"); } }
        public string mname { get { return smname; } set { smname = value; OnPropertyChanged("mname"); } }
        public string purpose { get; set; }
        public string type { get { return stype; } set { stype = value; OnPropertyChanged("type"); } }
        public ObservableCollection<model> Items { get; set; }
        public model()
        {
            Items = new ObservableCollection<model>();
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class VModel : ViewModelBase
    {
        public ObservableCollection <model> models { get; set; }
        public VModel()
        {
            models = new ObservableCollection<model>();
        }
    }
}
