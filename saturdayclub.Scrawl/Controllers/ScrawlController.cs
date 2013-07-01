using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace saturdayclub.Scrawl.Controllers
{
    public class ScrawlController : Controller
    {
        //
        // POST: /Scrawl/
        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("scrawl")]
        public ActionResult Scrawl()
        {
            return View();
        }

    }
}
