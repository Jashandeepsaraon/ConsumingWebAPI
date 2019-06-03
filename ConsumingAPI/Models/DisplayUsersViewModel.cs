using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models
{
    public class DisplayUsersViewModel
    {
        public int Id { get; set; }
        public List<string> Users { get; set; }
        public List<string> Categories { get; set; }
    }
}