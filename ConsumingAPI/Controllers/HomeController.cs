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
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = "http://localhost:49995/api/Households";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Read the response
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<HouseholdsViewModel>>(data);
                return View(result);
                //return RedirectToAction(nameof(HomeController.GetAll));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateHouseHold()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateHouseHold(HouseholdsViewModel formdata)
        {

            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = "http://localhost:49995/api/Households";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var name = formdata.Name;
            var description = formdata.Description;

            //Parameters list with KeyValue pair
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", name));
            parameters.Add(new KeyValuePair<string, string>("Description", description));

            //Encoding the parameters before sending to the API
            var encodedParameters = new FormUrlEncodedContent(parameters);

            //Calling the API and storing the response
            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                //Read the response
                var data = response.Content.ReadAsStringAsync().Result;

                //Convert the data back into an object
                var result = JsonConvert.DeserializeObject<Households>(data);
                return View(result);
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

            return View();
        }

        [HttpGet]
        public ActionResult EditHouseHold(int? id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = $"http://localhost:49995/api/Households/{id}";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<HouseholdsViewModel>(data);
            return View(result);
        }

        [HttpPost]
        public ActionResult EditHouseHold(int id, HouseholdsViewModel formdata)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = $"http://localhost:49995/api/Households/{id}";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var name = formdata.Name;
            var description = formdata.Description;
            var dateUpdated = DateTime.Now;

            //Parameters list with KeyValue pair
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", name));
            parameters.Add(new KeyValuePair<string, string>("Description", description));

            //Encoding the parameters before sending to the API
            var encodedParameters = new FormUrlEncodedContent(parameters);

            //Calling the API and storing the response
            var response = httpClient.PutAsync(url, encodedParameters).Result;

             if (response.StatusCode == System.Net.HttpStatusCode.Created)
             {
                //Read the response
                var data = response.Content.ReadAsStringAsync().Result;

                //Convert the data back into an object
                var result = JsonConvert.DeserializeObject<Households>(data);
             }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            return View();
        }

        [HttpGet]
        public ActionResult DisplayUsers(int? id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = $"http://localhost:49995/api/Households/DisplayUsers/{id}";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<DisplayUsersViewModel>>(data);
                return View(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return View();
        }

        [HttpGet]
        public ActionResult InviteUsers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InviteUsers(int? id, InviteUsersViewModel model)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;
            var url = "http://localhost:49995/api/Households/InviteUsers/{id}?email={email}";
            var email = model.Email;
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Email", email));

            var encodedValues = new FormUrlEncodedContent(parameters);
            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.PostAsync(url, encodedValues).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<InviteUsersViewModel>(data);
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult JoinHouseHold(int? id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = $"http://localhost:49995/api/Households/JoinHousehold/{id}";
            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "You are not Invited to this HouseHold.");
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<InviteUsersViewModel>(data);
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            var cookie = Request.Cookies["MyFirstCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Index");
            }

            var token = cookie.Value;

            var url = $"http://localhost:49995/api/Households/{id}";

            // HttpClient object to handle the comunication
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");
            var response = httpClient.DeleteAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "You are not Invited to this HouseHold.");
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<HouseholdsViewModel>(data);
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<APIErroData>(data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }
            return View();
        }

    }
}