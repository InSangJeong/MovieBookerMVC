using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class Member
    {
        public String ID { get; set; }
        public String Pass { get; set; }
        public String Name { get; set; }
        public String Age { get; set; }
        public String Birthday { get; set; }
        public String Sex { get; set; }
        public String Point { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }

        static public List<Member> SqlDataReaderToMember(SqlDataReader Reader)
        {
            List<Member> Members = new List<Member>();
            try
            {
                while (Reader.Read())
                {
                    Member member = new Member();
                    member.ID = Reader["ID"].ToString();
                    member.Pass = Reader["Pass"].ToString();
                    member.Name = Reader["Name"].ToString();
                    member.Age = Reader["Age"].ToString();
                    member.Birthday = Reader["Birthday"].ToString();
                    member.Sex = Reader["Sex"].ToString();
                    member.Point = Reader["Point"].ToString();
                    member.Address = Reader["Address"].ToString();
                    member.Phone = Reader["Phone"].ToString();

                    Members.Add(member);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return Members;
        }
        static public List<string> GetMemberNameToSting()
        {
            List<string> members = new List<string>();

            members.Add("ID");
            members.Add("비밀번호");
            members.Add("이름");
            members.Add("나이");
            members.Add("생일");
            members.Add("성별");
            members.Add("잔여포인트");
            members.Add("주소");
            members.Add("연락처");

            return members;
        }
    }

}