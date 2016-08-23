using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace MovieBooker.Models
{
    public class Seat
    {
        public string TheaterID { get; set; }
        public string Seatrow { get; set; }
        public string Seatnumber { get; set; }
        public bool Isbooked { get; set; }
        //예약 화면에서 사용자가 예약을 시도했는지 확인하는 필드입니다.
        public bool IsNewbooked { get; set; }
        public string Playtime { get; set; }


        static public List<Seat> SqlDataReaderToSeat(SqlDataReader Reader)
        {
            List<Seat> Seats = new List<Seat>();
            try
            {
                while (Reader.Read())
                {
                    Seat seat = new Seat();
                    seat.TheaterID = Reader["TheaterID"].ToString().Trim();
                    seat.Seatrow = Reader["Seatrow"].ToString().Trim();
                    seat.Seatnumber = Reader["Seatnumber"].ToString().Trim();
                    seat.Isbooked = Convert.ToBoolean(Reader["Isbooked"]);
                    seat.IsNewbooked = Convert.ToBoolean(Reader["Isbooked"]);

                    seat.Playtime = Reader["Playtime"].ToString();

                    Seats.Add(seat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return Seats;
        }
    }
}