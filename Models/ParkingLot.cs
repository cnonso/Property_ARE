using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{ 
    public class ParkingLot
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class ParkingLotViewModel
    {
        public int SelectedParkingType { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
    }
}