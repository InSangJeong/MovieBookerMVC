using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class Member
    {
        [Required]
        [Display(Name = "사용자 계정")]
        public String ID { get; set; }
        [Required]
        [Display(Name = "비밀번호")]
        public String Pass { get; set; }
        [Required]
        [Display(Name = "확인용 비밀번호")]
        [Compare("Pass", ErrorMessage ="암호와 확인암호가 일치하지 않습니다.")]
        public String CheckPass { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        [Range(1,150)]
        public String Age { get; set; }
        
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public String Birthday { get; set; }
        [Required]
        [Range(1, 4)]
        [StringLength(1, MinimumLength = 1)]
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

        static public bool DoCheckPass(Member member)
        {
            if (member.Pass == member.CheckPass)
                return true;
            else
                return false;
        }
    }

}