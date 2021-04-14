using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Metrovalencia.ViewModels
{
    public class ViewModelStops : INotifyPropertyChanged
    {
        private DbContext _dataContext;
        private DbContext DataContext
        {
            get
            {
                if (_dataContext != null)
                {
                    return _dataContext;
                }
                return _dataContext = new DbContext(DbContext.DBConnectionString);
            }
        }

        public IEnumerable<Stop> _items;
        public IEnumerable<Stop> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public IList<Stop> GetStops()
        {
            Items = DataContext.Stops.OrderBy(x => x.Name);
            return Items.ToList<Stop>();
        }

        public Stop GetStop(int index)
        {
            return DataContext.Stops.Where(x => x.Id == index).FirstOrDefault();
        }

        public Stop FindByCode(string code)
        {
            return DataContext.Stops.Where(x => x.Code == code).FirstOrDefault();
        }

        public void InsertStops(Collection<Stop> stops)
        {
            foreach (var item in stops)
            {
                DataContext.Stops.InsertOnSubmit(item);
            }
            DataContext.SubmitChanges();
            GetStops();
        }

        public void InsertStop(Stop stop)
        {
            DataContext.Stops.InsertOnSubmit(stop);
            DataContext.SubmitChanges();
            GetStops();
        }

        public void SaveChanges()
        {
            DataContext.SubmitChanges();
        }

        public void DeleteStop(Stop stop)
        {
            DataContext.Stops.DeleteOnSubmit(stop);
            DataContext.SubmitChanges();
            GetStops();
        }

        public void FlushCache()
        {
            DataContext.Stops.DeleteAllOnSubmit(DataContext.Stops.OrderBy(x => x.Name));
            DataContext.SubmitChanges();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
