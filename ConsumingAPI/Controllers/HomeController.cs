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
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult GetAll()
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
                .GetAsync("http://localhost:64310/api/household/view")
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var households = JsonConvert.DeserializeObject<List<HouseholdsViewModel>>(data);

                return View(households);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public ActionResult DisplayUsers(int id)
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
                .GetAsync($"http://localhost:64310/api/household/ViewUsers/{id}")
                .Result;

                var data = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var users = JsonConvert.DeserializeObject<List<DisplayUsersViewModel>>(data);     
                return View(users);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public ActionResult SingleHousehold(int id)
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
                .GetAsync($"http://localhost:64310/api/household/ViewById/{id}")
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<HouseholdsViewModel>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction("Index");
                }
                return View(result);
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateEditHouseholdViewModel model)
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

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync("http://localhost:64310/api/household/create",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
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
        public ActionResult Edit(int id)
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
                .GetAsync($"http://localhost:64310/api/household/ViewById/{id}")
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<HouseholdsViewModel>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction("Index");
                }

                var editViewModel = new CreateEditHouseholdViewModel();
                editViewModel.Name = result.Name;
                editViewModel.Description = result.Description;
                editViewModel.DateUpdated = DateTime.Now;
                return View(editViewModel);
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, CreateEditHouseholdViewModel model)
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

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/household/edit/{id}",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Invite(int id)
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
                .GetAsync($"http://localhost:64310/api/household/ViewById/{id}")
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<HouseholdsViewModel>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Invite(int id, InviteUsersViewModel model)
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
                new KeyValuePair<string, string>("Email", model.Email));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/household/invite/{id}",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
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

                return RedirectToAction("Error");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
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
        public ActionResult Join()
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
                .GetAsync($"http://localhost:64310/api/household/GetInvites")
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<List<InviteViewModel>>(data);

                return View(result);
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Join(int id)
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
                .PostAsync($"http://localhost:64310/api/household/join/{id}",
                    null)
                .Result;
            var data1 = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Join", new { id = id });
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

                return RedirectToAction("Error"); ;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like this household was deleted";
                return RedirectToAction("Index");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult Leave(int id)
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
                .PostAsync($"http://localhost:64310/api/household/leave/{id}",
                    null)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
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

                return RedirectToAction("Error");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like this household was deleted";
                return RedirectToAction("Index");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction("Error");
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
                .PostAsync($"http://localhost:64310/api/household/Delete/{id}",
                    null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                TempData["Message"] = "You Successfully deleted the HouseHold";
                return RedirectToAction("GetAll");
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

                return RedirectToAction("Error");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like the Household is not found";
                return RedirectToAction("Index");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction("Error");
            }
        }
    }
}