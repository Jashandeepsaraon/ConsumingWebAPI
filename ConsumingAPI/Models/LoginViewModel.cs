using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models
{
    public class LoginViewModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}