using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Metrovalencia.Helpers;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;

namespace Metrovalencia
{
    public partial class SelectStopPage : PhoneApplicationPage
    {
        private string ActionCode { get; set; }

        public SelectStopPage()
        {
            InitializeComponent();
            GlobalLoading.Instance.IsLoading = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var selected = from s in App.Stops
                           group s by s.Group into c
                           orderby c.Key
                           select new Group<Stop>(c.Key, c);
            StopsField.ItemsSource = selected.ToList();

            string action = "";
            if (NavigationContext.QueryString.TryGetValue("action", out action))
            {
                ActionCode = action;
            }
        }

        private void StopsField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StopsField.SelectedItem == null) return;
            Stop stop = StopsField.SelectedItem as Stop;

            if (ActionCode.CompareTo("departureSearch") == 0)
            {
                App.DepartureStopSearch = stop;
            }
            if (ActionCode.CompareTo("departureFavorite") == 0)
            {
                App.DepartureStopFavorite = stop;
            }
            if (ActionCode.CompareTo("arrivalSearch") == 0)
            {
                App.ArrivalStopSearch = stop;
            }
            if (ActionCode.CompareTo("arrivalFavorite") == 0)
            {
                App.ArrivalStopFavorite = stop;
            }
            NavigationService.GoBack();
        }
        /*
        private void StopsField_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            ItemContainerGenerator itemContainerGenerator = e.ItemsControl.ItemContainerGenerator;
            SwivelTransition transition = new SwivelTransition();
            transition.Mode = SwivelTransitionMode.ForwardIn;

            int itemCount = e.ItemsControl.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                UIElement element = itemContainerGenerator.ContainerFromIndex(i) as UIElement;
                ITransition animation = transition.GetTransition(element);
                animation.Begin();
            }
        }

        private void StopsField_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            e.Cancel = true;

            SwivelTransition transition = new SwivelTransition();
            transition.Mode = SwivelTransitionMode.ForwardOut;

            ItemContainerGenerator itemContainerGenerator = e.ItemsControl.ItemContainerGenerator;

            int animationFinished = 0;
            int itemCount = e.ItemsControl.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                UIElement element = itemContainerGenerator.ContainerFromIndex(i) as UIElement;

                ITransition animation = transition.GetTransition(element);
                animation.Completed += delegate
                {
                    // Close the group view when all animations have completed
                    if ((++animationFinished) == itemCount)
                    {
                        StopsField.CloseGroupView();
                        StopsField.ScrollToGroup(e.SelectedGroup);
                    }
                };
                animation.Begin();
            }

        }*/
    }

    public class Group<T> : IEnumerable<T>
    {
        public Group(string name, IEnumerable<T> items)
        {
            this.Name = name;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            Group<T> that = obj as Group<T>;
            return (that != null) && (this.Name.Equals(that.Name));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string Name { get; set; }
        public IList<T> Items { get; set; }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }
}