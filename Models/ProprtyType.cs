using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class ProprtyType
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}