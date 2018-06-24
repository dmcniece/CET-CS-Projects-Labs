using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class Photo
    {
        public int PhotoId
        {
            get;
            set;
        }
        [ForeignKey("BlogPostId")]
        public int BlogPostId
        {
            get;
            set;
        }
        public string Filename
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
    }
}
