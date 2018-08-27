using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Homecare_Eksame_project.Models;

namespace Homecare_Eksame_project.Controllers
{
    [Filters.CustomeAuthorize(Roles = "admin,driver")]
    public class DrivingPlansController : Controller
    {
        private HomecareEntities db = new HomecareEntities();

        // GET: DrivingPlans
        public ActionResult Index(int? customerID, int? employeeID)
        {
            var drivingPlans = db.DrivingPlans.Include(d => d.Customer).Include(d => d.Employee);
            // If the logged in user is a driver, show the driving plans of this driver only.
            if ((string)Session["RoleID"] == "driver")
            {
                int current_user_id = int.Parse((string)Session["UserID"]);
                drivingPlans = drivingPlans.Where(x => x.Employee.userID == current_user_id);
            }
            // If the customerID param is included, show the driving plans of this cusomter.
            if (customerID.HasValue)
                drivingPlans = drivingPlans.Where(x => x.customerID == customerID.Value);

            // If the employeeID param is included, show the driving plans of this employee.
            if (employeeID.HasValue)
                drivingPlans = drivingPlans.Where(x => x.employeeID == employeeID.Value);

            return View(drivingPlans.ToList());
        }

        // GET: DrivingPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrivingPlan drivingPlan = db.DrivingPlans.Find(id);
            if (drivingPlan == null)
            {
                return HttpNotFound();
            }
            return View(drivingPlan);
        }

        // GET: DrivingPlans/Create
        public ActionResult Create(int? employeeID, int? customerID)
        {

            ViewBag.customerID = new SelectList(db.Customers, "customerID", "customerFullName");
            ViewBag.employeeID = new SelectList(db.Employees.Where(x => x.User.roleID == 2), "employeeID", "employeeFullName");

            // Create the empty model to use in case an employee ID or a customer ID is included in the query string param.
            var model = new DrivingPlan();
            // if the employee ID has value, pre-populate the value in the model.
            if (employeeID.HasValue) model.employeeID = employeeID.Value;
            // if the customer ID has value, pre-populate the value in the model.
            if (customerID.HasValue) model.customerID = customerID.Value;

            return View(model);
        }

        // POST: DrivingPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "drivingID,customerID,employeeID,drivingDate,isDone")] DrivingPlan drivingPlan)
        {
            if (ModelState.IsValid)
            {
                db.DrivingPlans.Add(drivingPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.customerID = new SelectList(db.Customers, "customerID", "customerFullName", drivingPlan.customerID);
            ViewBag.employeeID = new SelectList(db.Employees.Where(x => x.User.roleID == 2), "employeeID", "employeeFullName", drivingPlan.employeeID);
            return View(drivingPlan);
        }

        // GET: DrivingPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrivingPlan drivingPlan = db.DrivingPlans.Find(id);
            if (drivingPlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.customerID = new SelectList(db.Customers, "customerID", "customerFullName", drivingPlan.customerID);
            ViewBag.employeeID = new SelectList(db.Employees, "employeeID", "employeeFullName", drivingPlan.employeeID);
            return View(drivingPlan);
        }

        // POST: DrivingPlans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "drivingID,customerID,employeeID,drivingDate,isDone")] DrivingPlan drivingPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(drivingPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.customerID = new SelectList(db.Customers, "customerID", "customerFullName", drivingPlan.customerID);
            ViewBag.employeeID = new SelectList(db.Employees, "employeeID", "employeeFullName", drivingPlan.employeeID);
            return View(drivingPlan);
        }

        // GET: DrivingPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DrivingPlan drivingPlan = db.DrivingPlans.Find(id);
            if (drivingPlan == null)
            {
                return HttpNotFound();
            }
            return View(drivingPlan);
        }

        // POST: DrivingPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DrivingPlan drivingPlan = db.DrivingPlans.Find(id);
            db.DrivingPlans.Remove(drivingPlan);
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
