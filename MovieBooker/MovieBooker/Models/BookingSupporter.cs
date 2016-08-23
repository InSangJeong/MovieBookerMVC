using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class BookingSupporter
    {
        //모든 영화 값이 들어감
        public List<Movie> Movies { get; set; }
        //사용자가 선택한 영화명에 해당하는 모든 영화 스케쥴이 들어감
        public List<Movieschedule> movieSchedules { get; set; }
        //여기에는 사용자가 선택한 영화명, 상영일시에 맞는 영화스케쥴이 들어갑니다.
        public List<Movieschedule> SelectedmovieSchedules { get; set; }
        //영화스케쥴을 분리한이유는 사용자가 중간에 값을 바꿀경우에 이전 데이터가 없어지는것을 방지하기 위함입니다.

        
        public List<Seat> Seats { get; set; }

        public String SelectedMovieID { get; set; }
        public String SelectedPlaytime { get; set; }
        public String SelectedTheater { get; set; }
    }
    
}
