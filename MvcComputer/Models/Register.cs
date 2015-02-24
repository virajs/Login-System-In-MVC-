using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.DynamicData;
namespace MvcComputer.Models
{
    [TableName("Register")]
    public class RegisterModel
    {
        [Key]
        [Required(ErrorMessage="Use name required")]
        [Display(Name="User name")]
        public string username { get; set; }

        [Required(ErrorMessage="Email required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public  string email { get; set; }

        [Required(ErrorMessage="Password required")]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        [MinLength(6)]
        public string password { get; set; }


        [Required(ErrorMessage="Password dint match")]
        [DataType(DataType.Password)]
        [Compare("password",ErrorMessage="Password Doesnot match")]
        [Display(Name="Please confirm the password")]
        [MinLength(6)]
        public string comparepwd  { get; set; }


    }

    public class ForgotPwdmodel
    {
        [Key]
        [Required(ErrorMessage = "Use name required")]
        [Display(Name = "User name")]
        public string username { get; set; }
  
        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }


        [Required(ErrorMessage = "Password dint match")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password Doesnot match")]
        [Display(Name = "Please confirm the password")]
        public string comparepwd { get; set; }


    }

    [TableName("ApplicationUser")]
    public class ApplicationUser : IdentityUser
    {

    }

    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
            
        }

        public static MyDbContext Create()
        {
            return new MyDbContext();
        }

        
}
}