using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public partial class PriceRange
    {
        [Key]
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string S_Amount { get; set; }
    }
}