using MovieBooker.DAL;
using MovieBooker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieBooker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Main()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult loginAdmin()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult loginAdmin(List<Member> loginedMember)
        {


            return View();
        }

    }
}