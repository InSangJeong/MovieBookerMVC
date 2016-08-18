using MovieBooker.DAL;
using MovieBooker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieBooker.Controllers
{
    public class TheaterController : Controller
    {
        // GET: Theater
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TheaterList()
        {
            List<Theater> ALLTheaters = Theater_DAL.Select_Theater("", new List<Tuple<string, object>>());
            ViewBag.Theaters = ALLTheaters;
            return View();
        }
        [HttpPost]
        public ActionResult TheaterList(Theater theater)
        {
            List<Theater> ALLTheaters = Theater_DAL.Select_Theater("", new List<Tuple<string, object>>());
            ViewBag.Theaters = ALLTheaters;
            return View();
        }

        public ActionResult NewTheater(Theater theater)
        {
            if(theater == null || theater.Seatnumbercount == null || theater.Seatrowcount == null)
            {
                //TODO 값 확인 메시지
                return RedirectToAction("TheaterList", "Theater");
            }
            string Command = " order by LEN(TheaterID), TheaterID desc";

            List<Theater> Theaters = Theater_DAL.Select_Theater(Command, new List<Tuple<string, object>>());
            int NewTheaterID = 0;
            if (Theaters != null ||Theaters.Count != 0)
            {
                //상영관 ID는 기존 ID중 가장 큰 ID + 1
                NewTheaterID = Convert.ToInt32(Theaters[0].TheaterID) + 1;
            }
            Command = "insert into Theater(TheaterID, Seatcount, Seatrowcount, Seatnumbercount)" +
                       "values(@TheaterID, @Seatcount, @Seatrowcount, @Seatnumbercount)";
            //아스키 코드값을 이용하여 열의 갯수를 숫자로 변환하고 열당 좌석수 만큼 곱하여 총 좌석수를 지정한다.
            //(int)(Convert.ToChar(TextBox_Row.Text.ToUpper()) - 64);//이전버전
            int Int_row = Common.AlphabetToNumber(theater.Seatrowcount);
            int SeatCountByRow = Convert.ToInt32(theater.Seatnumbercount);

            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@TheaterID", NewTheaterID));
            Params.Add(new Tuple<string, object>("@Seatcount", Int_row * SeatCountByRow));
            Params.Add(new Tuple<string, object>("@Seatrowcount", theater.Seatrowcount.ToUpper()));
            Params.Add(new Tuple<string, object>("@Seatnumbercount", SeatCountByRow));

            if(!Theater_DAL.DoCommand(Command, Params))
            {
                //TODO 에러 메시지
                ;
            }

            return RedirectToAction("TheaterList", "Theater");
        }
    }
}