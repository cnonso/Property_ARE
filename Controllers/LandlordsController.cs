using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyManagerWeb.Models;
using System.IO;
using Microsoft.AspNet.Identity;

namespace PropertyManagerWeb.Controllers
{ 
    public class LandlordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Landlords
        [Authorize(Roles ="Admin")]
        public ActionResult Index(int page = 1)
        {
            var landlords = db.Landlords.Include(l => l.User).Where(l=>l.LandlordID != "LA").OrderByDescending(c => c.LandlordID)
                                          .Skip((page - 1) * 5)
                                          .Take(5)
                                          .ToList();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = 5;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Landlords.Where(l => l.LandlordID != "LA").Count() / 5);

            return View(landlords);
        }

        // GET: Landlords/Details/5
        [Authorize]
        [ActionName("Profile")]
        public ActionResult LandlordProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Landlord landlord = db.Landlords.Find(id);
            if (landlord == null)
            {
                return HttpNotFound();
            }
            return View(landlord);
        }

        // GET: Landlords/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Landlords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Landlord landlord)
        {
            if (ModelState.IsValid)
            {
                db.Landlords.Add(landlord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            return View(landlord);
        }


        // GET: Landlords/Edit/5 
        [Authorize]
        public ActionResult Edit()
        {
            string userid = User.Identity.GetUserId();
            int landlord_id = db.Landlords.Where(l => l.ApplicationUserId == userid).First().IDforLand;

            //int idDecrypted = int.Parse(AppHandler.Decrypt(id));
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Landlord landlord = db.Landlords.Find(landlord_id);
            if (landlord == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            LandlordViewModel model = new LandlordViewModel
            {
                IDforLand = landlord.IDforLand,
                Title = landlord.Title,
                Surname = landlord.Surname,
                OtherNames = landlord.OtherNames,
                CorporateName = landlord.CorporateName,
                Address = landlord.Address,
                Phone = landlord.Phone,
                RegDate = landlord.RegDate,
                LandlordID = landlord.LandlordID,
                Status = landlord.Status,
                ImageMainUrl = landlord.ImageMainUrl,
                Facebook = landlord.Facebook,
                Instagram = landlord.Instagram,
                MiniBiography = landlord.MiniBiography,
                HappyClients = landlord.HappyClients,
                YearsOfExperience = landlord.YearsOfExperience,
                ApplicationUserId = landlord.ApplicationUserId
                
            };
            ViewBag.Notification = PropertiesController.notification;
            return View(model);
        }

        // POST: Landlords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LandlordViewModel model)
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
                if (model.ImageMain != null || model.ImageMain.ContentLength != 0)
                {
                    if (!validImageTypes.Contains(model.ImageMain.ContentType))
                    {
                        ModelState.AddModelError("ImageMain", "Please choose either a GIF, JPG or PNG image.");
                    }
                }
            }
            catch (Exception ex)
            { }

            if (ModelState.IsValid)
            {
                Landlord landlord = new Landlord
                {
                    IDforLand = model.IDforLand,
                    Title = model.Title,
                    Surname = model.Surname,
                    OtherNames = model.OtherNames,
                    CorporateName = model.CorporateName,
                    Address = model.Address,
                    Phone = model.Phone,
                    RegDate = model.RegDate,
                    LandlordID = model.LandlordID,
                    Status = model.Status,
                    MiniBiography = model.MiniBiography,
                    Facebook = model.Facebook,
                    Instagram = model.Instagram,
                    HappyClients = model.HappyClients,
                    YearsOfExperience = model.YearsOfExperience,
                    ApplicationUserId = model.ApplicationUserId

                };

                try
                {
                    if (model.ImageMain.ContentLength > 0)
                    {
                        var uploadDir = "/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), model.Phone + "_" + model.ImageMain.FileName);
                        var imageUrl = Path.Combine(uploadDir, model.Phone + "_" + model.ImageMain.FileName);
                        model.ImageMain.SaveAs(imagePath);
                        landlord.ImageMainUrl = imageUrl;
                    }
                }
                catch (Exception ex)
                {
                    landlord.ImageMainUrl = model.ImageMainUrl;
                }

                db.Entry(landlord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult VerifyAgent(string id)
        {
            string decrypted_id = AppHandler.Decrypt(id);
            new DB().ExecuteQuery("Update Landlords set [Status] = 'Active Verified' where IDforLand = '" + decrypted_id + "'");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UnverifyAgent(string id)
        {
            string decrypted_id = AppHandler.Decrypt(id);
            new DB().ExecuteQuery("Update Landlords set [Status] = 'Active Unverified' where IDforLand = '" + decrypted_id + "'");
            return RedirectToAction("Index");
        }

        // GET: Landlords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Landlord landlord = db.Landlords.Find(id);
            if (landlord == null)
            {
                return HttpNotFound();
            }
            return View(landlord);
        }

        // POST: Landlords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Landlord landlord = db.Landlords.Find(id);
            db.Landlords.Remove(landlord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static string messageSendingStatus = "";

        //POST: Leave message for Agent
        public ActionResult DropMessage(MultipleModels model, string propertyName, string landlordID)
        {
            var url = Session["url"].ToString();
            var message = new Message
            {
                SenderName = model.MessageViewModel.SenderName,
                SenderPhoneNo = model.MessageViewModel.SenderPhoneNo,
                Email = model.MessageViewModel.Email,
                MessageBody = model.MessageViewModel.MessageBody,
                PropertyName = Request.Form["Property_Name"].ToString(),
                LandlordID = Request.Form["Landlord_ID"].ToString(),
                DateSent = DateTime.Now.ToString(),
            };

            db.Messages.Add(message);
            db.SaveChanges();

            return Redirect(url);
        }


        //Get messages
        [Authorize]
        [ActionName("My-Messages")]
        public ActionResult MyMessages(int page = 1) 
        {
            var userid = User.Identity.GetUserId();
            string landlordid = db.Landlords.Where(l => l.ApplicationUserId == userid).First().LandlordID;
             
            var messages = db.Messages.Where(c => c.LandlordID == landlordid).ToList()
                .OrderByDescending(c => c.LandlordID)
                                        .Skip((page - 1) * 10)
                                        .Take(10)
                                        .ToList();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = 5;
            ViewBag.TotalPages = Math.Ceiling((Double)db.Messages.Where(c => c.LandlordID == landlordid).Count() / 10);
            

            return View(messages);
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
