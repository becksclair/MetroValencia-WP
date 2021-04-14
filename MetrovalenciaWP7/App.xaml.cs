using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Metrovalencia.Helpers;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Shell;

namespace Metrovalencia
{
    public partial class App : Application
    {
        public static int APP_VERSION = 0;

        public static bool IsDarkThemeEnabled { get; set; }

        private static ViewModelFavorites favoritesViewModel = null;
        public static ViewModelFavorites FavoritesViewModel
        {
            get
            {
                if (favoritesViewModel == null)
                    favoritesViewModel = new ViewModelFavorites();
                return favoritesViewModel;
            }
        }

        private static ViewModelStops stopsViewModel = null;
        public static ViewModelStops StopsViewModel
        {
            get
            {
                if (stopsViewModel == null)
                    stopsViewModel = new ViewModelStops();
                return stopsViewModel;
            }
        }

        private static IList<Stop> stops = null;
        public static IList<Stop> Stops
        {
            get
            {
                if (stops == null)
                    stops = App.StopsViewModel.GetStops();
                return stops;
            }
        }

        public static Stop DepartureStopSearch { get; set; }
        public static Stop ArrivalStopSearch { get; set; }
        public static Stop DepartureStopFavorite { get; set; }
        public static Stop ArrivalStopFavorite { get; set; }

        public static bool IsTrial
        {
            get;
            // setting the IsTrial property from outside is not allowed
            private set;
        }

        private void DetermineIsTrail()
        {
            #if TRIAL
                // return true if debugging with trial enabled (DebugTrial configuration is active)
                IsTrial = true;
            #else
                var license = new Microsoft.Phone.Marketplace.LicenseInformation();
                IsTrial = license.IsTrial();
            #endif
        }

        public PhoneApplicationFrame RootFrame { get; private set; }

        private void OverrideColorsViaCode()
        {
            (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 222, 0, 0);
            //(App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = Colors.Green;
            //(App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = Colors.Purple;
        }

        public App()
        {
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
            InitializePhoneApplication();
            OverrideColorsViaCode();

            using (DbContext db = new DbContext(DbContext.DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    // Create database
                    db.CreateDatabase();

                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                    dbUpdater.Execute();
                }
                else
                {
                    //db.DeleteDatabase();
                    //db.CreateDatabase();

                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                    dbUpdater.Execute();
                }
            }

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // refresh the value of the IsTrial property when the application is launched
            DetermineIsTrail();
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            MetroHelper.StartGetStops();

            // refresh the value of the IsTrial property when the application is launched
            DetermineIsTrail();
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            GlobalLoading.Instance.Initialize(RootFrame);
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}