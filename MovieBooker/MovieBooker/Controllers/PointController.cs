using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBooker.Models;
using MovieBooker.DAL;

namespace MovieBooker.Controllers
{
    public class PointController : Controller
    {
        // GET: Point
        public ActionResult Index()
        {
            
            return View();
        }
        //포인트 사용내역화면을 관리하는 컨트롤러입니다.
        //세션에 있는 로그인멤버 정보를 이용하여 포인트 사용내역을 DB에서 가져오고
        //ViewBag에 포인트내역을 전달하여 화면에 도시하도록 합니다.
        public ActionResult RechargePoint()
        {
            if(Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            Member LoginedMember = Session["MEMBER"] as Member;
            string where = " Where ID=@ID ORDER BY Occuredatetime ASC";
            List < Tuple < string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@ID", LoginedMember.ID.Trim()));
            List<Point> points = Point_DAL.Select_Points(where, Params);
            ViewBag.Points = points;

            return View();
        }
        [HttpPost]
        public ActionResult RechargePoint(Point RechargePoint)
        {
            //1. 세션 존재 확인.
            //2. 잔여 포인트 내역을 받아와야함.
            //3. 포인트 충전(DB에 전송)
            //4. DB에 적용된 결과를 화면에 도시
            //1
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            Member LoginedMember = Session["MEMBER"] as Member;
            //2
            string where = " Where ID=@ID ORDER BY Occuredatetime ASC";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@ID", LoginedMember.ID.Trim()));
            List<Point> points = Point_DAL.Select_Points(where, Params);
            int RecentlyRemainPoint = 0;
            if (points.Count > 0)
            {
                RecentlyRemainPoint = Convert.ToInt32(points.Last().Remainvalue.Trim());
            }
            
            //3
            Params.Clear();
            string Command = "insert into Point(ID, Occuredatetime, Usedvalue, Rechargedvalue, Remainvalue)" +
                        "values(@ID, @Occuredatetime, @Usedvalue, @Rechargedvalue, @Remainvalue)";

            Params.Add(new Tuple<string, object>("@id", LoginedMember.ID));
            Params.Add(new Tuple<string, object>("@Occuredatetime", Common.CSDateTiemToASPDateTime(DateTime.Now)));
            Params.Add(new Tuple<string, object>("@Usedvalue", 0));
            Params.Add(new Tuple<string, object>("@Rechargedvalue", RechargePoint.Rechargedvalue));
            Params.Add(new Tuple<string, object>("@Remainvalue", RecentlyRemainPoint + Convert.ToInt32(RechargePoint.Rechargedvalue)));

            Point_DAL.DoCommand(Command, Params);
            //4
            Params.Clear();
            Params.Add(new Tuple<string, object>("@ID", LoginedMember.ID.Trim()));
            points = Point_DAL.Select_Points(where, Params);
            ViewBag.Points = points;

            return View();
        }
    }
}