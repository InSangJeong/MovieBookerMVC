using MovieBooker.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.DAL
{
    public class Member_DAL
    {
        public static SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DBManager"].ToString());

        public static List<Member> Select_Member(string WhereString, List<Tuple<string, object>> Params)
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connect;
            //Set command
            cmd.CommandText = "Select * from Member" + WhereString;
            

            //Set params
            foreach (Tuple<string, object> param in Params)
            {
                cmd.Parameters.AddWithValue(param.Item1, param.Item2);
            }
            //Do read Data
            SqlDataReader reader = cmd.ExecuteReader();
            List<Member> members = Member.SqlDataReaderToMember(reader);
            //커넥트가 닫히면 값을 받아올수 없다.
            connect.Close();
            return members;
        }
    }
}