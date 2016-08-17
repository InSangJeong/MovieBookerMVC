using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class Bookinginfo
    {
        public String ID { get; set; }
        public String MovieID { get; set; }
        public String TheaterID { get; set; }
        public String Playdatetime { get; set; }
        public String Seatrow { get; set; }
        public String Seatnumber { get; set; }
        public String Moviename { get; set; }
        public String Bookedcount { get; set; }
        public String Viewingclass { get; set; }

        static public List<Bookinginfo> SqlDataReaderToBooking(SqlDataReader Reader)
        {
            List<Bookinginfo> bookings = new List<Bookinginfo>();
            try
            {
                while (Reader.Read())
                {
                    Bookinginfo Booking = new Bookinginfo();
                    Booking.ID = Reader["ID"].ToString().Trim();
                    Booking.MovieID = Reader["MovieID"].ToString().Trim();
                    Booking.TheaterID = Reader["TheaterID"].ToString().Trim();
                    Booking.Playdatetime = Reader["Playdatetime"].ToString().Trim();
                    Booking.Seatrow = Reader["Seatrow"].ToString();
                    Booking.Seatnumber = Reader["Seatnumber"].ToString().Trim();
                    Booking.Moviename = Reader["Moviename"].ToString().Trim();
                    Booking.Bookedcount = Reader["Bookedcount"].ToString();
                    Booking.Viewingclass = Reader["Viewingclass"].ToString();

                    bookings.Add(Booking);
                }
            }
            catch
            {
                return null;
            }
            return bookings;
        }

    }
}