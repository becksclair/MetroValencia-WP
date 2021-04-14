using System;
using System.Windows;
using System.Windows.Navigation;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;

namespace Metrovalencia
{
    public partial class FavoritePage : PhoneApplicationPage
    {
        public Favorite Current { get; set; }

        public FavoritePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.IsTrial)
            {
                ad.Visibility = Visibility.Collapsed;
                //LayoutRoot.RowDefinitions.RemoveAt(1);
            }

            object val;
            if (State.TryGetValue("name", out val))
                NameField.Text = val.ToString();
            if (State.TryGetValue("departure", out val))
                DepartureField.Text = val.ToString();
            if (State.TryGetValue("arrival", out val))
                ArrivalField.Text = val.ToString();

            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                Current = App.FavoritesViewModel.GetFavorite(index);

                if (Current != null)
                {
                    NameField.Text = Current.Title;
                    DepartureField.Text = App.StopsViewModel.FindByCode(Current.DepartureStop).Name;
                    ArrivalField.Text = App.StopsViewModel.FindByCode(Current.ArrivalStop).Name;
                }
            }

            if (App.DepartureStopFavorite != null)
                DepartureField.Text = App.DepartureStopFavorite.Name;
            if (App.ArrivalStopFavorite != null)
                ArrivalField.Text = App.ArrivalStopFavorite.Name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            State["name"] = NameField.Text;
            State["departure"] = DepartureField.Text;
            State["arrival"] = ArrivalField.Text;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameField.Text;

            if (Current == null)
            {
                if (NameField.Text.Length == 0)
                {
                    MessageBox.Show("Name can't be empty");
                    return;
                }
                if (DepartureField.Text.Length == 0)
                {
                    MessageBox.Show("Departure can't be empty");
                    return;
                }
                if (ArrivalField.Text.Length == 0)
                {
                    MessageBox.Show("Arrival can't be empty");
                    return;
                }

                App.FavoritesViewModel.InsertFavorite(new Favorite { Title = name, DepartureStop = App.DepartureStopFavorite.Code, ArrivalStop = App.ArrivalStopFavorite.Code, CacheDate = DateTime.Now });
            }
            else
            {
                Current.Title = NameField.Text;
                Current.DepartureStop = App.DepartureStopFavorite.Code;
                Current.ArrivalStop = App.ArrivalStopFavorite.Code;
                App.FavoritesViewModel.SaveChanges();
            }
            App.DepartureStopFavorite = null;
            App.ArrivalStopFavorite = null;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void DepartureField_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectStopPage.xaml?action=departureFavorite", UriKind.Relative));
        }

        private void ArrivalField_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectStopPage.xaml?action=arrivalFavorite", UriKind.Relative));
        }
    }
}