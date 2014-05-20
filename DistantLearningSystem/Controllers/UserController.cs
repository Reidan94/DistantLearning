using System;
using System.Web;
using System.Web.Mvc;
using DistantLearningSystem.Models;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.Managers;
using DistantLearningSystem.Models.LogicModels.Services;
using DistantLearningSystem.Models.LogicModels.ViewModels;

namespace DistantLearningSystem.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user == null)
                return RedirectToAction("LogIn");
            return View(user);
        }

        public ActionResult Logout()
        {
            var userCookie = new HttpCookie("UserId")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            var keyCookie = new HttpCookie("Key")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            var typeCookie = new HttpCookie("UserType")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            HttpContext.Response.Cookies.Set(userCookie);
            HttpContext.Response.Cookies.Set(keyCookie);
            HttpContext.Response.Cookies.Set(typeCookie);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogIn(int? result)
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user != null)
                return RedirectToAction("Index");
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        [HttpPost]
        public ActionResult ManageLogIn(LoginModel loginModel)
        {
            UserModel user;
            ProcessResult result = DataManager.Authentification.LogInUser(loginModel, out user);
            if (result.Succeeded && user != null)
            {
                SetUser((UserModel)user, user.Password);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("LogIn",
                "User",
                new
                {
                    result = result.Id
                });
        }

        public ActionResult Registration(int? result)
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user != null)
                return RedirectToAction("Index");
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        public ActionResult RegistrateStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ManageLecturerRegistration(Lecturer registrationModel, HttpPostedFileBase imageUpload)
        {
            ProcessResult result = DataManager.
                Lecturer.
                RegistrateLecturer(HttpContext,
                registrationModel,
                Server,
                imageUpload);

            if (result.Succeeded)
                return RedirectToAction("Registration", "User", new { result = result.Id });
            return View();
        }

        [HttpPost]
        public ActionResult ManageStudentRegistration(
            string Name, string LastName, string Login,
            string Email, string Password, HttpPostedFileBase imageUpload)
        {
            ProcessResult result = DataManager.
                Student.
                RegistrateStudent(HttpContext,
                new Student
                {
                    Name = Name,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Login = Login
                },
                Server,
                imageUpload);

            if (!result.Succeeded)
                return RedirectToAction("Registration", "User", new { result = result.Id });
            return View();
        }

        public ActionResult Confirm(string hash)
        {
            if (DataManager.ConfirmRegistration(hash))
                ViewBag.Message = "Регистрация подтверждена";
            else return RedirectToAction("Error");
            return View();
        }

        private void SetUser(UserModel user, string hashedKey)
        {
            if (user == null) return;
            var cookieUser = new HttpCookie("UserId")
            {
                Value = Convert.ToString(user.Id),
                Expires = DateTime.MaxValue
            };

            var cookieKey = new HttpCookie("Key")
            {
                Value = hashedKey,
                Expires = DateTime.MaxValue
            };

            var cookieuserType = new HttpCookie("UserType")
            {
                Value = ((int)user.UserType).ToString(),
                Expires = DateTime.MaxValue
            };

            HttpContext.Response.Cookies.Remove("UserId");
            HttpContext.Response.Cookies.Remove("Key");
            HttpContext.Response.Cookies.Remove("UserType");

            HttpContext.Response.SetCookie(cookieUser);
            HttpContext.Response.SetCookie(cookieKey);
            HttpContext.Response.SetCookie(cookieuserType);
        }

        //public ActionResult Bookmarks()
        //{
        //    User user = DefineUser(HttpContext);
        //    if (user == null)
        //        return RedirectToAction("LogIn");
        //    Culture culture = DefineLanguage(HttpContext);
        //    ViewBag.Culture = culture;
        //    return View(DataManager.Routes.GetUserBookmarks(user.Id, culture: culture));
        //}
    }
}