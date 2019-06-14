using ConsumingAPI.Models;
using ConsumingAPI.Models.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ConsumingAPI.Controllers
{
    public class BankAccountController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisplayBankAccount(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.HouseHoldId = id;
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .GetAsync($"http://localhost:64310/api/BankAccount/view/{id}")
                .Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var bankAccount = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(data);

                return View(bankAccount);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateBankAccount(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateBankAccount(int? id, CreateEditBankAccountViewModel model)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Name", model.Name));
            parameters.Add(
                new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(
                new KeyValuePair<string, string>("Balance", model.Balance.ToString()));
            parameters.Add(
                new KeyValuePair<string, string>("HouseHoldId", Convert.ToString(id)));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync("http://localhost:64310/api/BankAccount/create",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayBankAccount", new { id });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<APIErroData>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult EditBankAccount(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .GetAsync($"http://localhost:64310/api/BankAccount/ViewById/{id}")
                .Result;
            var a = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<BankAccountViewModel>(data);

                //if (!result.IsOwner)
                //{
                //    return RedirectToAction(nameof(HomeController.Index), "Home");
                //}

                var editViewModel = new CreateEditBankAccountViewModel();
                editViewModel.Name = result.Name;
                editViewModel.Description = result.Description;
                return View(editViewModel);
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpPost]
        public ActionResult EditBankAccount(int id, CreateEditBankAccountViewModel model)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Name", model.Name));
            parameters.Add(
                new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(
                new KeyValuePair<string, string>("HouseholdId", Convert.ToString(id)));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/BankAccount/edit/{id}",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayBankAccount", new { id });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<APIErroData>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        public ActionResult UpdateBalance(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/BankAccount/UpdateBalance/{id}",
                    null).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<APIErroData>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like the BankAccount is not found";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/BankAccount/Delete/{id}",
                    null).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<APIErroData>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like the BankAccount is not found";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }
    }
}