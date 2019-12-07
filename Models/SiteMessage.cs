using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class SiteMessage
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string SenderName { get; set; }

        [Display(Name = "Phone Number")]
        public string SenderPhoneNo { get; set; }

        public string Email { get; set; }

        [Display(Name = "Message")]
        public string MessageBody { get; set; }
        
        public string DateSent { get; set; }
    }

    public partial class SiteMessageViewModel
    {
        public int ID { get; set; }

        [Required]
        public string SenderName { get; set; }
        public string SenderPhoneNo { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string MessageBody { get; set; }
        public DateTime DateSent { get; set; }
    }
}