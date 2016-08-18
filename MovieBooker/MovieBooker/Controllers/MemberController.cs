using MovieBooker.DAL;
using MovieBooker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieBooker.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MemberList()
        {
            List<Member> ALLMembers = Member_DAL.Select_Member("", new List<Tuple<string, object>>());
            ViewBag.Members = ALLMembers;
            return View();
        }

        public ActionResult RemoveMember(Member member)
        {
            //회원 삭제
            String CommandString = "Delete from Member Where ID=@ID";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@ID", member.ID.Trim()));
            if(!Member_DAL.DoCommand(CommandString, Params))
            {
                //TODO Common.ShowMessage(, @"회원 삭제를 수행하지 못하였습니다.");
            }
            //회원 삭제되고나서 새로 갱신된 화면을 띄운다.
            List<Member> ALLMembers = Member_DAL.Select_Member("", new List<Tuple<string, object>>());
            ViewBag.Members = ALLMembers;
            return RedirectToAction("MemberList", "Member");
        }
    }
}