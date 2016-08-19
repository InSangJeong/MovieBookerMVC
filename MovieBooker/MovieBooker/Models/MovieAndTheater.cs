using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class MovieAndTheater
    {
        public Movie movie { get; set; }
        public Theater theater { get; set; }
        public string PlayDayTimes { get; set; }
    }
}