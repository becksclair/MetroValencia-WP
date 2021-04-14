using System;
using System.Linq;
using System.Windows;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Metrovalencia.Helpers
{
    public class TileManager
    {
        private static readonly string StopTileUriSource = "TileId=";

        public static void PinTile(Favorite fav)
        {
            GlobalLoading.Instance.SetLoading(true, "Preparing tile data");
            ShellTile t = FindTile(StopTileUriSource + fav.Id);

            if (t != null)
            {
                t.Delete();
            }

            StandardTileData tile = GetStopTileData(fav);
            if (tile == null) return;
            string tileUri = string.Concat("/MainPage.xaml?", StopTileUriSource + fav.Id);
            ShellTile.Create(new Uri(tileUri, UriKind.Relative), tile);
            GlobalLoading.Instance.IsLoading = false;

            // Set background updater
            //ScheduleTileUpdates();
        }

        /*
        public static void ScheduleTileUpdates()
        {
            ShellTileSchedule tileSchedule = new ShellTileSchedule(FindTile(StopTileUriSource));
            tileSchedule.Recurrence = UpdateRecurrence.Interval;
            tileSchedule.Interval = UpdateInterval.EveryHour;
            tileSchedule.StartTime = DateTime.Now;
            tileSchedule.Start();
        }*/

        public static void UpdateTiles()
        {
            foreach (var fav in App.FavoritesViewModel.Items)
            {
                UpdateTile(fav);
            }
        }

        private static StandardTileData GetStopTileData(Favorite fav)
        {
            // Make sure we've got cached results
            if (!fav.IsCacheValid())
            {
                MetroHelper mi = new MetroHelper();
                mi.RequestQuery(fav, fav.DepartureStop, fav.ArrivalStop, resp =>
                {
                    GlobalLoading.Instance.IsLoading = true;
                    StandardTileData tile = UpdateTile(resp);
                    string tileUri = string.Concat("/MainPage.xaml?", StopTileUriSource + resp.Id);
                    ShellTile.Create(new Uri(tileUri, UriKind.Relative), tile);
                    GlobalLoading.Instance.IsLoading = false;
                    return true;
                });
                return null;
            }
            else
            {
                return UpdateTile(fav);
            }
        }

        private static StandardTileData UpdateTile(Favorite fav)
        {
            //if (FindTile(StopTileUriSource + fav.Id) == null) return null;
            if (fav.Cache == null) return null;

            string[] Next = fav.Next.Split('\n');
            string nextMetros = "";

            foreach (var item in Next)
            {
                var h = item.Split(':');
                if (h[0].CompareTo("") == 0 || h[0].CompareTo("") == 0)
                    continue;

                var t = DateTime.MinValue.Date.Add(new TimeSpan(Int32.Parse(h[0]), Int32.Parse(h[1]), 0));
                if (t.Hour >= DateTime.Now.Hour)
                {
                    nextMetros += item + "\n";
                }
            }

            StandardTileData tileData = new StandardTileData
            {
                Title = fav.Title,
                BackgroundImage = new Uri("/Images/ApplicationIcon_173x173.png", UriKind.Relative),
                BackTitle = fav.Title,
                BackBackgroundImage = new Uri("/Images/Background.png", UriKind.Relative),
                Count = nextMetros.Split('\n').Count(),
                BackContent = nextMetros
            };
            return tileData;
        }

        public static ShellTile FindTile(string partOfUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(tile => tile.NavigationUri.ToString().Contains(partOfUri));
            return shellTile;
        }
    }
}
