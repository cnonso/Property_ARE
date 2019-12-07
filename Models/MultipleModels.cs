using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagerWeb.Models
{
    public class MultipleModels
    {
        public LoginViewModel LoginViewModel { get; set; }
        public Property Property { get; set; }
        public IEnumerable<Property> FeaturedProperties { get; set; }
        public Landlord Landlord { get; set; }
        public IEnumerable<Landlord> Landlords { get; set; }
        public LandlordViewModel LandlordViewModel { get; set; }
        public IEnumerable<Property> PropertyList { get; set; }
        public PropertyViewModel PropertyViewModel { get; set; }
        public ProprtyType ProprtyTypes { get; set; }
        public IEnumerable<Property> RelatedProperties { get; set; }
        public IEnumerable<Comment> CommentsForService { get; set; }
        public CommentsViewModel CommentsViewModel { get; set; }
        public MessageViewModel MessageViewModel { get; set; }

        public Blog Blog { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
        public BlogCommentsViewModel BlogCommentsViewModel { get; set; }
        public IEnumerable<Blog> RelatedBlogs { get; set; }
        public IEnumerable<BlogComment> BlogComments { get; set; }


        //public LocationViewModel LocationViewModel { get; set; }
        public string SelectedLocation { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }

        public string SelectedPropertyType { get; set; }
        public IEnumerable<SelectListItem> PropertyTypes { get; set; }
        public string SelectedParkingType { get; set; }
        public IEnumerable<SelectListItem> ParkingType { get; set; }

        public string SelectedPropertyCategory { get; set; }
        public IEnumerable<SelectListItem> PropertyCategories { get; set; }

        public int SelectedBed { get; set; }
        public IEnumerable<SelectListItem> Beds { get; set; }
        public int SelectedBath { get; set; }
        public IEnumerable<SelectListItem> Baths { get; set; }
        public int SelectedToilet { get; set; }
        public IEnumerable<SelectListItem> Toilets { get; set; }

    }
}