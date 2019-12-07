using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PropertyManagerWeb.Models
{
    public class Message
    {
        [Key]
        public int ID { get; set; }

        [Display(Name ="Name")]
        public string SenderName { get; set; }

        [Display(Name = "Phone Number")]
        public string SenderPhoneNo { get; set; }

        public string Email { get; set; }

        [Display(Name = "Message")]
        public string MessageBody { get; set; }

        [Display(Name = "Property")]
        public string  PropertyName { get; set; }
        public string LandlordID { get; set; }
        public string  DateSent { get; set; }
    }

    public partial class MessageViewModel
    {
        public int ID { get; set; }
        public string SenderName { get; set; }
        public string SenderPhoneNo { get; set; }
        public string Email { get; set; }

        [DataType(DataType.MultilineText)]
        public string MessageBody { get; set; }
        public string PropertyName { get; set; }
        public string LandlordID { get; set; }
        public DateTime DateSent { get; set; }
    }
}