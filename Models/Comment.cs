using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Icon { get; set; }
    }
    public partial class CommentsViewModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public HttpPostedFileBase Icon { get; set; }
    }
}