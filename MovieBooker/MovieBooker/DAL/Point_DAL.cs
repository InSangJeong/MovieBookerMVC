﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MovieBooker.Models;
using System.Configuration;

namespace MovieBooker.DAL
{
    public class Point_DAL
    {
        public static SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DBManager"].ToString());

        public static List<Point> Select_Points(string WhereString, List<Tuple<string, object>> Params)
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connect;
            //Set command
            cmd.CommandText = "Select * from Point " + WhereString;


            //Set params
            foreach (Tuple<string, object> param in Params)
            {
                cmd.Parameters.AddWithValue(param.Item1, param.Item2);
            }
            //Do read Data
            SqlDataReader reader = cmd.ExecuteReader();
            List<Point> Points = Point.SqlDataReaderToMember(reader);
            //커넥트가 닫히면 값을 받아올수 없다.
            connect.Close();
            return Points;
        }
        public static bool DoCommand(string CommandString, List<Tuple<string, object>> Params)
        {
            try
            {
                connect.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connect;
                //이전에 내렸던 명령이 있으면 삭제한다.
                cmd.CommandText = string.Empty;
                cmd.Parameters.Clear();
                //새로운 명령값을 셋팅한다.
                cmd.CommandText = CommandString;
                foreach (Tuple<string, object> param in Params)
                {
                    cmd.Parameters.AddWithValue(param.Item1, param.Item2);
                }
                //명령 실행 
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                connect.Close();
                Console.WriteLine("DB Error : " + e.Message);
                return false;
            }
            connect.Close();
            return true;
        }
    }
}