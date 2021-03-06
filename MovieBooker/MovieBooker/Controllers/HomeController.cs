﻿using MovieBooker.DAL;
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
            return RedirectToAction("Home", "Home");
        }

        public ActionResult Home()
        {
            if (Session["MEMBER"] != null)
            {
                Member member = (Member)Session["MEMBER"];
                if (member.ID == "admin")
                {
                    return RedirectToAction("loginAdmin", "Home");
                }
                else
                {
                    return RedirectToAction("loginMember", "Home");
                }
            }
            
            return View();
        }
        [HttpPost]
        public ActionResult Home(LoginM member)
        {
            if (Session["MEMBER"] != null)
            {
                Member loginedMember = (Member)Session["MEMBER"];
                if (loginedMember.ID == "admin")
                {
                    return RedirectToAction("loginAdmin", "Home");
                }
                else
                {
                    return RedirectToAction("loginMember", "Home");
                }
            }

            if (ModelState.IsValid)
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
                            return RedirectToAction("loginAdmin", "Home");
                        }
                        else
                        {
                            return RedirectToAction("loginMember", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "계정이나 비밀번호를 확인하세요.");
                        return View(member);
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
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            return View();
        }


        public ActionResult loginMember()
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");

            List<Movie> Movies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
            ViewBag.ViewMovie = Movies;
            return View();
        }
        [HttpPost]
        public ActionResult loginMember(Member member)
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
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