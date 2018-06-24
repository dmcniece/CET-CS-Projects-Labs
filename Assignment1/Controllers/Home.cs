using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;
using Microsoft.AspNetCore.Http;
using System.Dynamic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment1.Controllers
{
    public class Home : Controller
    {
        private Assignment1DataContext _dataContext;

        public Home(Assignment1DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            //if(HttpContext.Session.GetString("UserId") != null)
            //{
            //    ViewBag.UserId = HttpContext.Session.GetString("UserId");
            //}
            return View(_dataContext.BlogPosts.ToList());
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            //if (HttpContext.Session.GetString("Error") != null || HttpContext.Session.GetString("Error") != "")
            //{
            //    ViewBag.Error = HttpContext.Session.GetString("Error");
            //}
            //else
            //{
            //    HttpContext.Session.SetString("Error", "");
            //    ViewBag.Error = "";
            //}
            return View();
        }

        public IActionResult LoginUser(User user)
        {
            var userLogin = (from u in _dataContext.Users where u.EmailAddress == user.EmailAddress && u.Password == user.Password select u).FirstOrDefault();
            if(userLogin == null)
            {
                HttpContext.Session.SetString("Error", "Login Error! Username or password is incorrect.");
                return RedirectToAction("Login");
            }
            HttpContext.Session.SetString("UserId", userLogin.UserId.ToString());
            HttpContext.Session.SetString("UserFirstName", userLogin.FirstName);
            HttpContext.Session.SetString("UserLastName", userLogin.LastName);
            HttpContext.Session.SetInt32("UserRoleId", userLogin.RoleId);
            HttpContext.Session.SetString("Error", "");
            ViewBag.Error = "";
            return RedirectToAction("Index");
        }

        public IActionResult AddBlogPost()
        {
            if(HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("Error", "Sign In Before Posting Blog!");
                return RedirectToAction("Login");
            }
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult CreateBlogPost(BlogPost blogPost)
        {
            _dataContext.BlogPosts.Add(blogPost);
            _dataContext.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult DisplayFullBlogPost(int id)
        {
            List<string> commentsArray = new List<string>();
            List<string> commentsAuthorArray = new List<string>();
            var display = (from p in _dataContext.BlogPosts where p.BlogPostId == id select p).FirstOrDefault();
            var user = (from u in _dataContext.Users where u.UserId == display.UserId select u).FirstOrDefault();
            var comments = (from c in _dataContext.Comments where c.BlogPostId == id select c).ToArray();
            ViewBag.BlogPosterEmail = user.EmailAddress;
            ViewBag.BlogPosterFirstName = user.FirstName;
            ViewBag.BlogPosterLastName = user.LastName;
            foreach(var comment in comments)
            {
                var commentAuthor = (from c in _dataContext.Users where c.UserId == comment.UserId select c).FirstOrDefault();
                string name = commentAuthor.FirstName + " " + commentAuthor.LastName;
                commentsAuthorArray.Add(name);
                commentsArray.Add(comment.Content);
                
            }
            // ViewBag.Comments = comments.Content;
            ViewBag.Comments = commentsArray;
            ViewBag.CommentAuthors = commentsAuthorArray;
            //dynamic model = new ExpandoObject();
            //model.display = display;
            //model.user = user;
            //model.comments = comments;
            return View(display);
            //return View(model);
        }

        public IActionResult AddComment(int blogPostId, int userId, string content)
        {
            Comment comment = new Models.Comment();
            comment.BlogPostId = blogPostId;
            comment.UserId = userId;
            comment.Content = content;
            _dataContext.Comments.Add(comment);
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult EditBlogPost(int id)
        {
            var display = (from p in _dataContext.BlogPosts where p.BlogPostId == id select p).FirstOrDefault();
            return View(display);
        }

        public IActionResult ModifyBlogPost(BlogPost blogPost)
        {
            var id = Convert.ToInt32(Request.Form["BlogPostId"]);
            var blogPostToUpdate = (from p in _dataContext.BlogPosts where p.BlogPostId == id select p).FirstOrDefault();
            blogPostToUpdate.Title = blogPost.Title;
            blogPostToUpdate.Content = blogPost.Content;
            blogPostToUpdate.Posted = blogPost.Posted;
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
