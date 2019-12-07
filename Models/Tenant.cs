using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Tenant
    {
        [Key]
        public int TenantID { get; set; }

        public string AccountNo { get; set; }

        public string LandlordID { get; set; }


        public string Title { get; set; }

        public string Surname { get; set; }

        [Display(Name = "Othernames")]
        public string OtherNames { get; set; }

        
        public string Address { get; set; }


        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Display(Name = "Reg. Date")]
        public string RegDate { get; set; }

        public string PropertyID { get; set; }


        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public string Status { get; set; }

        public int NoOfProperty { get; set; }

        public float Rent { get; set; }

        public string PropertyNo { get; set; }

        public string RentPaymentStatus { get; set; }

        public string CycleCount { get; set; }

        [Display(Name = "End Of Grace")]
        public string EndOfGrace { get; set; }

        public string NoOfPropertyUnits { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string ApplicationUserId { get; set; }

        //public System.Web.HttpPostedFileBase PassportImage { get; set; }
    }

    public partial class TenantViewModel
    {
        public int TenantID { get; set; }
        public string AccountNo { get; set; }
        public string LandlordID { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        [Display(Name = "Othernames")]
        public string OtherNames { get; set; }
        public string Address { get; set; }
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        [Display(Name = "Reg. Date")]
        public string RegDate { get; set; }
        public string PropertyID { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
        public string Status { get; set; }
        public int NoOfProperty { get; set; }
        public float Rent { get; set; }

        public string PropertyNo { get; set; }

        public string RentPaymentStatus { get; set; }

        public string CycleCount { get; set; }

        [Display(Name = "End Of Grace")]
        public string EndOfGrace { get; set; }

        public string NoOfPropertyUnits { get; set; }
        
        public string ApplicationUserId { get; set; }

        //public System.Web.HttpPostedFileBase PassportImage { get; set; }
    }
}