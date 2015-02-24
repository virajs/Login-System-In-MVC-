using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcComputer.Models
{
    public class LoginModel
    {
        [Key]
        [Required(ErrorMessage="Username Required")]
        public string username { get; set; }

        [Required(ErrorMessage="Password Required")]
        [DataType(DataType.Password)]
        public string  password { get; set; }

        public string provider { get; set; }
             
    }

    public class ExternalLoginViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
        public string provider { get; set; }
    }

}