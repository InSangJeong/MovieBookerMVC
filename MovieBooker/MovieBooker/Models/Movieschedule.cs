using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
namespace MovieBooker.Models
{
    public class Movieschedule
    {
        public string MovieID { get; set; }
        public string TheaterID { get; set; }
        public string Playtime { get; set; }
        public string Seatbooked { get; set; }//어디서 쓰이는지 모르겠네
        public string Seatremained { get; set; }

        //드롭다운리스트에 제가 지정한 형식대로 표현하기위한 필드입니다. 그 이외에는 사용하지마세요.
        public string RemaindSeatMent { get; set; }

        static public List<Movieschedule> SqlDataReaderToMember(SqlDataReader Reader)
        {
            List<Movieschedule> Schedules = new List<Movieschedule>();
            try
            {
                while (Reader.Read())
                {
                    Movieschedule Schedule = new Movieschedule();
                    Schedule.MovieID = Reader["MovieID"].ToString();
                    Schedule.TheaterID = Reader["TheaterID"].ToString();
                    Schedule.Playtime = DateTime.Parse(Reader["Playtime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    Schedule.Seatbooked = Reader["Seatbooked"].ToString();
                    Schedule.Seatremained = Reader["Seatremained"].ToString();

                    Schedule.RemaindSeatMent = Schedule.TheaterID + "(" + Schedule.Seatremained + ")";

                    Schedules.Add(Schedule);
                }
            }
            catch
            {
                return null;
            }
            return Schedules;
        }
    }
}