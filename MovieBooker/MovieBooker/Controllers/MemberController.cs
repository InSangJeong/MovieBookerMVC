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
        public ActionResult NewMember()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewMember(Member inputMember)
        {
            //추가할 회원 값 셋팅
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@ID", inputMember.ID));
            Params.Add(new Tuple<string, object>("@Pass", inputMember.Pass));
            Params.Add(new Tuple<string, object>("@Name", inputMember.Name));
            DateTime dt = DateTime.Now;
            int bornyear = Convert.ToInt32(inputMember.Birthday.Substring(0, 2));
            if (Convert.ToInt32(inputMember.Sex) > 2)
            {
                bornyear += 2000;
            }
            else
            {
                bornyear += 1900;
            }
            Params.Add(new Tuple<string, object>("@Age", dt.Year - bornyear));

            Params.Add(new Tuple<string, object>("@Birthday", inputMember.Birthday));
            bool Sex = false;
            if (inputMember.Sex == "1" || inputMember.Sex == "3")
            {
                Sex = true;
            }
            Params.Add(new Tuple<string, object>("@Sex", Sex));
            Params.Add(new Tuple<string, object>("@Point", 0));
            Params.Add(new Tuple<string, object>("@Address", inputMember.Address));
            Params.Add(new Tuple<string, object>("@Phone", inputMember.Phone));
            string Command = "insert into Member(ID, Pass, Name, Age, Birthday, Sex, Point, Address, Phone)" +
                "values(@ID, @Pass, @Name, @Age, @Birthday, @Sex, @Point, @Address, @Phone)";
            //추가 명령 전송
            if (Member_DAL.DoCommand(Command, Params))
            {
                //메인페이지 이동.
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("NewMember", "Member");
            }
        }
    }
}