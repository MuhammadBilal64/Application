using Application.DataAccess.Data;
using BookedIn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.DataAccess.DBinitializer
{
    public class DBInitializer : IDBInitailizer
    {
        private readonly AppsContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(
            AppsContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void initialize()
        {
            // 1. Apply pending migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Migration failed:");
                Console.WriteLine(ex.Message);
                throw;
            }

            // 2. Create roles and admin user only if Admin role doesn't exist
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Company")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Employee")).GetAwaiter().GetResult();

                // 3. Create admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "bm64812111@gmail.com",
                    Email = "bm64812111@gmail.com",
                    Name = "Bilal Malik",
                    EmailConfirmed = true
                }, "Hellinthesell1@").GetAwaiter().GetResult();

                var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "bm64812111@gmail.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                }
            }
        }
    }
}
