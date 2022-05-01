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
using Newtonsoft.Json.Linq;

namespace NoahNPCGen.Pages
{
    public class IndexModel : PageModel
    {
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
