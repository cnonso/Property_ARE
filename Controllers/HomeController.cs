using PropertyManagerWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Net;

namespace PropertyManagerWeb.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        const int pageSize = 3;

        public ActionResult Index(int page = 1)
        {
            string url = Request.Url.AbsolutePath;
            ////Get property types from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> PropertyTypes = db.PropertyTypes.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.PropertyTypes = PropertyTypes;

            ////Get the locations from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> Locations = db.Locations.Select(c => new SelectListItem
            {
                Value = c.Name,
                Text = c.Name,
            });
            ViewBag.Locations = Locations;

            //Get the value from database and then set it to ViewBag to pass it View
            IEnumerable<SelectListItem> MinPrice = db.PriceRanges.OrderBy(c => c.Id).Select(c => new SelectListItem
            {
                Value = SqlFunctions.StringConvert(c.Amount),
                //Text = "₦" + string.Format("{0:#,0.00}", c.Amount)
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

            ViewBag.Nnewi = db.Properties.Where(l => l.Location == "Nnewi").Count();
            ViewBag.Ihiala = db.Properties.Where(l => l.Location == "Ihiala").Count();
            ViewBag.Awka = db.Properties.Where(l => l.Location == "Awka").Count();
            ViewBag.Onitsha = db.Properties.Where(l => l.Location == "Onitsha").Count();
            ViewBag.Ogbunike = db.Properties.Where(l => l.Location == "Ogbunike").Count();
            ViewBag.Nkpor = db.Properties.Where(l => l.Location == "Nkpor").Count();
            ViewBag.Aguleri = db.Properties.Where(l => l.Location == "Aguleri").Count();
            ViewBag.Achina = db.Properties.Where(l => l.Location == "Achina").Count();
            ViewBag.Ekwulobia = db.Properties.Where(l => l.Location == "Ekwulobia").Count();
            ViewBag.Umunze = db.Properties.Where(l => l.Location == "Umunze").Count();
            ViewBag.Igbariam = db.Properties.Where(l => l.Location == "Igbariam").Count();
            ViewBag.Mbakwu = db.Properties.Where(l => l.Location == "Mbakwu").Count();
            ViewBag.Oko = db.Properties.Where(l => l.Location == "Oko").Count();

            var model = new MultipleModels
            {
                Blogs = db.Blogs.OrderBy(b => b.ID).Take(5).ToList(),
                FeaturedProperties = db.Properties.Where(c => c.Featured == true).Take(3),
                PropertyList = db.Properties.OrderByDescending(c => c.PropertyID)
                                        //.Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList()
            };

            return View(model);
        }

        public JsonResult SendRating(string r, string s, string id, string url)
        {
            int serviceId = 0;
            Int16 thisVote = 0;
            Int16 sectionId = 0;
            Int16.TryParse(s, out sectionId);
            Int16.TryParse(r, out thisVote);
            int.TryParse(id, out serviceId);

            if (!User.Identity.IsAuthenticated)
            {
                return Json("<small>Not authenticated!</small>");
            }

            if (serviceId.Equals(0))
            {
                return Json("<small>Sorry, record to vote doesn't exists</small>");
            }

            switch (s)
            {
                case "1": // property rating
                    // check if he has already voted
                    var isIt = db.VoteLogs.Where(v => v.SectionId == sectionId &&
                        v.Username.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) && v.VoteForId == serviceId).FirstOrDefault();
                    if (isIt != null)
                    {
                        // keep the property voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(url, "true");
                        Response.Cookies.Add(cookie);
                        return Json("<small>You have already rated this post, thanks !</small>");
                    }

                    var property = db.Properties.Where(sc => sc.PropertyID == serviceId).FirstOrDefault();

                    var property_ = db.Properties.Find(serviceId);
                    int totalVotes = property_.TotalVotes;

                    if (property != null)
                    {
                        object obj = property.Votes;

                        string updatedVotes = string.Empty;
                        string[] votes = null;
                        if (obj != null && obj.ToString().Length > 0)
                        {
                            string currentVotes = obj.ToString(); // votes pattern will be 0,0,0,0,0
                            votes = currentVotes.Split(',');
                            // if proper vote data is there in the database
                            if (votes.Length.Equals(5))
                            {
                                // get the current number of vote count of the selected vote, always say -1 than the current vote in the array 
                                int currentNumberOfVote = int.Parse(votes[thisVote - 1]);
                                // increase 1 for this vote
                                currentNumberOfVote++;
                                // set the updated value into the selected votes
                                votes[thisVote - 1] = currentNumberOfVote.ToString();
                                property_.TotalVotes = totalVotes++;
                            }
                            else
                            {
                                votes = new string[] { "0", "0", "0", "0", "0" };
                                votes[thisVote - 1] = "1";
                                totalVotes = 1;
                                property_.TotalVotes = totalVotes;
                            }
                        }
                        else
                        {
                            votes = new string[] { "0", "0", "0", "0", "0" };
                            votes[thisVote - 1] = "1";
                            totalVotes = 1;
                            property_.TotalVotes = totalVotes;
                        }

                        // concatenate all arrays now
                        foreach (string ss in votes)
                        {
                            updatedVotes += ss + ",";
                        }
                        updatedVotes = updatedVotes.Substring(0, updatedVotes.Length - 1);

                        property.TotalVotes = totalVotes;
                        db.Entry(property).State = EntityState.Modified;
                        property.Votes = updatedVotes;
                        db.SaveChanges();

                        VoteLog vl = new VoteLog()
                        {
                            Active = true,
                            SectionId = Int16.Parse(s),
                            Username = User.Identity.Name,
                            Vote = thisVote,
                            VoteForId = serviceId
                        };

                        db.VoteLogs.Add(vl);

                        db.SaveChanges();

                        // keep the product voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(url, "true");
                        Response.Cookies.Add(cookie);
                    }
                    break;
                default:
                    break;
            }
            return Json("<small>You rated " + r + " star(s), thanks !</small>");
        }

        [Authorize(Roles = ("Admin"))]
        [ActionName("Create Blog")]
        public ActionResult CreateBlogTopic()
        {
            string userId = User.Identity.GetUserId();
            string userType = new DB().ReadData("Select UserType from AspNetUsers where Id = '" + userId + "'");
            if (userType != "Admin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult CreateBlogTopic(BlogViewModel model)
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
                if (model.BlogImage != null || model.BlogImage.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.BlogImage.ContentType))
                    {
                        ModelState.AddModelError("BlogImage", "Please choose either a GIF, JPG or PNG image.");
                    }
                }
            }
            catch (Exception ex)
            { }

            if (ModelState.IsValid)
            {
                Blog blog = new Blog
                {
                    Title = model.Title,
                    Body = model.Body,
                    DatePosted = DateTime.Now.ToString()
                };

                try
                {
                    if (model.BlogImage.ContentLength > 0)
                    {
                        var uploadDir = "/Images/Blog";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.Title + "_" + model.BlogImage.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.Title + "_" + model.BlogImage.FileName);
                        model.BlogImage.SaveAs(imagePath);
                        blog.ImageUrl = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    blog.ImageUrl = "~/Images/Blog/icon_.jpg";
                }

                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Blog Posts");
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            return View(model);
        }

        [ActionName("Blog Posts")]
        public ActionResult BlogPosts(int page = 1)
        {
            //return View(db.Blogs.OrderBy(b => b.ID).ToList());
            var result = db.Blogs.OrderBy(c => c.DatePosted)
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList(); ;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Blogs.Count() / pageSize);

            return View(result);
        }

        public ActionResult BlogDetail(int? id, string cC)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            Session["ID"] = id;

            //Property myaccount = db.Properties.Find(id);
            if (blog == null) 
            {
                return HttpNotFound();
            }

            ViewBag.BlogTitle = blog.Title;
            ViewBag.CommentsCount = cC;
            MultipleModels model = new MultipleModels
            {
                Blog = blog,
                RelatedBlogs = db.Blogs. Where(c => c.ID != id)
                                        .OrderBy(c => Guid.NewGuid())
                                        .Take(5).ToList(),

                BlogComments = db.BlogComments.Where(b => b.BlogId == blog.ID)
                                        .OrderByDescending(c => c.DatePosted)
                                        .ToList()
            };
            
            Session["blog_url"] = Request.Url.AbsolutePath;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(MultipleModels model)
        {
            var url = Session["blog_url"].ToString();

            int blog_id = int.Parse(Request.Form["blog_id"].ToString());
            if (ModelState.IsValid)
            {
                var comment = new BlogComment
                {
                    Name = model.BlogCommentsViewModel.Name,
                    Content = model.BlogCommentsViewModel.Content,
                    BlogId = blog_id,
                    DatePosted = DateTime.Now,
                    Status = "Shown"
                };

                db.BlogComments.Add(comment);
                db.SaveChanges();
                return Redirect(url);
            }
            return View("BlogDetails", model);
        }

        public ActionResult HideComment(int id)
        {
            var url = Session["blog_url"].ToString();
            new DB().ExecuteQuery("Update BlogComments SET [Status] = 'Hidden' where Id = '" + id + "'");
                return Redirect(url);
        }
        public ActionResult ShowComment(int id)
        {
            var url = Session["blog_url"].ToString();
            new DB().ExecuteQuery("Update BlogComments SET [Status] = 'Shown' where Id = '" + id + "'");
            return Redirect(url);
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
        // GET: Properties/Sub Plans
        [ActionName("Subscription Plans")]
        public ActionResult SubscriptionPlans()
        {           
            return View();
        }

        // GET: Properties/Contact Us/5
        
        public ActionResult ContactUs()
        {
            ViewBag.Title = "Contact Us";
            return View();
        }

        // POST: Properties/Contact Us/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(SiteMessage model)
        {
            return View();
        }
    }
}