using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyManagerWeb.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Data.Entity.SqlServer;

namespace PropertyManagerWeb.Controllers
{
    public class PropertiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        const int pageSize = 4;

        // GET: Properties
        [ActionName("Property Listing")]
        public ActionResult Index(int page = 1)
        {

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;


            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            var result = db.Properties.OrderByDescending(c => Guid.NewGuid())
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList(); ;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Count() / pageSize);

            return View(result);


        }

        public ActionResult SearchForRent(int page = 1)
        {

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            ViewBag.PreHeading = "Search Result-";

            string location, propertyType, parkingLot;
            float maxPrice, minPrice;
            int beds, baths;


            location = Request.Form["Locations"].ToString();
            propertyType = Request.Form["PropertyTypes"].ToString();
            parkingLot = Request.Form["parkingLot"].ToString();

            try { maxPrice = float.Parse(Request.Form["MaxPrice"].ToString()); }
            catch { maxPrice = 0; }
            try { minPrice = float.Parse(Request.Form["MinPrice"].ToString()); }
            catch { minPrice = 0; }

            beds = int.Parse(Request.Form["Beds"].ToString());
            baths = int.Parse(Request.Form["Baths"].ToString());

            List<Property> result;
            result = db.Properties.Where(p => p.Location == location)
                   .Where(p => p.PropertyType == propertyType)
                   .Where(p => p.Category == "For Rent")
                   .Where(p => p.RentAmount <= maxPrice)
                   .Where(p => p.RentAmount >= minPrice)
                   .Where(p => p.Beds == beds)
                   .Where(p => p.Baths == baths)
                   .Where(p=>p.ParkingLot == parkingLot)
                   //.Where(p => p.Toilets == toilets)
                   .OrderByDescending(c => c.Featured)
                                             .Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Location == location)
                .Where(p => p.PropertyType == propertyType)
                .Where(p => p.Category == "For Rent")
                .Where(p => p.RentAmount <= maxPrice)
                .Where(p => p.RentAmount >= minPrice)
                .Where(p => p.Beds == beds)
                .Where(p => p.Baths == baths)
                .Count() / pageSize);

            return View("Property Listing", result);
        }
        public ActionResult SearchForSale(int page = 1)
        {

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            ViewBag.PreHeading = "Search Result-";

            string location, propertyType, parkingLot;
            float maxPrice, minPrice;
            int beds, baths;


            location = Request.Form["Locations2"].ToString();
            propertyType = Request.Form["PropertyTypes2"].ToString();
            parkingLot = Request.Form["parkingLot"].ToString();

            try { maxPrice = float.Parse(Request.Form["MaxPrice2"].ToString()); }
            catch { maxPrice = 0; }
            try { minPrice = float.Parse(Request.Form["MinPrice2"].ToString()); }
            catch { minPrice = 0; }

            beds = int.Parse(Request.Form["Beds2"].ToString());
            baths = int.Parse(Request.Form["Baths2"].ToString());

            List<Property> result;
            result = db.Properties.Where(p => p.Location == location)
                   .Where(p => p.PropertyType == propertyType)
                   .Where(p => p.Category == "For Sale")
                   .Where(p => p.RentAmount <= maxPrice)
                   .Where(p => p.RentAmount >= minPrice)
                   .Where(p => p.Beds == beds)
                   .Where(p => p.Baths == baths)
                   .Where(p=>p.ParkingLot == parkingLot)
                   //.Where(p => p.Toilets == toilets)
                   .OrderByDescending(c => c.Featured)
                                             .Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Location == location)
                .Where(p => p.PropertyType == propertyType)
                .Where(p => p.Category == "For Rent")
                .Where(p => p.RentAmount <= maxPrice)
                .Where(p => p.RentAmount >= minPrice)
                .Where(p => p.Beds == beds)
                .Where(p => p.Baths == baths)
                .Count() / pageSize);

            return View("Property Listing", result);
        }

        public ActionResult Search(int page = 1)
        {

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            ViewBag.PreHeading = "Search Result-";

            float maxPrice, minPrice;
            string location = Request.Form["Location"].ToString();
            string propertyType = Request.Form["PropertyTypes"].ToString();
            try { maxPrice = float.Parse(Request.Form["MaxPrice"].ToString()); }
            catch { maxPrice = 0; }
            try { minPrice = float.Parse(Request.Form["MinPrice"].ToString()); }
            catch { minPrice = 0; }
            int beds = int.Parse(Request.Form["Beds"].ToString());
            int baths = int.Parse(Request.Form["Baths"].ToString());
            //int toilets = int.Parse(Request.Form["Toilets"].ToString()); 
            string category = Request.Form["Category"].ToString();
            string parkingLot = Request.Form["parkingLot"].ToString();


            List<Property> result;
            result = db.Properties.Where(p => p.Location == location)
                   .Where(p => p.PropertyType == propertyType)
                   .Where(p => p.Category == category)
                   .Where(p => p.RentAmount <= maxPrice)
                   .Where(p => p.RentAmount >= minPrice)
                   .Where(p => p.Beds == beds)
                   .Where(p => p.Baths == baths)
                   .Where(p=>p.ParkingLot == parkingLot)
                   //.Where(p => p.Toilets == toilets)
                   .OrderByDescending(c => c.Featured)
                                             .Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Location == location)
                .Where(p => p.PropertyType == propertyType)
                .Where(p => p.Category == category)
                .Where(p => p.RentAmount <= maxPrice)
                .Where(p => p.RentAmount >= minPrice)
                .Where(p => p.Beds == beds)
                .Where(p => p.Baths == baths)
                .Count() / pageSize);

            return View("Property Listing", result);
        }
        public ActionResult SearchLocation(string loc, int page = 1)
        {
            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            string location = "";
            if (loc == null)
                location = Request.Form["SearchLocation"].ToString();
            else location = loc;

            ViewBag.PreHeading = location + "-";
            var result = db.Properties.Where(p => p.Location == location)
                    .OrderByDescending(c => c.Featured)
                                              .Skip((page - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToList(); ;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Location == location)
                 .Count() / pageSize);

            return View("Property Listing", result);
        }
        public ActionResult MyPropertySearch(int page = 1)
        {

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;
            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            ViewBag.PreHeading = "Search Result-";
            var userid = User.Identity.GetUserId();
            string landlordid = db.Landlords.Where(l => l.ApplicationUserId == userid).First().LandlordID;

            float maxPrice, minPrice;
            string location = Request.Form["Location"].ToString();
            string propertyType = Request.Form["PropertyTypes"].ToString();
            try { maxPrice = float.Parse(Request.Form["MaxPrice"].ToString()); }
            catch { maxPrice = 0; }
            try { minPrice = float.Parse(Request.Form["MinPrice"].ToString()); }
            catch { minPrice = 0; }
            int beds = int.Parse(Request.Form["Beds"].ToString());
            int baths = int.Parse(Request.Form["Baths"].ToString());
            //int toilets = int.Parse(Request.Form["Toilets"].ToString()); 
            string category = Request.Form["Category"].ToString();
            string parkingLot = Request.Form["parkingLot"].ToString();

            List<Property> result;
            result = db.Properties.Where(p => p.Location == location)
                   .Where(p => p.PropertyType == propertyType)
                   .Where(p => p.Category == category)
                   .Where(p => p.RentAmount <= maxPrice)
                   .Where(p => p.RentAmount >= minPrice)
                   .Where(p => p.Beds == beds)
                   .Where(p => p.Baths == baths)
                   .Where(p => p.LandlordID == landlordid)
                   .Where(p=>p.ParkingLot == parkingLot)
                   //.Where(p => p.Toilets == toilets)
                   .OrderByDescending(c => c.RegDate)
                                             .Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Location == location)
                .Where(p => p.PropertyType == propertyType)
                .Where(p => p.Category == category)
                .Where(p => p.RentAmount <= maxPrice)
                .Where(p => p.RentAmount >= minPrice)
                .Where(p => p.Beds == beds)
                .Where(p => p.Baths == baths)
                                   .Where(p => p.LandlordID == landlordid)
                .Count() / pageSize);

            return View("My-Property-Listing", result);
        }


        [ActionName("Agents-Property-Listing")]
        public ActionResult AgentsListing(string id, string name, int page = 1)
        {
            ViewBag.Title = "Agent: " + name;
            ViewBag.Agent = name;
            string decrypted_id = AppHandler.Decrypt(id);
            int landlord_id = db.Landlords.Where(l => l.LandlordID == decrypted_id).First().IDforLand;
            Landlord landlord = db.Landlords.Find(landlord_id);

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;
            var model = new MultipleModels
            {
                Landlord = landlord,
                PropertyList = db.Properties.Where(l => l.LandlordID == decrypted_id).OrderByDescending(c => c.RegDate)
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList()
            };
        
           
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(l => l.LandlordID == decrypted_id).Count() / pageSize);

            return View(model);

            //return View(db.Properties.Include(l => l.Landlord).ToList());
        }

        [ActionName("Featured-Property-Listing")]
        public ActionResult FeaturedProperties(string id, string name, int page = 1)
        {
            ViewBag.PreHeading = "Featured-";
            
            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            var result = db.Properties.Where(p => p.Featured == true).OrderByDescending(c => c.RegDate)
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList();
            


            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(p => p.Featured == true).Count() / pageSize);

            return View("Property Listing", result);
        }


        [ActionName("My-Property-Listing")]
        public ActionResult MyListing(int page = 1)
        {
            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Location = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Location = Location;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;
            ////Get number of beds database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get number of baths from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;

            string userid = User.Identity.GetUserId();
            string landlord_id = new DB().ReadData("Select LandlordID from Landlords where ApplicationUserId = '" + userid + "'");
            var result = db.Properties.Where(l => l.LandlordID == landlord_id).OrderBy(c => c.RegDate)
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList(); ;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Properties.Where(l => l.LandlordID == landlord_id).Count() / pageSize);

            return View(result);
        }

        // GET: Properties/Details/5
        public ActionResult Details(int? id, string location)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = db.Properties.Find(id);
            Session["ID"] = id;

            //Property myaccount = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }

            //string location;
            if (location == null)
            {
                ViewBag.RelatedProperty = "You may also like";
            }
            else
            {
                ViewBag.RelatedProperty = "Other properties around " + location;
            }

            MultipleModels model = new MultipleModels
            {
                Property = db.Properties.Find(id),

                RelatedProperties = db.Properties.Where(l => l.Location.Contains(location))
                                        .OrderBy(c => Guid.NewGuid())
                                        .Take(3).ToList(),

                CommentsForService = db.Comments.Where(l => l.ServiceId == property.PropertyID)
                                        .OrderBy(c => c.Date)
                                        .ToList()
            };
            Session["url"] = Request.Url.AbsolutePath;
            return View(model);
        }

        // GET: Properties/Details/5
        [ActionName("Verified Agents")]
        public ActionResult VerifiedAgents()
        {
            var result = db.Landlords.Include(l => l.User).Where(l => l.Status.Equals("Active Verified")).ToList()
                                           .OrderBy(c => Guid.NewGuid())
                                          .ToList();
            return View(result);
        }


        public static string notification = "";
        // GET: Properties/Create
        [Authorize]
        public ActionResult NewProperty()
        {
            ////Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyType = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyType;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Locations = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Locations = Locations;

            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Beds = db.Beds.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Beds = Beds;

            ////Get No. of Baths from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Baths = db.Baths.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Baths = Baths;

            ////Get No. of Toilets from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Toilets = db.Toilets.Select(c => new SelectListItem
            {
                Value = c.Qty,
                Text = c.Title,
            });
            ViewBag.Toilets = Toilets;


            string userId = User.Identity.GetUserId();
            string userType = new DB().ReadData("Select UserType from AspNetUsers where Id = '" + userId + "'");
            if (userType != "Landlord" && userType != "Admin")
                return RedirectToAction("Index", "Home");

            string landlordName = new DB().ReadData("Select Surname from Landlords where ApplicationUserId = '" + userId + "'");
            if (landlordName.Trim() == string.Empty)
            {
                notification = "Please Complete your Profile to Continue.";
                return RedirectToAction("Edit", "Landlords");
            }
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewProperty(MultipleModels model)
        {
            string userId = User.Identity.GetUserId();
            string userType = new DB().ReadData("Select UserType from AspNetUsers where Id = '" + userId + "'");

            string landlordid = db.Landlords.Where(u => u.ApplicationUserId == userId).First().LandlordID;

            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };
            try
            {
                if (model.PropertyViewModel.ImageMain != null || model.PropertyViewModel.ImageMain.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.PropertyViewModel.ImageMain.ContentType))
                    {
                        ModelState.AddModelError("ImageMain", "Please choose either a GIF, JPG or PNG image.");
                    }
                }

                if (model.PropertyViewModel.Image1 != null || model.PropertyViewModel.Image1.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.PropertyViewModel.Image1.ContentType))
                    {
                        ModelState.AddModelError("Image1", "Please choose either a GIF, JPG or PNG image.");
                    }
                }

                if (model.PropertyViewModel.Image2 != null || model.PropertyViewModel.Image2.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.PropertyViewModel.Image2.ContentType))
                    {
                        ModelState.AddModelError("Image2", "Please choose either a GIF, JPG or PNG image.");
                    }
                }

                if (model.PropertyViewModel.Image3 != null || model.PropertyViewModel.Image3.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.PropertyViewModel.Image3.ContentType))
                    {
                        ModelState.AddModelError("Image3", "Please choose either a GIF, JPG or PNG image.");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            //if (ModelState.IsValid)
            //if (model.PropertyViewModel.Name.Trim() != string.Empty || model.PropertyViewModel.RentAmount.Trim() != string.Empty || model.PropertyViewModel.Address.Trim() != string.Empty || model.PropertyViewModel.PlainDescription.Trim() != string.Empty)            
            float coordLat, coordLong = 0;
            try
            {
                coordLat = float.Parse(Request.Form["CoordinateLat"].ToString());
                coordLong = float.Parse(Request.Form["CoordinateLong"].ToString());
            }
            catch (Exception ex)
            {
                coordLat = coordLong = 0;
            }
            Property property = new Property
            {
                LandlordID = db.Landlords.Where(u => u.ApplicationUserId == userId).First().LandlordID,
                RegDate = DateTime.Today.ToShortDateString(),
                Status = "Non-Managed",
                Name = model.PropertyViewModel.Name,
                Address = model.PropertyViewModel.Address,
                Location = Request.Form["Locations"].ToString(),
                PropertyType = Request.Form["PropertyTypes"].ToString(),
                Category = Request.Form["Category"].ToString(),
                PlainDescription = model.PropertyViewModel.PlainDescription,
                RentAmount = float.Parse(model.PropertyViewModel.RentAmount),
                CoordinateLat = coordLat,
                CoordinateLong = coordLong,
                Beds = int.Parse(Request.Form["Beds"].ToString()),
                Baths = int.Parse(Request.Form["Baths"].ToString()),
                //Toilets = int.Parse(Request.Form["Toilets"].ToString()),
                ParkingLot = Request.Form["ParkingLot"],
                SectionId = 1,
                Votes = "1,0,0,0,0",
                TotalVotes = 1

                //CoordinateLat = model.PropertyViewModel.CoordinateLat,
                //CoordinateLong = model.PropertyViewModel.CoordinateLong,               
                //Beds = model.PropertyViewModel.Beds,
                //Baths = model.PropertyViewModel.Baths,
                //Toilets = model.PropertyViewModel.Toilets,

            };

            try
            {
                if (model.PropertyViewModel.ImageMain.ContentLength > 0)
                {
                    var uploadDir = "/Images";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.ImageMain.FileName);
                    var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.ImageMain.FileName);
                    model.PropertyViewModel.ImageMain.SaveAs(imagePath);
                    property.ImageMainUrl = imageUrl;
                }

                if (model.PropertyViewModel.Image1.ContentLength > 0)
                {
                    var uploadDir = "/Images";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image1.FileName);
                    var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image1.FileName);
                    model.PropertyViewModel.Image1.SaveAs(imagePath);
                    property.Image1Url = imageUrl;
                }

                if (model.PropertyViewModel.Image2.ContentLength > 0)
                {
                    var uploadDir = "/Images";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image2.FileName);
                    var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image2.FileName);
                    model.PropertyViewModel.Image2.SaveAs(imagePath);
                    property.Image2Url = imageUrl;
                }

                if (model.PropertyViewModel.Image3.ContentLength > 0)
                {
                    var uploadDir = "/Images";
                    var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image3.FileName);
                    var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image3.FileName);
                    model.PropertyViewModel.Image3.SaveAs(imagePath);
                    property.Image3Url = imageUrl;
                }
            }
            catch (Exception ex)
            { }
            db.Properties.Add(property);
            db.SaveChanges();

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyType = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyType;
            ViewBag.SaveStatus = "Succefully enlisted property.";

            return RedirectToAction("Property Listing");
            //return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(MultipleModels model)
        {
            var url = Session["url"].ToString();

            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Name = model.CommentsViewModel.Name,
                    Content = model.CommentsViewModel.Content,
                    ServiceId = Convert.ToInt32(Session["ID"].ToString()),
                    Date = DateTime.Now,
                };

                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect(url);
            }
            return View(model);
        }

        // GET: Properties/Edit/5
        public ActionResult Update(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = db.Properties.Find(id);
            Session["ID"] = id;

            //Property myaccount = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }


            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,                
            });
            ViewBag.PropertyTypes = PropertyTypes;
 
            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MinPrice = MinPrice;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MaxPrice = db.PriceRanges.OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                Text = c.S_Amount
            });
            ViewBag.MaxPrice = MaxPrice;
             

            ViewBag.RelatedProperty = "Related Properties";
            string userid = User.Identity.GetUserId();
            string landlord_id = db.Landlords.Where(l => l.ApplicationUserId == userid).First().LandlordID;
            MultipleModels model = new MultipleModels
            {
                Property = db.Properties.Find(id),
                SelectedLocation = property.Location,
                Locations = db.Locations.Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                }),

                SelectedPropertyType = property.PropertyType,
                PropertyTypes = db.PropertyTypes.Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                }),

                SelectedPropertyCategory = property.Category,
                PropertyCategories = db.PropertyCategories.Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                }),

                SelectedParkingType = property.ParkingLot,
                ParkingType = db.ParkingLot.Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                }),

                SelectedBath = property.Baths,
                Baths = db.Baths.Select(x => new SelectListItem
                {
                    Value = x.Qty,
                    Text = x.Title
                }),

                SelectedBed = property.Beds,
                Beds = db.Beds.Select(x => new SelectListItem
                {
                    Value = x.Qty,
                    Text = x.Title
                }),

                SelectedToilet = property.Toilets,
                Toilets = db.Toilets.Select(x => new SelectListItem
                {
                    Value = x.Qty,
                    Text = x.Title
                }),

                PropertyViewModel = new PropertyViewModel
                {
                    PropertyID = property.PropertyID,
                    Name = property.Name,
                    Address = property.Address,
                    RegDate = Convert.ToDateTime(property.RegDate),
                    Status = property.Status,

                    RentAmount = "₦" + string.Format("{0:#,0.00}", property.RentAmount),
                    PlainDescription = property.PlainDescription,
                    ImageMainUrl = property.ImageMainUrl,
                },
                RelatedProperties = db.Properties.Where(l => l.LandlordID == landlord_id)
                                        .OrderBy(c => Guid.NewGuid())
                                        .Take(5).ToList(),

                CommentsForService = db.Comments.Where(l => l.ServiceId == property.PropertyID)
                                        .OrderBy(c => c.Date)
                                        .ToList()
            };
            Session["url"] = Request.Url.AbsolutePath;
            return View(model);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MultipleModels model)
        {

            var validImageTypes = new string[]
             {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
             };

            try
            {
                if (model.PropertyViewModel.ImageMain != null || model.PropertyViewModel.ImageMain.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.PropertyViewModel.ImageMain.ContentType))
                    {
                        ModelState.AddModelError("ImageMain", "Please choose either a GIF, JPG or PNG image.");
                    }
                }
            }
            catch (Exception ex)
            { }


            if (ModelState.IsValid)
            {
                Property property = new Property
                {
                    PropertyID = model.Property.PropertyID,
                    Name = model.PropertyViewModel.Name,
                    Address = model.PropertyViewModel.Address,
                    RegDate = model.Property.RegDate,
                    Status = model.Property.Status,
                    Votes = model.Property.Votes,
                    VoteId = model.Property.VoteId,
                    SectionId = model.Property.SectionId,
                    TotalVotes = model.Property.TotalVotes,
                    PlainDescription = model.PropertyViewModel.PlainDescription,
                    RentAmount = float.Parse(model.PropertyViewModel.RentAmount.Replace("₦", "").Replace(",", "").Trim()),
                    Featured = model.Property.Featured,
                    Beds = model.SelectedBed,
                    Baths = model.SelectedBath,
                    Toilets = model.SelectedToilet,
                    Location = model.SelectedLocation,
                    LandlordID = model.Property.LandlordID,
                    Category = model.SelectedPropertyCategory,
                    ParkingLot = model.SelectedParkingType
                };
                try
                {
                    property.PropertyType = Request.Form["PropertyType"].ToString();
                }
                catch (Exception ex)
                {
                    property.PropertyType = model.Property.PropertyType;
                }

                try
                {
                    property.CoordinateLat = float.Parse(Request.Form["CoordinateLat"].ToString());
                }
                catch (Exception ex)
                {
                    property.CoordinateLat = model.Property.CoordinateLat;
                }

                try
                {
                    property.CoordinateLong = float.Parse(Request.Form["CoordinateLong"].ToString());
                }
                catch (Exception ex)
                {
                    property.CoordinateLong = model.Property.CoordinateLong;
                }

                try
                {
                    if (model.PropertyViewModel.ImageMain.ContentLength > 0)
                    {
                        var uploadDir = "/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.ImageMain.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.ImageMain.FileName);
                        model.PropertyViewModel.ImageMain.SaveAs(imagePath);
                        property.ImageMainUrl = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    property.ImageMainUrl = model.Property.ImageMainUrl;
                }

                try
                {
                    if (model.PropertyViewModel.Image1.ContentLength > 0)
                    {
                        var uploadDir = "/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image1.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image1.FileName);
                        model.PropertyViewModel.Image1.SaveAs(imagePath);
                        property.Image1Url = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    property.Image1Url = model.Property.Image1Url;
                }

                try
                {
                    if (model.PropertyViewModel.Image2.ContentLength > 0)
                    {
                        var uploadDir = "/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image2.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image2.FileName);
                        model.PropertyViewModel.Image2.SaveAs(imagePath);
                        property.Image2Url = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    property.Image2Url = model.Property.Image2Url;
                }

                try
                {
                    if (model.PropertyViewModel.Image3.ContentLength > 0)
                    {
                        var uploadDir = "/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image3.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.PropertyViewModel.Name + "_" + model.PropertyViewModel.Image3.FileName);
                        model.PropertyViewModel.Image3.SaveAs(imagePath);
                        property.Image3Url = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    property.Image3Url = model.Property.Image3Url;
                }

                db.Entry(property).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("My-Property-Listing");
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            return View(model);
        }

        // GET: Properties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = db.Properties.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Property property = db.Properties.Find(id);
            db.Properties.Remove(property);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
