using ConsumingAPI.Models;
using ConsumingAPI.Models.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ConsumingAPI.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel formdata)
        {
            var url = "http://localhost:49995/token";

            var userName = formdata.UserName;
            var password = formdata.Password;
            var grantType = "password";

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", userName));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("","Either UserName or Password is incorrect.");
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<UserLogin>(data);

                // Session["Token"] = result.AccessToken;

                var cookie = new HttpCookie("MyFirstCookie",
                    result.AccessToken);
                Response.Cookies.Add(cookie);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data1 = response.Content.ReadAsStringAsync().Result;
                var result1 = JsonConvert.DeserializeObject<APIErroData>(data1);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var url = "http://localhost:49995/api/Account/Register";

            var email = model.Email;
            var password = model.Password;
            var confirmPassword = model.ConfirmPassword;

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Email", email));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AccountController.Login));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data1 = response.Content.ReadAsStringAsync().Result;
                var result1 = JsonConvert.DeserializeObject<APIErroData>(data1);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            //var data = response.Content.ReadAsStringAsync().Result;

            //var result = JsonConvert.DeserializeObject<UserLogin>(data);
            return RedirectToAction(nameof(AccountController.Register));
        }

        public ActionResult LogOut()
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;
            var newcookie = new HttpCookie("MyFirstCookie",
                    null);
            newcookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(newcookie);
            return RedirectToAction(nameof(AccountController.Login));
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var url = "http://localhost:49995/api/Account/ChangePassword";
            var cookie = Request.Cookies["MyFirstCookie"];
            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
               $"Bearer {token}");

            var oldPassword = model.OldPassword;
            var newPassword = model.NewPassword;
            var confirmPassword = model.ConfirmPassword;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("OldPassword", oldPassword));
            parameters.Add(new KeyValuePair<string, string>("NewPassword", newPassword));
            parameters.Add(new KeyValuePair<string, string>("ConfirmPassword", confirmPassword));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AccountController.LogOut));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data1 = response.Content.ReadAsStringAsync().Result;
                var result1 = JsonConvert.DeserializeObject<APIErroData>(data1);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return RedirectToAction(nameof(AccountController.LogOut));
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var url = "http://localhost:49995/api/Account/ForgotPassword";
            var httpClient = new HttpClient();
            var email = model.Email;
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Email", email));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AccountController.LogOut));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data1 = response.Content.ReadAsStringAsync().Result;
                var result1 = JsonConvert.DeserializeObject<APIErroData>(data1);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var data1 = response.Content.ReadAsStringAsync().Result;
                var result1 = JsonConvert.DeserializeObject<APIErroData>(data1);
                return View("Error");
            }
            return RedirectToAction(nameof(AccountController.LogOut));
        }
    }
}