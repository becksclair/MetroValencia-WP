using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Metrovalencia.Resources;

namespace Metrovalencia.ViewModels
{
    public class ViewModelFavorites : INotifyPropertyChanged
    {
        public ImageBrush PanoramaBackgroundImage
        {
            get
            {
                var lightThemeEnabled = (Visibility)App.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
                var url = lightThemeEnabled ? "/Images/PanoramaBackgroundLight.jpg" : "/Images/PanoramaBackgroundDark.jpg";

                var brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute))
                };
                App.IsDarkThemeEnabled = lightThemeEnabled ? false : true;
                return brush;
            }
        }

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

        public IEnumerable<Favorite> _items;
        public IEnumerable<Favorite> Items
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

        public void GetFavorites()
        {
            Items = DataContext.Favorites.OrderBy(x => x.Id);
        }

        public Favorite GetFavorite(int index)
        {
            return DataContext.Favorites.Where(x => x.Id == index).FirstOrDefault();
        }

        public void InsertFavorite(Favorite favorite)
        {
            DataContext.Favorites.InsertOnSubmit(favorite);
            DataContext.SubmitChanges();
            GetFavorites();
        }

        public void SaveChanges()
        {
            DataContext.SubmitChanges();
        }

        public void DeleteFavorite(Favorite favorite)
        {
            DataContext.Favorites.DeleteOnSubmit(favorite);
            DataContext.SubmitChanges();
            GetFavorites();
        }

        public void FlushCache()
        {
            DataContext.DeleteDatabase();
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
