using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBooker.Models;
using MovieBooker.DAL;
using System.Reflection;

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

            Movie NonSelectedValue = new Models.Movie();
            NonSelectedValue.MovieID = "None";
            NonSelectedValue.Moviename = "선택하세요.";
            ds.Movies.Add(NonSelectedValue);

            return View(ds);
        }


        [HttpPost]
        [MultipleButton(Name = "action", Argument = "DoBook")]
        public ActionResult DoBook(BookingSupporter inputData)
        {
            //TODO : 예약 객체 생성시 점검사항들 
            //   기본. 예매하는 당사자의 로그인정보를 확인한다.
            //      0. 예약 인원수를 찾아내고 해당좌석을 리스트로 만듭니다.
            //      1. 포인트가 사람수만큼 결제가능한지 확인합니다.
            //      2. 예약하는동안 선택한 좌석(들)이 예매가 되었는지 확인.
            //      3. 관람등급과 고객나이 확인
            //      4. 예매객체 생성
            //      5. 포인트 사용내역 객체 생성
            //      6. Seat의 예약상태를 True로 변경한다.
            //      7. 예약할경우 MovieSchedule의 RemainSeat값도 줄여줘야함.
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            Member LoginedMember = Session["MEMBER"] as Member;

            string Command = "";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            List<Movie> SelcetedMovieObject = null;
            int LastlyRemaindPoint = 0;
            List<Seat> TryBookSeats = new List<Seat>();
            #region 0
            foreach(Seat seat in inputData.Seats)
            {
                if(seat.IsNewbooked)
                {
                    TryBookSeats.Add(seat);
                }
            }
            if(TryBookSeats.Count == 0)
            {
                //에러
                return View();
            }
            #endregion
            #region 1
            Command = " WHERE ID=@id ORDER BY Occuredatetime DESC";
            Params.Add(new Tuple<string, object>("@id", LoginedMember.ID));

            List<Point> points = Point_DAL.Select_Points(Command, Params);
            if (points == null || points.Count == 0)
            {
                return View();
            }
            int totalPrice = TryBookSeats.Count * Constant.MoviePrice;
            if (Convert.ToInt32(points[0].Remainvalue) < totalPrice)
            {
                return View();
            }
            LastlyRemaindPoint = Convert.ToInt32(points[0].Remainvalue);
            Params.Clear();
            #endregion
            #region 2
            Command = " Where " +
              "TheaterID = @TheaterID AND Playtime = @Playtime AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber";
            foreach (Seat TryBookingSeat in TryBookSeats)
            {
                Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater));
                Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
                Params.Add(new Tuple<string, object>("@Seatrow", TryBookingSeat.Seatrow));
                Params.Add(new Tuple<string, object>("@Seatnumber", TryBookingSeat.Seatnumber));
                List<Seat> seat = Seat_DAL.Select_Seats(Command, Params);
                if (seat == null || seat.Count == 0 || seat[0].Isbooked)
                {
                    return View();
                }
                Params.Clear();
            }
            #endregion
            #region 3
            Command = " Where MovieID = @MovieID";
            Params.Add(new Tuple<string, object>("@MovieId", inputData.SelectedMovieID));
            SelcetedMovieObject = Movie_DAL.Select_Movie(Command, Params);
            if (SelcetedMovieObject == null || SelcetedMovieObject.Count == 0)
            {
                return View();
            }
            if (Convert.ToInt32(LoginedMember.Age) < Convert.ToInt32(SelcetedMovieObject[0].Viewingclass))
            {
                return View();
            }
            Params.Clear();
            #endregion
            #region 4
            Command = "insert into Booking(ID, MovieID, TheaterID, Playdatetime, Seatrow, Seatnumber, Moviename, Bookedcount, Viewingclass)" +
                    "values(@ID, @MovieID, @TheaterID, @Playdatetime, @Seatrow, @Seatnumber, @Moviename, @Bookedcount, @Viewingclass)";
            foreach (Seat TryBookingSeat in TryBookSeats)
            {
                Params.Add(new Tuple<string, object>("@ID", LoginedMember.ID.Trim()));
                Params.Add(new Tuple<string, object>("@MovieID", inputData.SelectedMovieID.Trim()));
                Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater.Trim()));
                Params.Add(new Tuple<string, object>("@Playdatetime", inputData.SelectedPlaytime));
                Params.Add(new Tuple<string, object>("@Seatrow", TryBookingSeat.Seatrow));
                Params.Add(new Tuple<string, object>("@Seatnumber", TryBookingSeat.Seatnumber));
                Params.Add(new Tuple<string, object>("@Moviename", SelcetedMovieObject[0].Moviename));
                Params.Add(new Tuple<string, object>("@Bookedcount", TryBookSeats.Count));
                Params.Add(new Tuple<string, object>("@Viewingclass", SelcetedMovieObject[0].Viewingclass));
                if (!Bookinginfo_DAL.DoCommand(Command, Params))
                {
                    return View();
                }
                Params.Clear();
            }
            #endregion
            #region 5
            Command = "insert into Point(ID, Occuredatetime, Usedvalue, Rechargedvalue, Remainvalue)" +
                    "values(@ID, @Occuredatetime, @Usedvalue, @Rechargedvalue, @Remainvalue)";
            Params.Add(new Tuple<string, object>("@ID", LoginedMember.ID));
            Params.Add(new Tuple<string, object>("@Occuredatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            Params.Add(new Tuple<string, object>("@Usedvalue", TryBookSeats.Count * Constant.MoviePrice));
            Params.Add(new Tuple<string, object>("@Rechargedvalue", 0));
            Params.Add(new Tuple<string, object>("@Remainvalue", LastlyRemaindPoint - TryBookSeats.Count * Constant.MoviePrice));
            if (!Point_DAL.DoCommand(Command, Params))
            {
                return View();
            }
            Params.Clear();
            #endregion
            #region 6
            Command = Command = "Update Seat Set Isbooked = @Isbooked " +
                "Where TheaterID = @TheaterID AND Seatrow = @Seatrow AND Seatnumber = @Seatnumber AND Playtime = @Playtime";
            foreach (Seat TryBookingSeat in TryBookSeats)
            {
                Params.Add(new Tuple<string, object>("@Isbooked", "True"));
                Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater));
                Params.Add(new Tuple<string, object>("@Seatrow", TryBookingSeat.Seatrow));
                Params.Add(new Tuple<string, object>("@Seatnumber", TryBookingSeat.Seatnumber));
                Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
                if (!Seat_DAL.DoCommand(Command, Params))
                {
                    return View();
                }
                Params.Clear();
            }
            #endregion
            #region 7
            //일단 기존남은 좌석 데이터를 가져와야함.
            int RemainSeatCount = 0;
            Command = "Where MovieID = @MovieId AND TheaterID = @TheaterID AND Playtime = @Playtime";
            Params.Add(new Tuple<string, object>("@MovieId", inputData.SelectedMovieID));
            Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater));
            Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
            List<Movieschedule> movieschedule = Movieschedule_DAL.Select_MovieSC(Command, Params);
            if (movieschedule == null || movieschedule.Count != 1)
                return View();
            RemainSeatCount = Convert.ToInt32(movieschedule[0].Seatremained);
            if (RemainSeatCount == 0)
                return View();

            Params.Clear();
            Command = "Update Movieschedule Set Seatremained = @Seatremained " +
                "Where MovieID = @MovieId AND TheaterID = @TheaterID AND Playtime = @Playtime";
            Params.Add(new Tuple<string, object>("@Seatremained", RemainSeatCount - TryBookSeats.Count));
            Params.Add(new Tuple<string, object>("@MovieId", inputData.SelectedMovieID));
            Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater));
            Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
            if (!Movieschedule_DAL.DoCommand(Command, Params))
            {
                return View();
            }
            #endregion
            return RedirectToAction("BookingList", "Book");
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Booking")]
        public ActionResult Booking(BookingSupporter inputData)
        {
            //사용자가 영화예매에 관련된 데이터들을 입력하였을때
            if (inputData.SelectedMovieID != null || inputData.SelectedMovieID != "None")
            {
                //사용자가 영화를 선택하였기 때문에 상영날짜데이터를 추가하여 뷰에 도시한다.
                List<Movie> Movies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
                inputData.Movies = Movies;

                string where = " Where MovieID=@MovieID";
                List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
                Params.Add(new Tuple<string, object>("@MovieID", inputData.SelectedMovieID));
                List<Movieschedule> MovieSC = Movieschedule_DAL.Select_MovieSC(where, Params);
                inputData.movieSchedules = MovieSC;

                if (!string.IsNullOrEmpty(inputData.SelectedPlaytime))
                {
                    //날짜 선택했으니 날짜에 맞는 상영관만 간추려서 보낸다.
                    where = " Where @MovieID = MovieID AND @Playtime = Playtime";
                    Params.Clear();
                    Params.Add(new Tuple<string, object>("@MovieID", inputData.SelectedMovieID));
                    Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
                    inputData.SelectedmovieSchedules = Movieschedule_DAL.Select_MovieSC(where, Params);

                    if (!string.IsNullOrEmpty(inputData.SelectedTheater))
                    {
                        //영화명, 상영날짜, 상영관을 모두 선택하였으므로 상영관 데이터를 보내 값을 checkbox를 유지하도록한다.
                        where = " Where @TheaterID = TheaterID AND @Playtime = Playtime Order by Seatrow, Seatnumber";
                        Params.Clear();
                        Params.Add(new Tuple<string, object>("@TheaterID", inputData.SelectedTheater));
                        Params.Add(new Tuple<string, object>("@Playtime", inputData.SelectedPlaytime));
                        inputData.Seats = Seat_DAL.Select_Seats(where, Params);
                    }
                    else
                    {
                        Movieschedule NonSelectedValue = new Models.Movieschedule();
                        NonSelectedValue.RemaindSeatMent = "선택하세요.";
                        NonSelectedValue.TheaterID = "None";
                        inputData.SelectedmovieSchedules.Add(NonSelectedValue);
                    }
                }
                else
                {
                    Movieschedule NonSelectedValue = new Models.Movieschedule();
                    NonSelectedValue.Playtime = "선택하세요.";
                    inputData.movieSchedules.Add(NonSelectedValue);
                }
                return View(inputData);
            }
            return View();
        }
    }

}
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MultipleButtonAttribute : ActionNameSelectorAttribute
{
    public string Name { get; set; }
    public string Argument { get; set; }

    public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
    {
        var isValidName = false;
        var keyValue = string.Format("{0}:{1}", Name, Argument);
        var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

        if (value != null)
        {
            controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
            isValidName = true;
        }

        return isValidName;
    }
}