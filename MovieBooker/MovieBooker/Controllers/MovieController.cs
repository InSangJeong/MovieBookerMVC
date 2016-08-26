using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBooker.Models;
using MovieBooker.DAL;
using System.IO;

namespace MovieBooker.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MovieList()
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            List<Movie> ALLMovies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
            ViewBag.Movies = ALLMovies;
            return View();
        }
        public ActionResult NewMovie()
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            ViewBag.ErrorMsg = "";
            return View();
        }
        [HttpPost]
        public ActionResult NewMovie(MovieAndTheater UserInputData, HttpPostedFileBase file)
        {
            if (Session["MEMBER"] == null)
                return RedirectToAction("Home", "Home");
            ValidResult vr = Valid.isValid(UserInputData, file);
            ViewBag.ErrorMsg = vr.FailMessage;
            if (!vr.Result)
            {
                //TODO : 유효성검사 실패
                //vr.FailMessage;
                
                return View();
            }
            //TODO List: 
            // 1. 새로운 영화의 ID를 지정하기위해 이전 영화의 MaxID 호출
            // 2. 영화등록
            // 3. 상영관의 좌석수를 얻기위해 값을 받아온다.
            // 4. 영화스케줄 등록
            // 5. 영화 스케쥴당 좌석 생성
            string command = " Order by MovieID DESC";
            String NewMovieID = "-1";
            String Seatcount = "0";
            String TheaterID = "0"; //상영관 ID
            String TheaterMaxRow = "0";//상영관 최대 열
            String TheaterMaxNumber = "0";//상영관 최대 행
            #region 1. 새로운 영화의 ID를 지정하기위해 이전 영화의 MaxID 호출
            List<Movie> movies = Movie_DAL.Select_Movie(command, new List<Tuple<string, object>>());
            if (movies == null || movies.Count == 0)
            {
                NewMovieID = "0";
            }
            else
            {
                //가장 큰 ID에 +1 해준값이 새로운 영화의 ID
                NewMovieID = (Convert.ToInt32(movies[0].MovieID.Trim(' ')) + 1).ToString();
            }
            #endregion
            #region 2. 영화등록
            command = "insert into Movie(MovieID, Moviename, Playstartdatetime, Playenddatetime, Runningtime, Viewingclass, Movieposter)" +
                    "values(@MovieID, @Moviename, @Playstartdatetime, @Playenddatetime, @Runningtime, @Viewingclass, @Movieposter)";
            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@MovieID", NewMovieID));
            Params.Add(new Tuple<string, object>("@Moviename", UserInputData.movie.Moviename));
            //yyyyMMddHHmmss To yyyy-MM-dd HH:mm:ss
            string formatString = "yyyyMMddHHmmss";
            Params.Add(new Tuple<string, object>("@Playstartdatetime", DateTime.ParseExact(UserInputData.movie.Playstartdatetime, formatString, null).ToString("yyyy-MM-dd HH:mm:ss")));
            Params.Add(new Tuple<string, object>("@Playenddatetime", DateTime.ParseExact(UserInputData.movie.Playenddatetime, formatString, null).ToString("yyyy-MM-dd HH:mm:ss")));
            Params.Add(new Tuple<string, object>("@Runningtime", UserInputData.movie.Runningtime));
            Params.Add(new Tuple<string, object>("@Viewingclass", UserInputData.movie.Viewingclass));

            file = Request.Files[0];
            var fileName = Path.GetFileName(UserInputData.movie.Movieposter);
            var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
            file.SaveAs(path);

            Params.Add(new Tuple<string, object>("@Movieposter", NewMovieID+"_"+ fileName));
            if (!Movie_DAL.DoCommand(command, Params))
            //if (!dbManager.DoCommand(command, Params))
            {
                Console.WriteLine("영화 추가 에러");
            }
            #endregion
            #region 3. 상영관의 좌석수를 얻기위해 값을 받아온다.
            Params.Clear();
            command = " Where TheaterID = @TheaterID";
            Params.Add(new Tuple<string, object>("@TheaterID", UserInputData.theater.TheaterID));
            List<Theater> Theaters = Theater_DAL.Select_Theater(command, Params);
            if (Theaters.Count != 1)
            {
                Console.WriteLine("같은 아이디의 극장이 여러개");
            }
            Seatcount = Theaters[0].Seatcount;
            TheaterMaxRow = Theaters[0].Seatrowcount;
            TheaterMaxNumber = Theaters[0].Seatnumbercount;
            #endregion
            #region 4. 영화스케줄 등록
            Params.Clear();
            string[] stringSeparators = new string[] { "\r\n" };
            string[] playtimes = UserInputData.PlayDayTimes.Split(stringSeparators, StringSplitOptions.None);

            foreach (string playtime in playtimes)
            {
                command = "insert into Movieschedule(MovieID, TheaterID, Playtime, Seatbooked, Seatremained)" +
                   "values(@MovieID, @TheaterID, @Playtime, @Seatbooked, @Seatremained)";
                if (playtime == "")
                    continue;
                Params.Add(new Tuple<string, object>("@MovieID", NewMovieID));
                Params.Add(new Tuple<string, object>("@TheaterID", TheaterID));
                Params.Add(new Tuple<string, object>("@Seatbooked", "0"));
                Params.Add(new Tuple<string, object>("@Seatremained", Seatcount));
                Params.Add(new Tuple<string, object>("@Playtime", DateTime.ParseExact(playtime, formatString, null).ToString("yyyy-MM-dd HH:mm:ss")));

                //영화스케쥴 생성에 성공하였으니 5번:영화관스케쥴당좌석을 생성한다.
                if (Movieschedule_DAL.DoCommand(command, Params))
                {
                    #region 5. 영화 스케쥴당 좌석 생성
                    command = "insert into Seat(TheaterID, Seatrow, Seatnumber, Isbooked, Playtime)" +
                   "values(@TheaterID, @Seatrow, @Seatnumber, @Isbooked, @Playtime)";
                    //A = 1, B = 2 ..... Z = 26
                    int I_TheaterMaxRow = Common.AlphabetToNumber(TheaterMaxRow);
                    int I_TheaterMaxNumber = Convert.ToInt32(TheaterMaxNumber);
                    for (int Seatrow = 1; Seatrow <= I_TheaterMaxRow; Seatrow++)//좌석 열수만큼
                    {
                        for (int Seatnumber = 1; Seatnumber <= I_TheaterMaxNumber; Seatnumber++)//열당 좌석수만큼
                        {
                            Params.Clear();
                            Params.Add(new Tuple<string, object>("@TheaterID", TheaterID));
                            Params.Add(new Tuple<string, object>("@Seatrow", Common.NumberToAlphabet(Seatrow)));
                            Params.Add(new Tuple<string, object>("@Seatnumber", Seatnumber));
                            Params.Add(new Tuple<string, object>("@Isbooked", "false"));
                            Params.Add(new Tuple<string, object>("@Playtime", DateTime.ParseExact(playtime, formatString, null).ToString("yyyy-MM-dd HH:mm:ss")));
                            if(!Seat_DAL.DoCommand(command, Params))
                            {
                                Console.Write("error");
                            }
                        }
                    }

                    #endregion
                }
                Params.Clear();
            }
            #endregion

            return RedirectToAction("MovieList", "Movie");
        }
        public ActionResult SelectTheater(string StartDatetime, string EndDatetime)
        {
            //TODO 가용상영관 검색 코드 추가.
          string Command = " WHERE Playstartdatetime <= @end AND Playenddatetime >= @start";


            List<Tuple<string, object>> Params = new List<Tuple<string, object>>();
            Params.Add(new Tuple<string, object>("@start", Common.yyyyMMddHHmmss2DatetimeString(StartDatetime)));
            Params.Add(new Tuple<string, object>("@end", Common.yyyyMMddHHmmss2DatetimeString(EndDatetime)));
            List<Movie> Movies = Movie_DAL.Select_Movie(Command, Params);

            List<Theater> AbleTheaters = new List<Theater>();
            // DEBUG : 시간 겹치는 영화만 나오는지 확인해
            if (Movies == null || Movies.Count == 0)
            {
                //기간이 겹치는 영화가 없으므로 모든 상영관을 리턴한다.
                AbleTheaters = Theater_DAL.Select_Theater("", new List<Tuple<string, object>>());
            }
            else
            {
                //기간이 겹치는 영화들이 있다.
                //1.영화 스케쥴에 사용되고있는 상영관 리스트를 추출

                //명령어 셋팅
                Command = " WHERE MovieID IN (";
                int i = 1;
                foreach (var movie in Movies)
                {
                    if (Movies.Count != i)
                        Command += "@movieID" + Convert.ToString(i) + ", ";
                    else
                        Command += "@movieID" + Convert.ToString(i);
                    i++;
                }
                Command += ")";
                //명령어에 해당하는 변수 설정.
                Params = new List<Tuple<string, object>>();
                i = 1;
                foreach (var movie in Movies)
                {
                    //DB에서 값 가져올때 공백이 생겨서 공백 제거를합니다.
                    Movies[i - 1].MovieID = Movies[i - 1].MovieID.Trim(' ');
                    Params.Add(new Tuple<string, object>("@movieID" + Convert.ToString(i), Movies[i - 1].MovieID));
                    i++;
                }
                List<Movieschedule> MovieSc = Movieschedule_DAL.Select_MovieSC(Command, Params);
                var listDistinct = MovieSc.GroupBy(mvsc => mvsc.TheaterID, (key, group) => group.First()).ToList();
                //뽑아온 곳은 영화 스케쥴 테이블인데 영화관 객체에 넣으면 안되기때문에 영화관 ID만 받아서 리스트로 생성한다.

                //2. 1.에서 뽑은 상영관을 전체 상영관에서 제거한다.
                Params = new List<Tuple<string, object>>();
                Command = " WHERE TheaterID NOT IN (";
                
                foreach (var Theater in listDistinct)
                {
                    if (!listDistinct.Last().Equals(Theater))
                        Command += "@TheaterID" + Theater.TheaterID.Trim().ToString() + ", ";
                    else
                        Command += "@TheaterID" + Theater.TheaterID.Trim().ToString();
                    Params.Add(new Tuple<string, object>("@TheaterID" + Theater.TheaterID.Trim().ToString(), Theater.TheaterID.Trim()));
                }
                Command += ")";

                AbleTheaters = Theater_DAL.Select_Theater(Command, Params);
            }

            //TODO DataSet으로만 보내줘야하나?


            return View(AbleTheaters);
        }

    }
}