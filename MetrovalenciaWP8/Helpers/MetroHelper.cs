using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using HtmlAgilityPack;
using Metrovalencia.ViewModels;
using Microsoft.Phone.Controls;

namespace Metrovalencia.Helpers
{
    public class MetroHelper
    {
        public MetroHelper()
        {
            
        }

        public static void StartGetStops()
        {
            if (!Utils.InternetIsAvailable()) return;
            try
            {
                if (App.StopsViewModel == null || App.StopsViewModel.Items.Count() == 0)
                {
                    MetroHelper mi = new MetroHelper();
                    mi.GetStops();
                }
            }
            catch (Exception) { }
        }

        public void GetStops()
        {
            WebHelper.Get(WebHelper.API_QUERY_URL, resp =>
            {
                GlobalLoading.Instance.SetLoading(true, "Loading stops");

                App.StopsViewModel.FlushCache();
                Collection<Stop> Stops = new Collection<Stop>();

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = false;
                worker.WorkerReportsProgress = true;

                worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
                {
                    BackgroundWorker w = sender as BackgroundWorker;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml((string) args.Argument);

                    foreach (var s in doc.DocumentNode.SelectNodes("//select[@name='origen']"))
                    {
                        var stops = s.SelectNodes("option");
                        foreach (var st in stops)
                        {
                            if (st.NextSibling.InnerText.CompareTo("Elige parada") == 0)
                            {
                                continue;
                            }
                            Stops.Add(new Stop { Name = st.NextSibling.InnerText, Code = st.GetAttributeValue("value", "") });
                        }
                    }
                });

                worker.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
                {
                    App.StopsViewModel.InsertStops(Stops);
                    GlobalLoading.Instance.IsLoading = false;
                });

                worker.RunWorkerAsync(resp);
                return true;
            });
        }

        public void GetStopsFromResult(string resp)
        {
            GlobalLoading.Instance.SetLoading(true, "Loading stops");

            App.StopsViewModel.FlushCache();
            Collection<Stop> Stops = new Collection<Stop>();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = false;
            worker.WorkerReportsProgress = true;

            worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs args)
            {
                BackgroundWorker w = sender as BackgroundWorker;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml((string)args.Argument);

                foreach (var s in doc.DocumentNode.SelectNodes("//select[@name='origen']"))
                {
                    var stops = s.SelectNodes("option");
                    foreach (var st in stops)
                    {
                        if (st.NextSibling.InnerText.CompareTo("Elige parada") == 0)
                        {
                            continue;
                        }
                        Stops.Add(new Stop { Name = st.NextSibling.InnerText, Code = st.GetAttributeValue("value", "") });
                    }
                }
            });

            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs args)
            {
                App.StopsViewModel.InsertStops(Stops);
                GlobalLoading.Instance.IsLoading = false;
            });

            worker.RunWorkerAsync(resp);
        }

        public void RequestQuery(Favorite fav, string from, string to)
        {
            RequestQuery(null, fav, from, to, "00:00", "23:59", DateTime.Now.ToString("dd/MM/yyyy"), null);
        }

        public void RequestQuery(Favorite fav, string from, string to, Func<Favorite, bool> fAfter)
        {
            RequestQuery(null, fav, from, to, "00:00", "23:59", DateTime.Now.ToString("dd/MM/yyyy"), fAfter);
        }

        public void RequestQuery(WebBrowser view, Favorite fav, string from, string to)
        {
            RequestQuery(view, fav, from, to, "00:00", "23:59", DateTime.Now.ToString("dd/MM/yyyy"), null);
        }

        public void RequestQuery(WebBrowser view, Favorite fav, string from, string to, string hini, string hfin, string date, Func<Favorite, bool> fAfter)
        {
            GlobalLoading.Instance.SetLoading(true, "Loading results query");

            string query = WebHelper.API_QUERY_URL + "?res=0&key=0&calcular=1&origen=" +
                from + "&destino=" + to + "&fecha=" + date + "&hini=" + hini + "&hfin=" + hfin;

            string backgroundColor = App.IsDarkThemeEnabled ? "black" : "white";
            string foregroundColor = App.IsDarkThemeEnabled ? "white" : "#222222";

            string html_header = "<!doctype html>" +
                                    "<html>" +
                                    "<head>" +
                                        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0\" />" +
                                        "<style type='text/css'>" +
                                            "body {" +
                                                "width: 100%;" +
                                                "font-family: 'Segoe WP Light';" +
                                                "background: " + backgroundColor + ";" +
                                                "color: " + foregroundColor + ";" +
                                            "}" +
                                            "table {" +
                                                "font-size: 30px;" +
                                            "}" +
                                            "table tr { line-height: 28px; border-bottom: 1px solid #222222; }" +
                                            "table tbody td {" +
                                                "padding-top: 0px;" +
                                                "padding-bottom: 10px;" +
                                                "padding-left: 0px;" +
                                                "padding-right: 7px;" +
                                            "}" +
                                        "</style>" +
                                    "</head><body>";

            string html_footer = "</body></html>";
            string html = html_header;
            Collection<string> transbords = new Collection<string>();

            WebHelper.Get(query, resp =>
            {
                GetStopsFromResult(resp);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(resp);

                int currentTable = 1;
                string next = "";

                var tables = doc.DocumentNode.SelectNodes("//table");
                if (tables.Count > 1)
                {
                    var transbordTexts = doc.DocumentNode.SelectNodes("//span[@class='texto_transbordo']");
                    if (transbordTexts != null)
                    {
                        foreach (var t in transbordTexts)
                        {
                            transbords.Add(t.InnerText);
                        }
                    }
                }

                foreach (var table in tables)
                {
                    if (tables.Count > 1)
                    {
                        html += "<h3>" + transbords[currentTable-1] + "<h3>";
                    }
                    html += "<table><tbody>";

                    bool firstRow = true;
                    var rows = table.SelectNodes(".//tr");
                    foreach (var row in rows)
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                            row.Remove();
                            continue;
                        }
                        html += "<tr>";

                        var cells = row.SelectNodes("td");
                        if (cells != null)
                        {
                            foreach (var cell in cells)
                            {
                                if (cell.InnerText.Length < 4)
                                {
                                    cell.Remove();
                                    continue;
                                }
                                html += "<td>" + cell.InnerHtml + "</td>";
                                if (currentTable == 1)
                                {
                                    next += cell.InnerHtml + "\n";
                                }
                            }
                        }

                        html += "</tr>";
                    }
                    next = next.Substring(0, next.LastIndexOf('\n'));
                    html += "</tbody></table>";
                    currentTable++;
                    firstRow = true;
                }
                html += html_footer;

                if (fav != null)
                {
                    fav.CacheDate = DateTime.Now;
                    fav.Cache = html;
                    fav.Next = next;
                    App.FavoritesViewModel.SaveChanges();
                }

                if (view != null)
                {
                    view.NavigateToString(html);
                }
                if (fAfter != null)
                {
                    fAfter(fav);
                }
                GlobalLoading.Instance.IsLoading = false;
                return true;
            });
        }
    }
}
