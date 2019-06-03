using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumingAPI.Models.Domain
{
    public class InviteUsersViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}