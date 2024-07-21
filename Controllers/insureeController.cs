using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carinsurance.Models;

namespace carinsurance.Controllers
{
    public class insureeController : Controller
    {
        private insuranceEntities db = new insuranceEntities();

        // GET: insuree
        public ActionResult Index()
        {
            return View(db.insurees.ToList());
        }

        // GET: insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insuree insuree = db.insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.CalculateQuote();
                db.insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insuree insuree = db.insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.CalculateQuote();
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            insuree insuree = db.insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            insuree insuree = db.insurees.Find(id);
            db.insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: insuree/Admin
        public ActionResult Admin()
        {
            var insurees = db.insurees.Select(i => new
            {
                i.FirstName,
                i.LastName,
                i.EmailAddress,
                i.Quote
            }).ToList().Select(i => new insuree
            {
                FirstName = i.FirstName,
                LastName = i.LastName,
                EmailAddress = i.EmailAddress,
                Quote = i.Quote
            }).ToList();
            return View(insurees);
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

namespace carinsurance.Models
{
    public partial class insuree
    {
        public void CalculateQuote()
        {
            // Base quote
            decimal quote = 50m;

            // Age-based adjustment
            int age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age <= 18)
            {
                quote += 100;
            }
            else if (age >= 19 && age <= 25)
            {
                quote += 50;
            }
            else
            {
                quote += 25;
            }

            // Car year adjustment
            if (CarYear < 2000)
            {
                quote += 25;
            }
            else if (CarYear > 2015)
            {
                quote += 25;
            }

            // Car make and model adjustment
            if (CarMake.ToLower() == "porsche")
            {
                quote += 25;
                if (CarModel.ToLower() == "911 carrera")
                {
                    quote += 25;
                }
            }

            // Speeding tickets adjustment
            quote += SpeedingTickets * 10;

            // DUI adjustment
            if (DUI)
            {
                quote *= 1.25m;
            }

            // Coverage type adjustment
            if (CoverageType)
            {
                quote *= 1.50m;
            }

            // Update the Quote property
            Quote = quote;
        }
    }
}
