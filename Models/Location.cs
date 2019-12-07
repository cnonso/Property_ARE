using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class LocationViewModel
    {
        public int SelectedLocationId { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
    }
}