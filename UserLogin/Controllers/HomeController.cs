
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserLogin.Models;

namespace UserLogin.Controllers
{
    public class HomeController : Controller
    {
        public EmployeeDBEntities db = new EmployeeDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Employee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Employee(Employee emp)
        {
            string EncryptPassword = Cryptography.Encrypt(emp.UserPass);
            if (ModelState.IsValid)
            {
                emp.UserPass = EncryptPassword;
                db.Employees.Add(emp);
                db.SaveChanges();
            }
            return View("Login");
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee emp)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string PasswordQuery = (from data in db.Employees where data.UserName == emp.UserName select data.UserPass).FirstOrDefault();

                    string d = Convert.ToString(PasswordQuery);
                    string DecryptPassword = Cryptography.Decrypt(d);
                    var UsernameQuery = (from UserList in db.Employees where UserList.UserName == emp.UserName select new { UserList.UserName, UserList.FirstName }).ToList();
                    
                    if (UsernameQuery.ToString() != emp.UserName)
                    {
                        ModelState.AddModelError(string.Empty, " Credentials Invalid");
                    }
                    if (DecryptPassword != null && DecryptPassword.Equals(emp.UserPass))
                    {
                        Session["FirstName"] = UsernameQuery.FirstOrDefault().FirstName;
                        return RedirectToAction("Welcome", "Home");
                    }

                  
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
            }


            return View(emp);
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult KendoData()
        {
            ViewBag.Massage = "Kendo-UI Grid Page";


            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<Employee> emp = db.Employees;

            DataSourceResult result = emp.ToDataSourceResult(request, data => new Models.Employee
            {
                EmpID = data.EmpID,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                UserPass = data.UserPass,
                Email = data.Email,
                Addr = data.Addr,
                DOB = data.DOB,
                PhoneNo = data.PhoneNo,
                Gender = data.Gender

            });

            return Json(result);
            //return View(result);
        }
    }
}