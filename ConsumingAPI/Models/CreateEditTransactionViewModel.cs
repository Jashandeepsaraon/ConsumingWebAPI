using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsumingAPI.Models
{
    public class CreateEditTransactionViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        [Required]
        public int BankAccountId { get; set; }
    }
}