using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
namespace MovieBooker.Models
{
    public class Point
    {
        public string ID { get; set; }
        public string Occuredatetime { get; set; }
        public string Usedvalue { get; set; }
        public string Rechargedvalue { get; set; }
        public string Remainvalue { get; set; }

        static public List<Point> SqlDataReaderToMember(SqlDataReader Reader)
        {
            List<Point> Members = new List<Point>();
            try
            {
                while (Reader.Read())
                {
                    Point member = new Point();
                    member.ID = Reader["ID"].ToString();
                    member.Occuredatetime = Reader["Occuredatetime"].ToString();
                    member.Usedvalue = Reader["Usedvalue"].ToString();
                    member.Rechargedvalue = Reader["Rechargedvalue"].ToString();
                    member.Remainvalue = Reader["Remainvalue"].ToString();

                    Members.Add(member);
                }
            }
            catch
            {
                return null;
            }
            return Members;
        }

        static public List<string> GetMemberNameToSting()
        {
            List<string> members = new List<string>();

            members.Add("계정명");
            members.Add("변경일자");
            members.Add("사용금액");
            members.Add("충전금액");
            members.Add("잔여금액");

            return members;
        }
    }
}