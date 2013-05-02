using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace Boom
{
    class Highscore
    {
        private const string ApiKey = "a59a292915406d8b865c894c9ed5eef94a7c12e3";
        private const string GameId = "83eb5b8f28";
        private const string ApiUrl = "https://www.scoreoid.com/api/";

        public void GetScores(int num, Action<IEnumerable<KeyValuePair<string,int>>> callback, Action<string> failed)
        {
            var result = new List<KeyValuePair<string, int>>();

            SubmitRequest("getBestScores", new Dictionary<string, string> { { "order_by", "score" }, { "limit", Convert.ToString(num) } },
                // Success
                xml =>
                {
                    try
                    {
                        XElement xscores = xml.Element("scores");
                        if (xscores == null)
                            throw new InvalidOperationException("Scoreid was unable to load scores.");

                        foreach (XElement xplayer in xscores.Elements("player"))
                        {
                            XElement xscore = xplayer.Element("score");

                            if (xscore != null)
                            {
                                int value = 0;
                                Int32.TryParse(xscore.Attribute("score").Value, out value);

                                string username = xscore.Attribute("data").Value;

                                result.Add(new KeyValuePair<string,int>(username, value));
                            }
                        }

                        callback(result);
                    }
                    catch (Exception ex)
                    {
                        failed(ex.Message);
                    }

                },
                // Failure
                error =>
                {
                    failed(error);
                });
        }

        public void Submit(Action<string> callback, string playerName, int score)
        {
            SubmitRequest("createScore", new Dictionary<string, string> { { "username", Guid.NewGuid().ToString() }, { "data", playerName }, { "score", Convert.ToString(score) } },
                // Success
                 xml =>
                 {
                     try
                     {
                         XElement xsuccess = xml.Element("success");
                         if (xsuccess == null)
                             throw new InvalidOperationException("Scoreid was unable to submit the score.");

                         callback(null);
                     }
                     catch (Exception ex)
                     {
                         callback(ex.Message);
                     }

                 },
                // Failure
                 error =>
                 {
                     callback(error);
                 });
        }

        private void SubmitRequest(string method, Dictionary<string, string> parameters, Action<XDocument> success, Action<string> failed)
        {
            // Create a request

            string uri = String.Format("{0}/{1}", ApiUrl, method);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "POST";

            // What we are sending
            string postData = String.Format("api_key={0}&game_id={1}&response={2}",
                HtmlEncode(ApiKey),
                HtmlEncode(GameId),
                HtmlEncode("XML"));

            StringBuilder sb = new StringBuilder();
            foreach (var param in parameters)
            {
                sb.AppendFormat("&{0}={1}", param.Key, HtmlEncode(param.Value));
            }
            postData = String.Concat(postData, sb.ToString());

            // Turn our request string into a byte stream
            byte[] postBuffer = Encoding.UTF8.GetBytes(postData);

            // This is important - make sure you specify type this way
            webRequest.ContentType = "application/x-www-form-urlencoded";

            int timeoutInterval = 30000;

            DateTime requestDate = DateTime.Now;

            Timer timer = new Timer(
                (state) =>
                {
                    if ((DateTime.Now - requestDate).TotalMilliseconds >= timeoutInterval)
                        webRequest.Abort();

                }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10000));

            try
            {
                webRequest.BeginGetRequestStream(
                    requestAsyncResult =>
                    {
                        try
                        {
                            timer.Change(Timeout.Infinite, Timeout.Infinite);

                            HttpWebRequest request =
                                ((HttpWebRequest)((object[])requestAsyncResult.AsyncState)[0]);

                            byte[] buffer =
                                ((byte[])((object[])requestAsyncResult.AsyncState)[1]);

                            Stream requestStream =
                                request.EndGetRequestStream(requestAsyncResult);

                            requestStream.Write(buffer, 0, buffer.Length);
                            requestStream.Close();

                            requestDate = DateTime.Now;
                            timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));

                            request.BeginGetResponse((state) =>
                            {
                                 timer.Change(Timeout.Infinite, Timeout.Infinite);

                                 HttpWebResponse response = null;

                                 try
                                 {
                                     response =
                                         (HttpWebResponse)((HttpWebRequest)state.AsyncState).EndGetResponse(state);

                                     if (response.StatusCode == HttpStatusCode.OK)
                                     {
                                         // If the request success, then call the success callback
                                         // or the failed callback by reading the response data     
                                         using (Stream stream = response.GetResponseStream())
                                         {
                                             try
                                             {
                                                 XDocument xdoc = XDocument.Load(stream);

                                                 // Data contains error notification.
                                                 if (xdoc.Root.Name == "error")
                                                     throw new InvalidOperationException(xdoc.Root.Value);

                                                 success(xdoc);
                                             }
                                             catch (Exception ex)
                                             {
                                                 failed(ex.Message);
                                             }

                                             stream.Close();
                                         }
                                     }
                                     else
                                     {
                                         // If the request fails, then call the failed callback
                                         // to notfiy the failing status description of the request
                                         failed(response.StatusDescription);
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     // If the request fails, then call the failed callback
                                     // to notfiy the failing status description of the request
                                     failed(ex.Message);
                                 }
                                 finally
                                 {
                                     request.Abort();

                                     if (response != null)
                                         response.Close();
                                 }

                             }, request);
                        }
                        catch (Exception ex)
                        {
                            // Raise an error in case of exception
                            // when submitting a request
                            failed(ex.Message);
                        }

                    }, new object[] { webRequest, postBuffer });
            }
            catch (Exception ex)
            {
                // Raise an error in case of exception
                // when submitting a request
                failed(ex.Message);
            }

        }

        private string HtmlEncode(string value)
        {
            return Uri.EscapeUriString(value);
        }
    }
}
