using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Property
    {
        [Key]
        public int PropertyID { get; set; }        

        [Display(Name = "Property Name")]
        public string Name { get; set; }
        public string PropertyType { get; set; }

        public string Category { get; set; }

        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public string Location { get; set; }

        [Display(Name = "Reg. Date")]
        public string RegDate { get; set; }
        public float Commission { get; set; }

        [Display(Name = "Remittance Pref.")]
        public string RemittancePref { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Description")]
        public string PlainDescription { get; set; }
        public float RentAmount { get; set; }

        [Display(Name = "Available Units")]
        public string AvailableUnits { get; set; }

        public string Status { get; set; }

        public float CoordinateLong { get; set; }

        public float CoordinateLat { get; set; }

        public string Votes { get; set; }
        public Nullable<int> VoteId { get; set; }
        public Nullable<int> SectionId { get; set; }

        public int TotalVotes { get; set; }

        public string ImageMainUrl { get; set; }
        public string Image1Url { get; set; }
        public string Image2Url { get; set; }
        public string Image3Url { get; set; }
        public string Image4Url { get; set; }
        public string Image5Url { get; set; }
        public virtual Landlord Landlord { get; set; }
        public string LandlordID { get; set; }
        public bool Featured { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public int Toilets { get; set; }
        public string ParkingLot { get; set; }

    }
    public partial class PropertyViewModel
    {
        public int PropertyID { get; set; }

        [Display(Name = "Property Name")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public DateTime RegDate { get; set; }
        public string PropertyType { get; set; }

        public string Status { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string PlainDescription { get; set; }

        [Display(Name = "Rent")]
        public string RentAmount { get; set; }

        public float CoordinateLong { get; set; }

        public float CoordinateLat { get; set; }
                
        public string ImageMainUrl { get; set; }
        public string Image1Url { get; set; }
        public string Image2Url { get; set; }
        public string Image3Url { get; set; }
        public string Image4Url { get; set; }
        public string Image5Url { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public int Toilets { get; set; }
        public string ParkingLot { get; set; }

        public System.Web.HttpPostedFileBase ImageMain { get; set; }
        public System.Web.HttpPostedFileBase Image1 { get; set; }
        public System.Web.HttpPostedFileBase Image2 { get; set; }
        public System.Web.HttpPostedFileBase Image3 { get; set; }
        public System.Web.HttpPostedFileBase Image4 { get; set; }
        public System.Web.HttpPostedFileBase Image5 { get; set; }

    }

}
