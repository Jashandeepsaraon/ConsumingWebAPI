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
    }
}