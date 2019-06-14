using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models
{
    public class BankAccountViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Balance { get; set; }
        public bool IsOwner { get; set; }
    }
}