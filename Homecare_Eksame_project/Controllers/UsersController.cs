using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Homecare_Eksame_project.Models;

namespace Homecare_Eksame_project.Controllers
{
    public class UsersController : Controller
    {
        private HomecareEntities db = new HomecareEntities();


        //Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "userID,userFullName,password")] User user)
        {
            if (ModelState.IsValid)
            {
                var hashed_password = HashPassword(user.password);
                var username = user.userFullName.ToLower().Trim();
                var authUser = db.Users.FirstOrDefault(row => row.userFullName.ToLower() == username && row.password == hashed_password);
                if (authUser != null)
                {
                    Session["UserID"] = authUser.userID.ToString();
                    Session["Username"] = authUser.userFullName.ToString();
                    string role_id = (string)(Session["RoleID"] = authUser.Role.roleName.ToLower().ToString());

                    FormsAuthentication.SetAuthCookie(user.userFullName, false);
                    return RedirectToAction("Index", role_id == "admin" ? "Employees" : "DrivingPlans");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username and/or password");
                }
            }

            return View();
        }

        //Logout
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("index","Users");
        }

        //LoggedIn
        public ActionResult LoggedIn()
        {
            if (Session["UserID"] != null)
            {
                return View(User);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Users
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult UserList()
        {
            var users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.roleID = new SelectList(db.Roles, "roleID", "roleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "userID,userFullName,password,roleID")] User user)
        {
            if (ModelState.IsValid)
            {
                user.password = HashPassword(user.password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("UserList");
            }

            ViewBag.roleID = new SelectList(db.Roles, "roleID", "roleName", user.roleID);
            return View(user);
        }


        // GET: Users/Edit/5
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.roleID = new SelectList(db.Roles, "roleID", "roleName", user.roleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "userID,userFullName,password,roleID")] User user)
        {
            if (ModelState.IsValid)
            {
                user.password = HashPassword(user.password);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            ViewBag.roleID = new SelectList(db.Roles, "roleID", "roleName", user.roleID);
            return View(user);
        }

        // GET: Users/Delete/5
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Filters.CustomeAuthorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("UserList");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        static byte[] password_salt = Encoding.UTF8.GetBytes("#_PW_SALT_#");
        private static string HashPassword(string value)
        {
            return Encoding.UTF8.GetString(Hash(value, password_salt));
        }
        private static byte[] Hash(string value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        private static byte[] Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();
           

            return new SHA256Managed().ComputeHash(saltedValue);
        }
    }
}
