using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieBooker.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        //public ActionResult Index()
        {
            //return View();
            return "Hellow world";
        }
    }
}