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

        public ActionResult Home()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Home(Member member)
        {
            //로그인 처리
            if (!string.IsNullOrEmpty(member.ID) && !string.IsNullOrEmpty(member.Pass))
            {
                string Wherestring = "Where ID=@ID AND Pass=@Pass";
                List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
                Params.Add(new Tuple<string, object>("@ID", member.ID));
                Params.Add(new Tuple<string, object>("@Pass", member.Pass));

                List<Member> loginedMember = Member_DAL.Select_Member(Wherestring, Params);
                if (loginedMember.Count() == 1)
                {
                    //TODO : Make Session.
                    Session["MEMBER"] = loginedMember[0];
                    if (member.ID == "admin")
                    {
                        return  RedirectToAction("loginAdmin", "Home");
                    }
                    else
                    {
                        return RedirectToAction("loginMember", "Home");
                    }
                }
            }
            return View();
        }

        public ActionResult NewMember()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewMember(Member member)
        {
            return View();
        }


        public ActionResult loginAdmin()
        {
            if(Session["MEMBER"] == null)
            {
                return RedirectToAction("Home", "Home");
            }

            return View();
        }
        [HttpPost]
        public ActionResult loginAdmin(Member member)
        {
            return View();
        }


        public ActionResult loginMember()
        {
            List<Movie> Movies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
            ViewBag.ViewMovie = Movies;
            return View();
        }
        [HttpPost]
        public ActionResult loginMember(Member member)
        {
            return View();
        }


        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Home", "Home");
        }
    }
}