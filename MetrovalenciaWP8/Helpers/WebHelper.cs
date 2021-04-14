using System;
using System.Net;
using System.Windows;
using System.IO;

namespace Metrovalencia.Helpers
{
    public class WebHelper
    {
        public static string API_QUERY_URL = "http://www.metrovalencia.es/horarios.mobi.php";

        public static void Post(string url, string Body, Func<string, bool> fAfter)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "POST";
            request.BeginGetRequestStream(ar =>
            {
                var requestStream = request.EndGetRequestStream(ar);
                using (var sw = new StreamWriter(requestStream))
                {
                    sw.Write(Body);
                    sw.Flush();
                    sw.Close();
                }

                request.BeginGetResponse(a =>
                {
                    try
                    {
                        var rsp = request.EndGetResponse(a);
                        var responseStream = rsp.GetResponseStream();
                        using (var sr = new StreamReader(responseStream))
                        {
                            string resp = sr.ReadToEnd();
                            sr.Close();
                            object[]  args = { resp };
                            Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                        }
                        rsp.Close();
                    }
                    catch (Exception)
                    {
                        object[] args = { "" };
                        Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                    }

                }, request);

            }, request);
        }

        public static void Get(string url, Func<string, bool> fAfter)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "GET";
            request.BeginGetResponse(a =>
            {
                try
                {
                    var rsp = request.EndGetResponse(a);
                    var responseStream = rsp.GetResponseStream();
                    using (var sr = new StreamReader(responseStream))
                    {
                        string resp = sr.ReadToEnd();
                        sr.Close();
                        object[] args = { resp };
                        Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                    }
                    rsp.Close();
                }
                catch (Exception e)
                {
                    object[] args = { e };
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(e.Message);
                    });
                }

            }, request);
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

    }
}
