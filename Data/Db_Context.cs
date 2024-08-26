using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FGCCore.Data
{
    public class Db_Context : IdentityDbContext<ApplicationUser>
    {
        public Db_Context(DbContextOptions<Db_Context> options)
            : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
               new IdentityRole() { Id = "SA", Name = "SA", NormalizedName = "SA", ConcurrencyStamp = null }, //Super Admin
               new IdentityRole() { Id = "User", Name = "User", NormalizedName = "User", ConcurrencyStamp = null } //Admin User

               );




            var hasher = new PasswordHasher<ApplicationUser>();


            //Seeding the User to AspNetUsers table
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "c58e1a9d-1c28-46db-830a-7b3f0b9663f1", // primary key
                    UserName = "tahir@batech.com.pk",
                    NormalizedUserName = "tahir@batech.com.pk".ToUpper(),
                    Email = "tahir@batech.com.pk",
                    NormalizedEmail = "tahir@batech.com.pk".ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Sa123!@#"),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    FullName = "Tahir",

                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "SA",
                UserId = "c58e1a9d-1c28-46db-830a-7b3f0b9663f1"
            }
        );
            base.OnModelCreating(modelBuilder);
        }
    }
}
