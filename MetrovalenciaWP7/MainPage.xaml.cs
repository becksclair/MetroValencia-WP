using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Metrovalencia.Helpers;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Metrovalencia
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            DepartureTime.Value = DateTime.MinValue.Date.Add(new TimeSpan(0, 0, 0));
            ArrivalTime.Value = DateTime.MinValue.Date.Add(new TimeSpan(23, 59, 0));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.InitialDataLoad();
            MetroHelper.StartGetStops();

            if (!App.IsTrial)
            {
                ad.Visibility = Visibility.Collapsed;
            }

            DataContext = App.FavoritesViewModel;
            TripDate.Value = DateTime.Now;

            // Update tiles
            TileManager.UpdateTiles();

            Utils.RemindReview();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            GlobalLoading.Instance.IsLoading = false;

            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("TileId", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                Favorite fav = App.FavoritesViewModel.GetFavorite(index);
                NavigationService.Navigate(new Uri("/ResultsPage.xaml?selectedItem=" + fav.Id, UriKind.Relative));
                return;
            }

            if (App.DepartureStopSearch != null)
                DepartureField.Text = App.DepartureStopSearch.Name;
            if (App.ArrivalStopSearch != null)
                ArrivalField.Text = App.ArrivalStopSearch.Name;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (App.IsTrial)
            {
                Utils.ShowPurchaseInfo();
            }
            else
            {
                NavigationService.Navigate(new Uri("/FavoritePage.xaml", UriKind.Relative));
            }
        }

        private void Favorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoritesList.SelectedIndex == -1) return;
            Favorite fav = FavoritesList.SelectedItem as Favorite;
            NavigationService.Navigate(new Uri("/ResultsPage.xaml?selectedItem=" + fav.Id, UriKind.Relative));
            FavoritesList.SelectedIndex = -1;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.DepartureStopSearch == null || App.ArrivalStopSearch == null)
            {
                return;
            }
                
            var hiniTime = (DateTime)DepartureTime.Value;
            var hfinTime = (DateTime)ArrivalTime.Value;
            var dateObj = (DateTime)TripDate.Value;

            string from = App.DepartureStopSearch.Code;
            string to = App.ArrivalStopSearch.Code;
            string hini = hiniTime.ToString("HH:mm");
            string hfin = hfinTime.ToString("HH:mm");
            string date = dateObj.ToString("dd/MM/yyyy");

            App.DepartureStopSearch = null;
            App.ArrivalStopSearch = null;

            NavigationService.Navigate(new Uri("/ResultsPage.xaml?from=" + from + "&to=" + to +
                "&hini=" + hini + "&hfin=" + hfin + "&date=" + date, UriKind.Relative));
            FavoritesList.SelectedIndex = -1;
        }

        private void feedbackBtn_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Subject = "SimpleScribe feedback";
            email.To = "feedback@steelcode.net";
            email.Show();
        }

        private void steelcodeLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webTask = new WebBrowserTask();
            webTask.Uri = new Uri(steelcodeLink.Text, UriKind.Absolute);
            webTask.Show();
        }

        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            Favorite fav = ((sender as FrameworkElement).DataContext) as Favorite;

            if (fav == null) return;
            NavigationService.Navigate(new Uri("/FavoritePage.xaml?selectedItem=" + fav.Id, UriKind.Relative));
        }

        private void PinMenu_Click(object sender, RoutedEventArgs e)
        {
            Favorite fav = ((sender as FrameworkElement).DataContext) as Favorite;

            if (fav == null) return;
            TileManager.PinTile(fav);
        }

        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            Favorite fav = ((sender as FrameworkElement).DataContext) as Favorite;
            App.FavoritesViewModel.DeleteFavorite(fav);
        }

        private void DepartureField_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectStopPage.xaml?action=departureSearch", UriKind.Relative));
        }

        private void ArrivalField_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectStopPage.xaml?action=arrivalSearch", UriKind.Relative));
        }

        private void MapImage_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsTrial)
            {
                Utils.ShowPurchaseInfo();
            }
            else
            {
                NavigationService.Navigate(new Uri("/ImageViewer.xaml", UriKind.Relative));
            }
        }
    }
}