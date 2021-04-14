using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using Metrovalencia.Helpers;
using Microsoft.Phone.Controls;

namespace Metrovalencia.ViewModels
{
    [Table]
    public class Favorite : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public Favorite() { }

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

        private string _title;
        [Column]
        public string Title {
            get
            {
                return _title;
            }
            set
            {
                NotifyPropertyChanging("Title");
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string _departureStop;
        [Column]
        public string DepartureStop {
            get
            {
                return _departureStop;
            }
            set
            {
                NotifyPropertyChanging("DepartureStop");
                _departureStop = value;
                NotifyPropertyChanged("DepartureStop");
            }
        }

        private string _arrivalStop;
        [Column]
        public string ArrivalStop
        {
            get
            {
                return _arrivalStop;
            }
            set
            {
                NotifyPropertyChanging("ArrivalStop");
                _arrivalStop = value;
                NotifyPropertyChanged("ArrivalStop");
            }
        }

        private DateTime _cacheDate;
        [Column(CanBeNull=true)]
        public DateTime CacheDate
        {
            get
            {
                return _cacheDate;
            }
            set
            {
                NotifyPropertyChanging("CacheDate");
                _cacheDate = value;
                NotifyPropertyChanged("CacheDate");
            }
        }

        private string _cache;
        [Column(DbType="NText", UpdateCheck=UpdateCheck.Never, CanBeNull = true)]
        public string Cache
        {
            get
            {
                return _cache;
            }
            set
            {
                NotifyPropertyChanging("Cache");
                _cache = value;
                NotifyPropertyChanged("Cache");
            }
        }

        private string _next;
        [Column(CanBeNull = true)]
        public string Next
        {
            get
            {
                return _next;
            }
            set
            {
                NotifyPropertyChanging("Next");
                _next = value;
                NotifyPropertyChanged("Next");
            }
        }

        public void FetchCache()
        {
            MetroHelper mi = new MetroHelper();
            mi.RequestQuery(this, this.DepartureStop, this.ArrivalStop);
        }

        public void FetchQuery(WebBrowser view)
        {
            MetroHelper mi = new MetroHelper();
            mi.RequestQuery(view, this, DepartureStop, ArrivalStop);
        }

        public bool IsCacheValid()
        {
            if (CacheDate != null && Cache != null &&
                CacheDate.Day <= DateTime.Now.Day &&
                (CacheDate.Day - 1) == (DateTime.Now.Day - 1))
            {
                return true;
            }
            else
            {
                return false;
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
