﻿using System.Web;
using System.Web.Mvc;

namespace saturdayclub.Scrawl
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}