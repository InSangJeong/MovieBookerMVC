using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBooker.Models;
using MovieBooker.DAL;

namespace MovieBooker.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MovieList()
        {
            List<Movie> ALLMovies = Movie_DAL.Select_Movie("", new List<Tuple<string, object>>());
            ViewBag.Movies = ALLMovies;
            return View();
        }
        public ActionResult NewMovie()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewMovie(MovieAndTheater UserInputData, HttpPostedFileBase file)
        {
            return View();
        }
    }
}