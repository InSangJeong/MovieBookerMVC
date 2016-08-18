using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class Movie
    {
        public string MovieID { get; set; }
        public string Moviename { get; set; }
        public string Playstartdatetime { get; set; }
        public string Playenddatetime { get; set; }
        public string Runningtime { get; set; }
        public string Viewingclass { get; set; }
        public string Movieposter { get; set; }

        static public List<Movie> SqlDataReaderToMember(SqlDataReader Reader)
        {
            List<Movie> Movies = new List<Movie>();
            try
            {
                while (Reader.Read())
                {
                    Movie Movie = new Movie();
                    Movie.MovieID = Reader["MovieID"].ToString().Trim();
                    Movie.Moviename = Reader["Moviename"].ToString().Trim();
                    Movie.Playstartdatetime = Reader["Playstartdatetime"].ToString();
                    Movie.Playenddatetime = Reader["Playenddatetime"].ToString();
                    Movie.Runningtime = Reader["Runningtime"].ToString();
                    Movie.Viewingclass = Reader["Viewingclass"].ToString();
                    Movie.Movieposter = Reader["Movieposter"].ToString().Trim();

                    Movies.Add(Movie);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
                return null;
            }
            return Movies;
        }

    }
}