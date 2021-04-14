using System;
using System.Windows;
using System.Windows.Navigation;
using Metrovalencia.Helpers;
using Metrovalencia.Resources;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Metrovalencia
{
    public partial class ResultsPage : PhoneApplicationPage
    {
        private Favorite Current { get; set; }
        private string From { get; set; }
        private string To { get; set; }

        public bool Loading { get; set; }
        private bool HasAppBar { get; set; }

        public string Html { get; set; }

        public ResultsPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ResultsPage_Loaded);
        }

        private void ResultsPage_Loaded(object sender, RoutedEventArgs e)
        {
            BuildApplicationBar();

            string backgroundColor = App.IsDarkThemeEnabled ? "black" : "white";
            string foregroundColor = App.IsDarkThemeEnabled ? "white" : "#222222";
            string defaultPage = "<!doctype html>" +
                                    "<html>" +
                                    "<head>" +
                                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0\" />" +
                                        "<style type='text/css'>" +
                                            "body {" +
                                                "width: 100%;" +
                                                "font-family: 'Segoe WP Light';" +
                                                "background: " + backgroundColor + ";" +
                                                "color: " + foregroundColor + ";" +
                                            "}</style></head><body></body></html>";

            // Make sure the view has the background set
            ContentPanel.NavigateToString(defaultPage);

            ContentPanel.Navigating += new EventHandler<NavigatingEventArgs>(ContentPanel_Navigating);
            ContentPanel.Navigated += new EventHandler<NavigationEventArgs>(ContentPanel_Navigated);
            ContentPanel.NavigationFailed += (s, le) =>
            {
                MessageBox.Show("Navigation error");
            };

            try
            {
                ContentPanel.NavigateToString(Html);
                Loading = false;
            }
            catch (Exception) { }
        }

        private void BuildApplicationBar()
        {
            if (!HasAppBar) return;

            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("Images/Dark/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.ResultsPageAppBarSave;
            appBarButton.Click += MenuSave_Click;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        void ContentPanel_Navigated(object sender, NavigationEventArgs e)
        {
            if (!App.IsTrial)
            {
                ad.Visibility = Visibility.Collapsed;
            }

            if (!Loading)
            {
                GlobalLoading.Instance.IsLoading = false;
                return;
            }
            Loading = false;
        }

        void ContentPanel_Navigating(object sender, NavigatingEventArgs e)
        {
            GlobalLoading.Instance.SetLoading(true, "Loading results");
            GlobalLoading.Instance.Text = "Navigating...";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.State["Loaded"] = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            object loaded;
            this.State.TryGetValue("Loaded", out loaded);
            if (loaded != null && (bool)loaded) return;

            GlobalLoading.Instance.SetLoading(true, "Loading results");
            Loading = true;

            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                HasAppBar = false;

                int index = int.Parse(selectedIndex);
                Current = App.FavoritesViewModel.GetFavorite(index);
                if (Current == null)
                    Utils.InitialDataLoad();

                ApplicationTitle.Text = Current.Title;

                if (Current.IsCacheValid())
                {
                    Html = Current.Cache;
                }
                else
                {
                    // Check for internet connection
                    if (!Utils.InternetIsAvailable())
                    {
                        MessageBox.Show("Internet not available at this time.");
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        Current.FetchQuery(ContentPanel);
                    }
                }
            }
            else
            {
                HasAppBar = true;

                // Load a search query
                string from = "";
                string to = "";
                string hini = "";
                string hfin = "";
                string date = "";

                NavigationContext.QueryString.TryGetValue("from", out from);
                NavigationContext.QueryString.TryGetValue("to", out to);
                NavigationContext.QueryString.TryGetValue("hini", out hini);
                NavigationContext.QueryString.TryGetValue("hfin", out hfin);
                NavigationContext.QueryString.TryGetValue("date", out date);

                From = from;
                To = to;

                MetroHelper helper = new MetroHelper();
                helper.RequestQuery(ContentPanel, null, from, to, hini, hfin, date, null);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var uri = "/MainPage.xaml";
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            if (App.IsTrial)
            {
                Utils.ShowPurchaseInfo();
            }
            else
            {
                Stop dStop = App.StopsViewModel.FindByCode(From);
                Stop aStop = App.StopsViewModel.FindByCode(To);

                String title = dStop.Name + " - " + aStop.Name;

                App.FavoritesViewModel.InsertFavorite(new Favorite { Title = title, DepartureStop = From, ArrivalStop = To, CacheDate = DateTime.Now });
                MessageBox.Show("Current search saved.");
            }
        }
    }
}