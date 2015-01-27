using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;




namespace WindowsFormsApplication1
{
    public class SingleValueArrayConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = new Object();
            if (reader.TokenType == JsonToken.StartObject)
            {
                T instance = (T)serializer.Deserialize(reader, typeof(T));
                retVal = new List<T>() { instance };
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                retVal = serializer.Deserialize(reader, objectType);
            }
            return retVal;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
    public class Resource
    {

        [JsonProperty("@label")]
        public string Label { get; set; }

        [JsonProperty("@uri")]
        public string Uri { get; set; }

        [JsonProperty("@contextualScore")]
        public string ContextualScore { get; set; }

        [JsonProperty("@percentageOfSecondRank")]
        public string PercentageOfSecondRank { get; set; }

        [JsonProperty("@support")]
        public string Support { get; set; }

        [JsonProperty("@priorScore")]
        public string PriorScore { get; set; }

        [JsonProperty("@finalScore")]
        public string FinalScore { get; set; }

        [JsonProperty("@types")]
        public string Types { get; set; }
    }

    public class SurfaceForm
    {

        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("@offset")]
        public string Offset { get; set; }

        [JsonProperty("resource")]
        [JsonConverter(typeof(SingleValueArrayConverter<Resource>))]
        public List<Resource> Resource { get; set; }
    }

    public class Annotation
    {

        [JsonProperty("@text")]
        public string Text { get; set; }

        [JsonProperty("surfaceForm")]
        public SurfaceForm[] SurfaceForm { get; set; }
    }

    public class Response
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


                        var obj = JsonConvert.DeserializeObject<Response>(json);

                        var AnnotationCount = obj.Annotation.SurfaceForm.Length;
                        outputString += "Found " + AnnotationCount + " Annotations\r\n\r\n";

                        for (int i = 0; i < AnnotationCount; i++)
                        {
                            var ResourceCount = obj.Annotation.SurfaceForm[i].Resource.Count;
                            var annotation = obj.Annotation.SurfaceForm[i].Name.ToString();
                            var offset = obj.Annotation.SurfaceForm[i].Offset.ToString();
                            outputString += annotation + " at Offset " + offset + "\r\n";

                            if (ResourceCount == 1)
                            {
                                outputString += "Resource:\r\n";
                            } else
                            {
                                outputString += ResourceCount + " Resources:\r\n";
                            }
                      
                            for (int j = 0; j < ResourceCount; j++)
                            {
                                var label = obj.Annotation.SurfaceForm[i].Resource[j].Label.ToString();
                                var uri = obj.Annotation.SurfaceForm[i].Resource[j].Uri.ToString();
                                var contextualScore = obj.Annotation.SurfaceForm[i].Resource[j].ContextualScore.ToString();
                                var percentageOfSecondRank = obj.Annotation.SurfaceForm[i].Resource[j].PercentageOfSecondRank.ToString();
                                var support = obj.Annotation.SurfaceForm[i].Resource[j].Support.ToString();
                                var priorScore = obj.Annotation.SurfaceForm[i].Resource[j].PriorScore.ToString();
                                var finalScore = obj.Annotation.SurfaceForm[i].Resource[j].FinalScore.ToString();
                                var types = obj.Annotation.SurfaceForm[i].Resource[j].Types.ToString();

                                outputString += "  Label: " + label + "\r\n" +
                                                "  Uri: " + uri + "\r\n" +
                                                "  Contextual Score: " + contextualScore + "\r\n" +
                                                "  Percentage Of Second Rank: " + percentageOfSecondRank + "\r\n" +
                                                "  Support: " + support + "\r\n" +
                                                "  Prior Score: " + priorScore + "\r\n" +
                                                "  Final Score: " + finalScore + "\r\n" +
                                                "  Types: " + types + "\r\n";
                                if (ResourceCount > 1)
                                {
                                    outputString += "-----------------------------------------------\r\n";
                                }
                            }

                            outputString += "=============================================================================================\r\n";

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
