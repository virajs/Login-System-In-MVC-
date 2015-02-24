using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcComputer.Models
{
    public class ComputerDb : DbContext
    {
        public ComputerDb()
            : base("ComputerDb")
        {
            Database.SetInitializer<ComputerDb>(new CreateDatabaseIfNotExists<ComputerDb>());
           

        }

        public static ComputerDb Create()
        {
            return new ComputerDb();
        }
       

        public DbSet<RegisterModel> Register{get;set;}
        public DbSet<LoginModel> Login { get; set; }
     
            

    }
}