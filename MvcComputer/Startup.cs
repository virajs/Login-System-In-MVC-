using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.IO;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Provider;
using Microsoft.Owin.Security.Google;
using MvcComputer.Models;
using MvcComputer.App_Start;
//[assembly: OwinStartup(typeof(StartupDemo.Startup))]

namespace MvcComputer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {

                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
           
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ApplicationCookie);

            app.CreatePerOwinContext<MyDbContext>(MyDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

     //app.UseFacebookAuthentication(
     //    appId: "632505570172496",
     //           appSecret: "31a2f42989bc4549242b10f7ade02cda");

     //app.UseGoogleAuthentication(
     //    clientId: "629946084454-puufdfmu57pus6qs9ams0r7jiimfisf3.apps.googleusercontent.com", clientSecret: "3YcBYQwjIKLx_UAlceT9hya5");
          
           
        }
        

        
    }
}