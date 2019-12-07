using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{
    public class BlogComment
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string  Email { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        public Nullable<int> BlogId { get; set; }
        public Nullable<System.DateTime> DatePosted { get; set; }
        public string Icon { get; set; }
        public string Status { get; set; }
    }
    public partial class BlogCommentsViewModel
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        public HttpPostedFileBase Icon { get; set; }
    }
}