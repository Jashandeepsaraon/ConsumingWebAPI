﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models
{
    public class CreateEditHouseholdViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public CreateEditHouseholdViewModel()
        {
            DateCreated = DateTime.Now;
        }
    }
}