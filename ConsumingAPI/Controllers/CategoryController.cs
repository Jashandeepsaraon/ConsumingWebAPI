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
    public class CategoryController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisplayCategory(int id)
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
                .GetAsync($"http://localhost:64310/api/Category/view/{id}")
                .Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var category = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);

                return View(category);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateCategory(int id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(int? id, CreateEditCategoryViewModel model)
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
                new KeyValuePair<string, string>("HouseholdId", Convert.ToString(model.Id)));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync("http://localhost:64310/api/Category/create",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayCategory", new { id });
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
                TempData["Message"] = "You are not Owner of this HousHold.";
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
        public ActionResult EditCategory(int id)
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
                .GetAsync($"http://localhost:64310/api/Category/ViewById/{id}")
                .Result;
            var a = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<CategoryViewModel>(data);

                //if (!result.IsOwner)
                //{
                //    return RedirectToAction(nameof(HomeController.Index), "Home");
                //}

                var editViewModel = new CreateEditCategoryViewModel();
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
        public ActionResult EditCategory(int id, CreateEditCategoryViewModel model)
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
                new KeyValuePair<string, string>("HouseholdId", Convert.ToString(model.Id)));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/Category/edit/{id}",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayCategory", new { id });
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
                .PostAsync($"http://localhost:64310/api/Category/Delete/{id}",
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
                TempData["Message"] = "It looks like the Category is not found";
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