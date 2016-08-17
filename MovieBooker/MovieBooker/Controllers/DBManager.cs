using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class DBManager
    {
        public SqlConnection dbConnection;
        public SqlCommand dbCommand;
        public SqlDataReader dbDataReader;
        public DBManager(string DBlink)
        {
            dbConnection = new SqlConnection(DBlink);
            try
            {
                dbConnection.Open();
                dbCommand = new SqlCommand();
                dbCommand.Connection = dbConnection;
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR : DBConnect (" + e.Message + ")");
            }
        }
        private DBManager()
        {
            ;
        }

        //DB에 추가 삭제 변경 명령을 내리는 함수입니다.
        public bool DoCommand(String Command, List<Tuple<string, object>> Params)
        {
            try
            {
                //이전에 내렸던 명령이 있으면 삭제한다.
                if (dbDataReader != null)
                {
                    dbDataReader.Close();
                }
                dbCommand.CommandText = string.Empty;
                dbCommand.Parameters.Clear();
                //새로운 명령값을 셋팅한다.
                dbCommand.CommandText = Command;
                foreach (Tuple<string, object> param in Params)
                {
                    dbCommand.Parameters.AddWithValue(param.Item1, param.Item2);
                }
                //명령 실행 
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("DB Error : " + e.Message);
                return false;
            }
            return true;
        }
        //DB에서 데이터를 받아오는 함수입니다.
        public SqlDataReader GetDataList(String Command, List<Tuple<string, object>> Params)
        {
            try
            {
                //이전에 내렸던 명령이 있으면 삭제한다.
                if (dbDataReader != null)
                {
                    dbDataReader.Close();
                }

                dbCommand.CommandText = string.Empty;
                dbCommand.Parameters.Clear();
                //새로운 명령값을 셋팅한다.
                dbCommand.CommandText = Command;
                foreach (Tuple<string, object> param in Params)
                {
                    dbCommand.Parameters.AddWithValue(param.Item1, param.Item2);
                }
                //명령 실행 
                dbDataReader = dbCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine("DB Error : " + e.Message);
                return null;
            }
            //datareader 객체를 리턴하니 각 페이지에서 Dataset에서 맞게 셋팅해서 쓰면된다.
            return dbDataReader;
        }
        //DB연결 객체를 해제하는 함수입니다.
        public void Disconnet()
        {
            this.dbConnection.Close();
        }
    }
}