using LibraryData.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.Models.Login
{
        public class LogInInfo
        {
            [DisplayName("Card Id: ")]
            [Required(ErrorMessage = "This field is required.")]
            public string CardID { get; set; }
            [DisplayName("Password: ")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "This field is required.")]
            public string Password { get; set; }            
            public string LoginErrorMessage { get; set; }
        }
}