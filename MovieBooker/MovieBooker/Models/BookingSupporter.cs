using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class BookingSupporter
    {
        public List<Movieschedule> movieSchedules { get; set; }
        public List<Movie> Movies { get; set; }
        public String SelectedMovieID { get; set; }
        public Movie SelectedMovie { get; set; }
        public Theater SelectedTheater { get; set; }
    }
    
}
