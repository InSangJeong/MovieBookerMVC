using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class Theater
    {
        public string TheaterID { get; set; }
        public string Seatcount { get; set; }
        public string Seatrowcount { get; set; }
        public string Seatnumbercount { get; set; }


        static public List<Theater> SqlDataReaderToMember(SqlDataReader Reader)
        {
            List<Theater> Theaters = new List<Theater>();
            try
            {
                while (Reader.Read())
                {
                    Theater member = new Theater();
                    member.TheaterID = Reader["TheaterID"].ToString();
                    member.Seatcount = Reader["Seatcount"].ToString();
                    member.Seatrowcount = Reader["Seatrowcount"].ToString();
                    member.Seatnumbercount = Reader["Seatnumbercount"].ToString();

                    Theaters.Add(member);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return Theaters;
        }
        static public List<string> OlnyTheaterID(SqlDataReader Reader)
        {
            List<string> Theaters = new List<string>();
            try
            {
                while (Reader.Read())
                {
                    string member = string.Empty;
                    member = Reader["TheaterID"].ToString();
                    Theaters.Add(member);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return Theaters;
        }

        static public List<string> GetMemberNameToSting()
        {
            List<string> members = new List<string>();

            members.Add("상영관ID");
            members.Add("총 좌석수");
            members.Add("마지막 열");
            members.Add("열당 좌석수");

            return members;
        }
    }
}