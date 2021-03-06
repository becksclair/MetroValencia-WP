using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Shell;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools;

namespace Metrovalencia.UnitTests
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.IsVisible = false;
            var testPage = UnitTestSystem.CreateTestPage();
            IMobileTestPage imobileTestPage = testPage as IMobileTestPage;
            BackKeyPress += (x, xe) => xe.Cancel = imobileTestPage.NavigateBack();
            (Application.Current.RootVisual as PhoneApplicationFrame).Content = testPage;
        }
    }
}