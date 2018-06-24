using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    public class Home : Controller
    {
        private Assignment2DataContext _dataContext;

        public Home(Assignment2DataContext dataContext)
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
            var blogPhotos = _dataContext.Photos.ToList();
            ViewBag.BlogPostPhotos = blogPhotos;
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
            if (userLogin == null)
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
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("Error", "Sign In Before Posting Blog!");
                return RedirectToAction("Login");
            }
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            return View();
        }

        public IActionResult CreateBlogPost(BlogPost blogPost)
        {
            var isAvailable = Convert.ToInt32(Request.Form["IsAvailable"]);
            blogPost.IsAvailable = Convert.ToBoolean(isAvailable);
            _dataContext.BlogPosts.Add(blogPost);
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
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
            foreach (var comment in comments)
            {
                var commentAuthor = (from c in _dataContext.Users where c.UserId == comment.UserId select c).FirstOrDefault();
                string name = commentAuthor.FirstName + " " + commentAuthor.LastName;
                commentsAuthorArray.Add(name);
                commentsArray.Add(comment.Content);

            }
            // ViewBag.Comments = comments.Content;
            ViewBag.Comments = commentsArray;
            ViewBag.CommentAuthors = commentsAuthorArray;
            var blogPhotos = (from i in _dataContext.Photos where i.BlogPostId == id select i).ToList();
            ViewBag.BlogPostPhotos = blogPhotos;
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
            List<BadWord> badWords = new List<BadWord>();
            badWords = _dataContext.BadWords.ToList();
            foreach(var badWord in badWords)
            {
                if (content.Contains(badWord.Word))
                {
                    content = content.Replace(badWord.Word, "******");
                }
            }
            comment.Content = content;
            _dataContext.Comments.Add(comment);
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult EditBlogPost(int id)
        {
            var display = (from p in _dataContext.BlogPosts where p.BlogPostId == id select p).FirstOrDefault();
            var blogPhotos = (from i in _dataContext.Photos where i.BlogPostId == id select i).ToList();
            ViewBag.BlogPostPhotos = blogPhotos;
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

        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("Error", "Sign In To Edit Profile!");
                return RedirectToAction("Login");
            }
            int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var profile = (from p in _dataContext.Users where p.UserId == id select p).FirstOrDefault();
            return View(profile);
        }


        public IActionResult ModifyProfile(User user)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var profile = (from p in _dataContext.Users where p.UserId == id select p).FirstOrDefault();
            profile.Address = user.Address;
            profile.City = user.City;
            profile.Country = user.Country;
            profile.DateOfBirth = user.DateOfBirth;
            profile.EmailAddress = user.EmailAddress;
            profile.FirstName = user.FirstName;
            profile.LastName = user.LastName;
            profile.PostalCode = user.PostalCode;
            profile.RoleId = user.RoleId;
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult ViewBadWords()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("Error", "Sign In To Edit Profile!");
                return RedirectToAction("Login");
            }
            var badWords = (from w in _dataContext.BadWords select w).ToList();
            return View(badWords);

        }

        public IActionResult SubmitBadWords(string word)
        {
            BadWord badWord = new BadWord();
            badWord.Word = word;
            _dataContext.BadWords.Add(badWord);
            _dataContext.SaveChanges();
            return RedirectToAction("ViewBadWords");

        }

        public async Task<IActionResult> AddImage(int blogPostId, IList<IFormFile> files)
        {
            // get your storage accounts connection string
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cst8359;AccountKey=ecMPpNU6vimZKMDTJG4seALrY7Kq7UJYjgl0/yLanXn857C8xtUJ2sF4ciB6wy9gg+e/YeYbRTaly2DVOxWhXQ==");

            // create an instance of the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            // create a container to hold your blob (binary large object.. or something like that)
            // naming conventions for the curious https://msdn.microsoft.com/en-us/library/dd135715.aspx
            var container = blobClient.GetContainerReference("justinsphotostorage");
            await container.CreateIfNotExistsAsync();

            // set the permissions of the container to 'blob' to make them public
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await container.SetPermissionsAsync(permissions);

            // for each file that may have been sent to the server from the client
            foreach (var file in files)
            {
                try
                {
                    // create the blob to hold the data
                    var blockBlob = container.GetBlockBlobReference(file.FileName);
                    if (await blockBlob.ExistsAsync())
                        await blockBlob.DeleteAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        // copy the file data into memory
                        await file.CopyToAsync(memoryStream);

                        // navigate back to the beginning of the memory stream
                        memoryStream.Position = 0;

                        // send the file to the cloud
                        await blockBlob.UploadFromStreamAsync(memoryStream);
                    }

                    // add the photo to the database if it uploaded successfully
                    var photo = new Photo();
                    photo.Url = blockBlob.Uri.AbsoluteUri;
                    photo.Filename = file.FileName;
                    photo.BlogPostId = blogPostId;
                    _dataContext.Photos.Add(photo);
                    _dataContext.SaveChanges();
                }
                catch
                {

                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteImage(String fileName)
        {

            // get your storage accounts connection string
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cst8359;AccountKey=ecMPpNU6vimZKMDTJG4seALrY7Kq7UJYjgl0/yLanXn857C8xtUJ2sF4ciB6wy9gg+e/YeYbRTaly2DVOxWhXQ==");

            // create an instance of the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            // create a container to hold your blob (binary large object.. or something like that)
            // naming conventions for the curious https://msdn.microsoft.com/en-us/library/dd135715.aspx
            var container = blobClient.GetContainerReference("justinsphotostorage");
            var blockBlob = container.GetBlockBlobReference(fileName);
            bool result = await blockBlob.DeleteIfExistsAsync();
            if (result)
            {
                Photo photo = (from p in _dataContext.Photos where p.Filename == fileName select p).FirstOrDefault();
                _dataContext.Photos.Remove(photo);
                _dataContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteBadWord(string badWord)
        {
            var badWordToDelete = (from w in _dataContext.BadWords where w.Word == badWord select w).FirstOrDefault();
            _dataContext.BadWords.Remove(badWordToDelete);
            _dataContext.SaveChanges();
            return RedirectToAction("ViewBadWords");
        }

        public IActionResult DeleteBlogPost(int id)
        {
            var blogPost = (from p in _dataContext.BlogPosts where p.BlogPostId == id select p).FirstOrDefault();
            var blogPhotos = (from i in _dataContext.Photos where i.BlogPostId == id select i).ToList();
            var blogComments = (from c in _dataContext.Comments where c.BlogPostId == id select c).ToList();
            foreach(Comment comment in blogComments)
            {
                _dataContext.Comments.Remove(comment);
            }
            _dataContext.SaveChanges();
            foreach (Photo photo in blogPhotos)
            {
                //_dataContext.Photos.Remove(photo);
                deletePic(photo.Filename);
            }
           // _dataContext.SaveChanges();
            _dataContext.BlogPosts.Remove(blogPost);
            _dataContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public async void deletePic(string photoName)
        {
            // get your storage accounts connection string
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cst8359;AccountKey=ecMPpNU6vimZKMDTJG4seALrY7Kq7UJYjgl0/yLanXn857C8xtUJ2sF4ciB6wy9gg+e/YeYbRTaly2DVOxWhXQ==");

            // create an instance of the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            // create a container to hold your blob (binary large object.. or something like that)
            // naming conventions for the curious https://msdn.microsoft.com/en-us/library/dd135715.aspx
            var container = blobClient.GetContainerReference("justinsphotostorage");
            var blockBlob = container.GetBlockBlobReference(photoName);
            bool result = await blockBlob.DeleteIfExistsAsync();
            if (result)
            {
                Photo photo = (from p in _dataContext.Photos where p.Filename == photoName select p).FirstOrDefault();
                _dataContext.Photos.Remove(photo);
                _dataContext.SaveChanges();
            }
        }
    }
}
