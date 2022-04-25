using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Syncfusion.Blazor;
using Newtonsoft.Json.Linq;

namespace NoahNPCGen.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, dynamic> LoadAPI(string url)
        {
            WebRequest request = WebRequest.Create("https://www.dnd5eapi.co/api/" + url);
            request.Method = "GET";
            using var webStream = request.GetResponse().GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            Console.WriteLine("HERE IS THE ISSUE: " + NameAPI().ToString());

            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);
        }
        public static string NameAPI()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://api.fungenerators.com/name/categories.json?start=0&limit=5");
                request.Method = "GET";
                using var webStream = request.GetResponse().GetResponseStream();

                using var reader = new StreamReader(webStream);
                var data = reader.ReadToEnd();

                return JObject.Parse(data).ToString();
            }
            catch (WebException e)
            {
                string pageContent = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
                return pageContent;
            }
        }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public IActionResult OnPost(string selectName, string selectRace, string selectClass, string selectSubclass, int selectLevel, string selectBackG, string selectAlignment)
        {
            return RedirectToPage("Character", "SingleOrder", new { charName = selectName, charRace = selectRace, charClass = selectClass, charSubClass = selectSubclass, charLevel = selectLevel, charBackG = selectBackG, charAlignment = selectAlignment });
        }
        public void OnGet()
        {

        }

    }
}
