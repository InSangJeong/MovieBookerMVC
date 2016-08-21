using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace MovieBooker.Models
{
    public class Common
    {
        static public void ShowMessage(Page page, string script)
        {
            string scriptmsg = "alert('" + script + "');";
            ScriptManager.RegisterStartupScript(page, page.GetType(),
                                  "ServerControlScript", scriptmsg, true);
        }
        static public string CSDateTiemToASPDateTime(DateTime dateTime)
        {
            //코드는 간단한데 여기저기서 많이 불러서 공통으로 옮김.
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        //A = 1, B = 2 ..... Z = 26
        static public int AlphabetToNumber(String Alphabet)
        {
            Alphabet = Alphabet.Trim();
            return (int)(Convert.ToChar(Alphabet.ToUpper()) - 64);
        }
        //A = 1, B = 2 ..... Z = 26
        static public String NumberToAlphabet(int Number)
        {
            if (Number > 26 || Number <= 0)
                return "알파벳 범위 초과";
            //디버깅 안해봤는데 바로 String로 바꿔도 알파벳으로 변형되려나?
            String Alphabet = Convert.ToString(Convert.ToChar(Number + 64));

            return Alphabet;
        }
        //중복제거
        public T[] GetDistinctValues<T>(T[] array)
        {
            List<T> tmp = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                if (tmp.Contains(array[i]))
                    continue;
                tmp.Add(array[i]);
            }

            return tmp.ToArray();
        }
        static public string yyyyMMddHHmmss2DatetimeString(string yyyyMMddHHmmss)
        {
            DateTime sdate = DateTime.ParseExact(yyyyMMddHHmmss, "yyyyMMddHHmmss", null);
            return sdate.ToString("yyyy-MM-dd HH:mm:ss");

        }
    }
}