using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.WebPages;
using System.Web.Mvc;
using MvcComputer.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Security;
using Microsoft.AspNet;
using System.Net;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using System.Security.Claims;
using OAuth2;
using OAuth2.Client;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin;
using MvcComputer.App_Start;

namespace MvcComputer.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        { }
        public AccountController(ApplicationUserManager UserManager)
        {
            userManager = UserManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager userManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            }
            private set
            {
                userManager = _userManager;
            }

        }
        //
        // GET: /Account/
        private readonly AuthorizationRoot authorizationRoot=new AuthorizationRoot();

        private const string ProviderNameKey = "Providername";
        private string ProviderName
        {
            get { return (string)Session[ProviderNameKey]; }
            set { Session[ProviderNameKey] = value; }
        }
        

       
 
        ComputerDb db = new ComputerDb();


        ApplicationUser user = new ApplicationUser();

     

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnurl)
        {
            HttpCookie existingcookie=null;
            string action =Convert.ToString(Session["status"]);
            if(action.Equals("Logout"))
            {
                Session["status"] = "loggedout";
               return View();
            }
            var existinguser = userManager.Users;
            foreach (ApplicationUser xuser in existinguser)
            {
               existingcookie = Request.Cookies[xuser.UserName];
               if (existingcookie != null)
               {
                   
                   user.UserName = existingcookie.Name;
                   user.Id = existingcookie.Value;
                 
                   await SignInAsync(user, true);
                  
                   return RedirectToLocal(returnurl);
               }

            }
           
            return View();
        }

        [ChildActionOnly]
        public ActionResult FbLogin()
        {
            var model = authorizationRoot.Clients.Select(client => new ExternalLoginViewModel
             {
                 provider = client.Name
             });

            return View("_ExternalLoginListPartial", model);
        }
        
        [AllowAnonymous]
       [HttpPost]
        public async Task<ActionResult> Login(LoginModel model,string returnurl,string rememberme)
        {
            
            if(ModelState.IsValid)
            {
                var result =await userManager.FindAsync(model.username, model.password);          
           
                bool temp;
                if (rememberme == "true")
                {
                    temp = true;
                    string id = result.Id;
                    HttpCookie newcookie = new HttpCookie(result.UserName);
                    newcookie.Value = result.Id;
                    newcookie.Expires = DateTime.Now.AddMonths(12);
                    Response.Cookies.Add(newcookie);
                }
                else
                    temp = false;
                                

                if(result!=null)
                {
                        await SignInAsync(result,isPersistent:temp);
                        ViewBag.user = result.UserName;                       
                        return RedirectToLocal(returnurl);                       
                    
                }
                else
                    {
                        ModelState.AddModelError("","Username or password wrong");
                    }
              

            }
            return View(model);
        }

       
         [AllowAnonymous]
        public ActionResult Register()
        {
                        return View();
        }

        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel reg)
        {
            EmailService email = new EmailService();
            IdentityMessage message = new IdentityMessage();
           
            try
            {
                if (ModelState.IsValid)
                {
                    
                  
                    //user.Claims.Add(new IdentityUserClaim() { ClaimType = ClaimTypes.Gender, ClaimValue = "Male"});


                    user.UserName = reg.username;
                    user.Email = reg.email;
                    message.Destination = reg.email;
                    message.Subject = "Thanks for registeration";
                    message.Body = "HI" + user.UserName + "!Welcome to our world";
                    //user.Id = reg.username;
                    //var result = await userManager.GetClaimsAsync(user.Id);
                  var  result = await userManager.CreateAsync(user, reg.password);
                  if (result.Succeeded)
                  {
                      await SignInAsync(user, isPersistent: false);
                      string code=
                      ViewBag.user = user.UserName;
                      email.Send(message);
                      return Content("Hi " + user.UserName + "! You are succesfully registered.Please close the window to continue.", "text/html");
                  }
                  else
                  {
                      foreach(var error in result.Errors)
                      ModelState.AddModelError("",error );
                  }
                       
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            
            {
                string error = Convert.ToString(dbex.EntityValidationErrors);
                ModelState.AddModelError("", error);
                return View();
            }
            
            
            return View();
        }

       

      [Authorize]
        public ActionResult Index()
        {
            
            return View(user);
        
        }




        [AllowAnonymous]
       

        public ActionResult ExternalLogin(string providerName,string returnurl)
        {
             ProviderName =providerName;
            return new RedirectResult(GetClient().GetLoginLinkUri());
           //return new ChallengeResult(provider,Url.Action("ExternalLoginCallback","Account",new{ReturnUrl=returnurl}));

        }

        private IClient GetClient()
        {
            return authorizationRoot.Clients.First(c => c.Name == ProviderName);
        }
       
   
   
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
           var userinfo=  GetClient().GetUserInfo(Request.QueryString);
          

           if (userinfo == null)
               return View("ExternalLoginFailure");
 
           user.UserName = userinfo.FirstName+userinfo.LastName;
           user.Email = userinfo.Email;           
           var existinguser = await userManager.FindByEmailAsync(userinfo.Email);
           if (existinguser != null)
           {
               await SignInAsync(existinguser, true);
               return RedirectToLocal(returnUrl);
           }
           else
           {
               var result = await userManager.CreateAsync(user);
               if (result.Succeeded)
               {
                   await SignInAsync(user, true);
                   return RedirectToLocal(returnUrl);
               }
           }

           return View();

        }


        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
           
          
           
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var identity = await userManager.CreateIdentityAsync(
               user,DefaultAuthenticationTypes.ApplicationCookie);
          
 
            AuthenticationManager.SignIn(
               new AuthenticationProperties()
               {
                   IsPersistent = isPersistent
               }, identity);
        }

      
   
        public ActionResult Logoff()
        {
         
            Session["status"]="Logout";
            string username = User.Identity.GetUserName();

            HttpCookie existingUser = Request.Cookies[username];
            if (existingUser != null)
            {
                existingUser.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(existingUser);
            }
             AuthenticationManager.SignOut();
             return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Forgotpwd()
        {
            return View("_ForgotPwd");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Forgotpwd(ForgotPwdmodel model)
        {
            ViewBag.message = "Forgotpwd";
           
            
            if (ModelState.IsValid)
            {
                var user=await userManager.FindByNameAsync(model.username);
            

                if (user != null)
                {
                    string token=userManager.GeneratePasswordResetToken(user.Id);
                    IdentityResult result = await userManager.ResetPasswordAsync(user.Id,token,model.password);
                    if (result.Succeeded)
                    {                       
                        
                        return Content("Hi " + user.UserName + "!your password has been changed.Please close the window and login with the new password");
                    }
                    else
                    {
                        foreach (string error in result.Errors)
                        ModelState.AddModelError("",error);
                    }
                }
            }
            return View("_ForgotPwd");
        }

      private ActionResult RedirectToLocal(string redirecturl)
      {
          if (Url.IsLocalUrl(redirecturl))
          {
             return Redirect(redirecturl);
          }
          else
          {
             return RedirectToAction("Index", "Account");
          }
          
      }


      private IAuthenticationManager AuthenticationManager
      {
          get
          {
              return HttpContext.GetOwinContext().Authentication;
          }
      }

      public string UserNameCheck(string username)
      {
          string status="false";
          var users = userManager.Users;
          foreach (ApplicationUser usernames in users)
          {
              string name = usernames.UserName;
              if (name == username)
              {
                  status = "True";
                  break;
              }
              else
                  status = "false";
          }
          return status;
      }

    }
  


   
    }




