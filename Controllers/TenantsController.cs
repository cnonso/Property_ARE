using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyManagerWeb.Models;

namespace PropertyManagerWeb.Controllers
{
    public class TenantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tenants
        public ActionResult Index()
        {
            var tenants = db.Tenants.Include(t => t.User);
            return View(tenants.ToList());
        }

        // GET: Tenants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }

        // GET: Tenants/Create
        public ActionResult Create()
        {
            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: Tenants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenantID,AccountNo,LandlordID,Title,Surname,OtherNames,Address,Phone,RegDate,PropertyID,StartDate,EndDate,Status,NoOfProperty,Rent,PropertyNo,RentPaymentStatus,CycleCount,EndOfGrace,NoOfPropertyUnits,ApplicationUserId")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                db.Tenants.Add(tenant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", tenant.ApplicationUserId);
            return View(tenant);
        }

        // GET: Tenants/Edit/5
        public ActionResult Edit(string id)
        {
            int idDecrypted = int.Parse(AppHandler.Decrypt(id));
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Tenant tenant = db.Tenants.Find(idDecrypted);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", landlord.ApplicationUserId);
            TenantViewModel model = new TenantViewModel
            {
                TenantID = tenant.TenantID,
                AccountNo = tenant.AccountNo,
                LandlordID = tenant.LandlordID,
                Title = tenant.Title,
                Surname = tenant.Surname,
                OtherNames = tenant.OtherNames,
                Address = tenant.Address,
                Phone = tenant.Phone,
                RegDate = tenant.RegDate,
                PropertyID = tenant.PropertyID,
                StartDate = tenant.StartDate,
                EndDate = tenant.EndDate,
                Status = tenant.Status,
                NoOfProperty = tenant.NoOfProperty,
                Rent=tenant.Rent,
                PropertyNo = tenant.PropertyNo,
                RentPaymentStatus = tenant.RentPaymentStatus,
                CycleCount = tenant.CycleCount,
                EndOfGrace = tenant.EndOfGrace,
                NoOfPropertyUnits = tenant.NoOfPropertyUnits,
                ApplicationUserId = tenant.ApplicationUserId                
            };
            return View(model);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TenantViewModel model)
        {
            if (ModelState.IsValid)
            {
                Tenant tenant = new Tenant
                {
                    TenantID = model.TenantID,
                    AccountNo = model.AccountNo,
                    LandlordID = model.LandlordID,
                    Title = model.Title,
                    Surname = model.Surname,
                    OtherNames = model.OtherNames,
                    Address = model.Address,
                    Phone = model.Phone,
                    RegDate = model.RegDate,
                    PropertyID = model.PropertyID,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = model.Status,
                    NoOfProperty = model.NoOfProperty,
                    Rent = model.Rent,
                    PropertyNo = model.PropertyNo,
                    RentPaymentStatus = model.RentPaymentStatus,
                    CycleCount = model.CycleCount,
                    EndOfGrace = model.EndOfGrace,
                    NoOfPropertyUnits = model.NoOfPropertyUnits,
                    ApplicationUserId = model.ApplicationUserId
                };
                
                db.Entry(tenant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", tenant.ApplicationUserId);
            return View(model);
        }

        // GET: Tenants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tenant tenant = db.Tenants.Find(id);
            db.Tenants.Remove(tenant);
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
