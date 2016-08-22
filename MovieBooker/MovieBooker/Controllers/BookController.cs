using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBooker.Models;
using MovieBooker.DAL;

namespace MovieBooker.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }
        //로그인한 멤버의 예매 내역을 도시하는 화면을 제어하는 컨트롤러.
        public ActionResult BookingList()
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            Member LoginedMember = Session["MEMBER"] as Member;

            string where = "Where id=@id";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@id", LoginedMember.ID));

            List<Bookinginfo> bookinfos = Bookinginfo_DAL.Select_Bookinfos(where, Params);
            ViewBag.Bookings = bookinfos;
            return View();
        }

        //고객의 요청에 의하여 해당 예매를 취소하고 최신화된 영화예매 리스트를 반환하는 컨트롤러.
        public ActionResult RemoveBooking(Bookinginfo bookinfo)
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            Member LoginedMember = Session["MEMBER"] as Member;
            //TODO : Movie Schedule의 RemianSeat 반환
            //       Seat에서 예약상태 변경
            //       예약 내용 제거
            //       환불
            int refundCount = 0;
            string RefundID = "";
            string Command = "";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();

            #region 예약 내용 Get
            Command = " Where ID = @ID AND MovieID = @MovieID AND TheaterID = @TheaterID AND " +
                "Playdatetime = @Playdatetime AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber";
            RefundID = bookinfo.ID;
            String PlayTime = DateTime.ParseExact(bookinfo.Playdatetime, "yyyy-MM-dd tt h:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
            Params.Add(new Tuple<string, object>("@ID", bookinfo.ID));
            Params.Add(new Tuple<string, object>("@MovieID", bookinfo.MovieID));
            Params.Add(new Tuple<string, object>("@TheaterID", bookinfo.TheaterID));
            Params.Add(new Tuple<string, object>("@Playdatetime", PlayTime));
            Params.Add(new Tuple<string, object>("@Seatrow", bookinfo.Seatrow));
            Params.Add(new Tuple<string, object>("@Seatnumber", bookinfo.Seatnumber));

            List<Bookinginfo> BookingInfo = Bookinginfo_DAL.Select_Bookinfos(Command, Params);
            Params.Clear();
            #endregion

            #region MovieSchedule 테이블에서 RemainSeat 반환
            //
            //   Update example
            //   Command = Command = "Update Seat Set Isbooked = @Isbooked " +
            //   "Where TheaterID = @TheaterID AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber AND Playtime = @Playtime";
            Command = "Update Movieschedule Set Seatremained = Seatremained+1" +
                        "Where MovieID = @MovieID AND TheaterID = @TheaterID AND Playtime = @Playtime";

            Params.Add(new Tuple<string, object>("@MovieID", BookingInfo[0].MovieID));
            Params.Add(new Tuple<string, object>("@TheaterID", BookingInfo[0].TheaterID));
            Params.Add(new Tuple<string, object>("@Playtime", PlayTime));
            Movieschedule_DAL.DoCommand(Command, Params);
            Params.Clear();
            #endregion

            #region Seat테이블에서 Seat 반환
            Command = "Update Seat Set Isbooked = 0" +
                        "Where TheaterID = @TheaterID AND Playtime = @Playtime AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber ";
            Params.Add(new Tuple<string, object>("@TheaterID", BookingInfo[0].TheaterID));
            Params.Add(new Tuple<string, object>("@Playtime", PlayTime));
            Params.Add(new Tuple<string, object>("@Seatrow", BookingInfo[0].Seatrow));
            Params.Add(new Tuple<string, object>("@Seatnumber", BookingInfo[0].Seatnumber));
            Seat_DAL.DoCommand(Command, Params);
            Params.Clear();
            #endregion

            #region 예약 내용 제거
            // Delete Example
            // string Command = "DELETE FROM Member where ID=@id";
            Command = "Delete From Booking Where ID = @ID AND MovieID = @MovieID AND TheaterID = @TheaterID AND " +
             "Playdatetime = @Playdatetime AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber";
            Params.Add(new Tuple<string, object>("@ID", bookinfo.ID));
            Params.Add(new Tuple<string, object>("@MovieID", bookinfo.MovieID));
            Params.Add(new Tuple<string, object>("@TheaterID", bookinfo.TheaterID));
            Params.Add(new Tuple<string, object>("@Playdatetime", PlayTime));
            Params.Add(new Tuple<string, object>("@Seatrow", bookinfo.Seatrow));
            Params.Add(new Tuple<string, object>("@Seatnumber", bookinfo.Seatnumber));
            Bookinginfo_DAL.DoCommand(Command, Params);
            Params.Clear();
            #endregion

            #region 환불
                Command = " WHERE ID=@id ORDER BY Occuredatetime DESC";
                Params.Add(new Tuple<string, object>("@id", RefundID));
                List<Point> points = Point_DAL.Select_Points(Command, Params);
                Params.Clear();

                Command = "Insert into Point(ID, Occuredatetime, Usedvalue, Rechargedvalue, Remainvalue)" +
                            "values(@ID, @Occuredatetime, @Usedvalue, @Rechargedvalue, @Remainvalue)";
                Params.Add(new Tuple<string, object>("@ID", RefundID));
                Params.Add(new Tuple<string, object>("@Occuredatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                Params.Add(new Tuple<string, object>("@Usedvalue", 0));
                Params.Add(new Tuple<string, object>("@Rechargedvalue", 0));
                Params.Add(new Tuple<string, object>("@Remainvalue", Convert.ToInt32(points[0].Remainvalue) + (Constant.MoviePrice)));
                Point_DAL.DoCommand(Command, Params);
            #endregion

            return RedirectToAction("BookingList", "Book");
        }

        public ActionResult Booking()
        {
            //if (Session["MEMBER"] == null)
            //    return RedirectToAction("Home", "Home");
            List<Movie> Movies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
            BookingSupporter ds = new BookingSupporter();
            ds.Movies = Movies;
            return View(ds);
        }
        [HttpPost]
        public ActionResult Booking(BookingSupporter inputData)
        {
            if(inputData.SelectedMovieID != null)
            {

                return View(inputData);
            }
            return RedirectToAction("Booking", "Book");
        }
    }
}