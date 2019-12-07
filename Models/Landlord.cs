using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Landlord
    {
        [Key]
        public int IDforLand { get; set; }

        public string Title { get; set; }

        public string Surname { get; set; }

        [Display(Name = "Othernames")]
        public string OtherNames { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return Title + " " + Surname + ", " + OtherNames; } }

        [Display(Name = "Corporate Name")]
        public string CorporateName { get; set; }

        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Reg. Date")]
        public string RegDate { get; set; }
        public string LandlordID { get; set; }
        public string Status { get; set; }
        public string ImageMainUrl { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ApplicationUserId { get; set; }

        //public System.Web.HttpPostedFileBase PassportImage { get; set; }

        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string MiniBiography { get; set; }
        public int HappyClients { get; set; }
        public int YearsOfExperience { get; set; }

    }

    public partial class LandlordViewModel
    {
        public int IDforLand { get; set; }
        public string Title { get; set; }

        public string Surname { get; set; }

        [Display(Name = "Othernames")]
        public string OtherNames { get; set; }


        [Display(Name = "Corporate Name")]
        public string CorporateName { get; set; }

        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        public string RegDate { get; set; }
        public string LandlordID { get; set; }
        public string Status { get; set; }
        public string ImageMainUrl { get; set; }
        public string ApplicationUserId { get; set; }        
        public System.Web.HttpPostedFileBase ImageMain { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }

        [Display(Name = "Biography")]
        public string MiniBiography { get; set; }

        [Display(Name = "Happy Clients")]
        public int HappyClients { get; set; }

        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

    }
}