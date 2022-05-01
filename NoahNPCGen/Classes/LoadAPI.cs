using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NoahNPCGen
{
    public class LoadAPI
    {
        public JObject LoadJObj(string url)
        {
            WebRequest request = WebRequest.Create("https://www.dnd5eapi.co/api/" + url);
            request.Method = "GET";
            using var webStream = request.GetResponse().GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            return JObject.Parse(data);
        }

        public JArray LoadJArr(string url)
        {
            WebRequest request = WebRequest.Create("https://www.dnd5eapi.co/api/" + url);
            request.Method = "GET";
            using var webStream = request.GetResponse().GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            return JArray.Parse(data);
        }

        public Queue<int> LoadQuan()
        {
            WebRequest request = WebRequest.Create("https://qrng.anu.edu.au/API/jsonI.php?length=1000&type=uint16&size=0");
            request.Method = "GET";
            using var webStream = request.GetResponse().GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            Queue<int> result = new Queue<int>();
            for (int i = 0; i < 1000; i++)
                result.Enqueue((int)JObject.Parse(data)["data"][i]);
            return result;
        }
    }
}
