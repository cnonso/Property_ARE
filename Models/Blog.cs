using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{
    public class Blog
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Body { get; set; }
        public string DatePosted { get; set; }
        public string ImageUrl { get; set; }
    }

    public partial class BlogViewModel
    { 
        public string Title { get; set; }

        [AllowHtml]
        public string Body { get; set; }
        public DateTime DatePosted { get; set; }
        public HttpPostedFileBase BlogImage { get; set; }
    }
}