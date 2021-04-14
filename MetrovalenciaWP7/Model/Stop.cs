using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace Metrovalencia.ViewModels
{
    [Table]
    public class Stop : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public Stop() { }

        private int _id;

        [Column(IsPrimaryKey=true, IsDbGenerated=true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id {
            get
            {
                return _id;
            }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string _name;

        [Column]
        public string Name {
            get
            {
                return _name;
            }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _code;

        [Column]
        public string Code {
            get
            {
                return _code;
            }
            set
            {
                NotifyPropertyChanging("Code");
                _code = value;
                NotifyPropertyChanged("Code");
            }
        }

        public string Group {
            get
            {
                return Name.Substring(0, 1);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
