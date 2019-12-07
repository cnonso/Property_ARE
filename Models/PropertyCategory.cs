using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{
    public class PropertyCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PropertyCategoryViewModel
    {
        public int SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}