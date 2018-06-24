using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lab4B.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab4B.Controllers
{
    public class Home : Controller
    {
        private MoviesContext _moviesContext;
        // GET: /<controller>/
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public Home(MoviesContext context)
        {
            _moviesContext = context;
        }


        public IActionResult Index()
        {
            // if you get an error here be sure to run the script to generate the database
            // also, obviously make sure that sql server express is installed
            /*
                USE [master]                
                GO                
                CREATE DATABASE [MVCCoreAndEF]                
                GO                
                USE [MVCCoreAndEF]                
                CREATE TABLE Cars (                
                    CarId int not null primary key identity(1,1),                
                    Model nvarchar(1000) not null                
                )
            */
            // if you still get an error after running the database script change the connection string in Startup.cs
            // from @"Server=localhost;Database=MVCCoreAndEF;Trusted_Connection=True;MultipleActiveResultSets=true;";
            // to   @"Server=localhost\SQLEXPRESS;Database=MVCCoreAndEF;Trusted_Connection=True;MultipleActiveResultSets=true;";
            // if you still get an error, find me :)

            return View(_moviesContext.Movies.ToList());
            //return View();
        }

        public IActionResult AddMovie()
        {
            return View();
        }

        public IActionResult CreateMovie(Movie movie)
        {
            _moviesContext.Movies.Add(movie);
            _moviesContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditMovie(int id)
        {
            var movieToUpdate = (from m in _moviesContext.Movies where m.MovieId == id select m).FirstOrDefault();

            return View(movieToUpdate);
        }

        public IActionResult ModifyMovie(Movie movie)
        {
            var id = Convert.ToInt32(Request.Form["MovieId"]);

            var movieToUpdate = (from m in _moviesContext.Movies where m.MovieId == id select m).FirstOrDefault();
            movieToUpdate.Title = movie.Title;
            movieToUpdate.SubTitle = movie.SubTitle;
            movieToUpdate.Description = movie.Description;
            movieToUpdate.Year = movie.Year;
            movieToUpdate.Rating = movie.Rating;

            _moviesContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteMovie(int id)
        {
            var movieToDelete = (from m in _moviesContext.Movies where m.MovieId == id select m).FirstOrDefault();
            _moviesContext.Movies.Remove(movieToDelete);
            _moviesContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
