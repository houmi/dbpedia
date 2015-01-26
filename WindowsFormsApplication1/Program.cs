using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;




namespace WindowsFormsApplication1
{

    internal class SurfaceForm
    {

        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("@offset")]
        public string Offset { get; set; }

        [JsonProperty("resource")]
        public object Resource { get; set; }
    }

    internal class Annotation
    {

        [JsonProperty("@text")]
        public string Text { get; set; }

        [JsonProperty("surfaceForm")]
        public SurfaceForm[] SurfaceForm { get; set; }
    }

    internal class Response
    {

        [JsonProperty("annotation")]
        public Annotation Annotation { get; set; }
    }


    static class Program
    {
      
        public static string CreateRequest(string queryString, double confidence, int support)
        {
            string UrlRequest = "http://pishi.cloudapp.net:2222/rest/candidates?text=" + queryString + 
                "&confidence=" + confidence + "&support=" + support;
            // queryString +
            // "&confidence=.2&support=20";
            Console.WriteLine(UrlRequest);
            return (UrlRequest);
        }


        public static string MakeRequest(string requestUrl)
        {
            string outputString="";
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Accept = "application/json";
                request.Method = "GET";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {

                        string json = reader.ReadToEnd();

                        //outputString += json;

                        var obj = JsonConvert.DeserializeObject<Response>(json);

                        var count = obj.Annotation.SurfaceForm.Length;
                        outputString += "Found " + count + " Surface Forms";



                        for (int idx = 0; idx < count; idx++)
                        {
                            string temp = obj.Annotation.SurfaceForm[idx].Resource.ToString();
                            outputString += temp;
                            outputString += "\n";
                        }

                    }
                }

                return outputString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                outputString += e.Message;
                return outputString;
            }
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
