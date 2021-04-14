using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Linq;

namespace Metrovalencia.ViewModels
{
    public class DbContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/Metrovalencia_db.sdf";
        public Table<Favorite> Favorites;
        public Table<Stop> Stops;

        public DbContext(string ConnectionString) : base(ConnectionString) { }
    }
}
