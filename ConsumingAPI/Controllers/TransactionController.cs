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
    public class TransactionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisplayTransaction(int bankAccountId, int houseHoldId)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.BankAccountId = bankAccountId;
            ViewBag.HouseHoldId = houseHoldId;
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .GetAsync($"http://localhost:64310/api/Transaction/view/{bankAccountId}")
                .Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var transactions = JsonConvert.DeserializeObject<List<TransactionViewModel>>(data);

                return View(transactions);
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
        }

        [HttpGet]
        public ActionResult CreateTransaction(int bankAccountId, int houseHoldId)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var url = $"http://localhost:64310/api/Category/view/{houseHoldId}";
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .GetAsync(url)
                .Result;
            CreateEditTransactionViewModel model = null;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                model = new CreateEditTransactionViewModel
                {
                    CategoryList = categories.Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }).ToList()
                };
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
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTransaction(int? bankAccountId,  int houseHoldId, CreateEditTransactionViewModel model)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var url = $"http://localhost:64310/api/Category/view/{houseHoldId}";
                var httpClient1 = new HttpClient();

                httpClient1.DefaultRequestHeaders.Add("Authorization",
                    $"Bearer {cookie.Value}");

                var response1 = httpClient1
                    .GetAsync(url)
                    .Result;

                if (response1.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = response1.Content.ReadAsStringAsync().Result;

                    var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                    model.CategoryList = categories.Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }).ToList();
                    
                }
                return View(model);
            }

            var parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(
                new KeyValuePair<string, string>("Title", model.Title));
            parameters.Add(
                new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(
                new KeyValuePair<string, string>("Amount", Convert.ToString(model.Amount)));
            parameters.Add(
                new KeyValuePair<string, string>("TransactionDate", Convert.ToString(model.TransactionDate)));
            parameters.Add(
                new KeyValuePair<string, string>("BankAccountId", Convert.ToString(bankAccountId)));
            parameters.Add(
                new KeyValuePair<string, string>("CategoryId", Convert.ToString(model.CategoryId)));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync("http://localhost:64310/api/Transaction/create",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayTransaction", new { bankAccountId, houseHoldId });
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
        public ActionResult EditTransaction(int id, int houseHoldId)
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
                .GetAsync($"http://localhost:64310/api/Transaction/ViewById/{id}")
                .Result;
            var a = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<TransactionViewModel>(data);
                var categoriesResponse = httpClient
                .GetAsync($"http://localhost:64310/api/Category/view/{houseHoldId}")
                .Result;
                var categoriesData = categoriesResponse.Content.ReadAsStringAsync().Result;
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesData);
                
                //if (!result.IsOwner)
                //{
                //    return RedirectToAction(nameof(HomeController.Index), "Home");
                //}

                var editViewModel = new CreateEditTransactionViewModel();
                editViewModel.Title = result.Title;
                editViewModel.Description = result.Description;
                editViewModel.TransactionDate = result.TransactionDate;
                editViewModel.Amount = result.Amount;
                editViewModel.CategoryList = categories.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
                return View(editViewModel);
            }
            else
            {
                //Create a log for the error message
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpPost]
        public ActionResult EditTransaction(int id, int houseHoldId, int bankAccountId, CreateEditTransactionViewModel model)
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
                new KeyValuePair<string, string>("Title", model.Title));
            parameters.Add(
                new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(
                new KeyValuePair<string, string>("BankAccountId", Convert.ToString(model.BankAccountId)));
            parameters.Add(
                new KeyValuePair<string, string>("CategoryId", Convert.ToString(model.CategoryId)));
            parameters.Add(
                new KeyValuePair<string, string>("Amount", Convert.ToString(model.Amount)));
            parameters.Add(
                new KeyValuePair<string, string>("TransactionDate", Convert.ToString(model.TransactionDate)));
            var encodedParameters = new FormUrlEncodedContent(parameters);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {cookie.Value}");

            var response = httpClient
                .PostAsync($"http://localhost:64310/api/Transaction/edit/{id}",
                    encodedParameters)
                .Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("DisplayTransaction", new { bankAccountId, houseHoldId });
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
                .PostAsync($"http://localhost:64310/api/Transaction/Delete/{id}",
                    null).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                TempData["Message"] = "You Successfully deleted the Transaction";
                return RedirectToAction(nameof(HomeController.Index), "Home");
                //return RedirectToAction("DisplayTransaction", new { ViewBag.BankAccountId, houseHoldId });
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

                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like the Transaction is not found";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
        }

        public ActionResult Void(int id)
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
                .PostAsync($"http://localhost:64310/api/Transaction/Void/{id}",
                    null).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(HomeController.Index), "Home");
                //return RedirectToAction("DisplayTransaction", new { ViewBag.BankAccountId, houseHoldId });
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
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like the Transaction is not found";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                //Create a log for the error message
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
        }
    }
}