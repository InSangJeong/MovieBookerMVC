using MovieBooker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBooker.Controllers
{
    public class Valid
    {
        static public ValidResult isValid(MovieAndTheater UserInputData, HttpPostedFileBase file)
        {
            bool validResult = true;
            string resultMsg = "";
            if (file == null)
                validResult = false;
            //requried
            if (String.IsNullOrEmpty(UserInputData.movie.Moviename) ||
                String.IsNullOrEmpty(UserInputData.theater.TheaterID) ||
                String.IsNullOrEmpty(UserInputData.movie.Viewingclass) ||
                String.IsNullOrEmpty(UserInputData.movie.Runningtime) ||
                String.IsNullOrEmpty(UserInputData.PlayDayTimes)
                )
            {
                validResult = false;
                resultMsg = "필수 항목중 미입력된 항목이 있습니다.";
            }
            else
            {
                //상영시간이 상영시작일시와 종료일시 사이에 존재하는지
                string[] stringSeparators = new string[] { "\r\n" };
                string[] playtimes = UserInputData.PlayDayTimes.Split(stringSeparators, StringSplitOptions.None);
                string formatString = "yyyyMMddHHmmss";
                DateTime StartdateTime = DateTime.ParseExact(UserInputData.movie.Playstartdatetime, formatString, null);
                DateTime EnddateTime = DateTime.ParseExact(UserInputData.movie.Playenddatetime, formatString, null);
                foreach (string playtime in playtimes)
                {
                    if (playtime == "")
                        continue;
                    DateTime Chkplaytime = DateTime.ParseExact(playtime, formatString, null);
                    if (StartdateTime > Chkplaytime || EnddateTime < Chkplaytime)
                    {
                        validResult = false;
                        resultMsg = "상영목록중 상영날짜에 포함할수 없는 항목이 있습니다.";
                    }
                }
            }
            return new ValidResult(validResult, resultMsg);
        }

    }
    public class ValidResult
    {
        public bool Result { get; set; }
        public string FailMessage { get; set; }
        public ValidResult(bool Result, string FailMessage = "")
        {
            this.Result = Result;
            this.FailMessage = FailMessage;
        }
    }
}