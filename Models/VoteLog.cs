using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class VoteLog
    {
        [Key]
        public int VoteId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> VoteForId { get; set; }
        public string Username { get; set; }
        public Nullable<short> Vote { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}