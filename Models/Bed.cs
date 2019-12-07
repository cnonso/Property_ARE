using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Bed
    {
        [Key]
        public int ID { get; set; }
        public string Qty { get; set; }
        public string Title { get; set; }
    }
}