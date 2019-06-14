using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}