namespace Panda_Player.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Panda_Player.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using Panda_Player.Models.Manage.Admin;
    using System.Collections.Generic;
    using Panda_Player.Models.PandaPlayer;
    using System.Data.Entity;

    internal sealed class DbMigrationsConfig : DbMigrationsConfiguration<Panda_Player.Models.ApplicationDbContext>
    {
        public DbMigrationsConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Panda_Player.Models.ApplicationDbContext";
        }

        protected override void Seed(Panda_Player.Models.ApplicationDbContext context)
        {
            if (!context.Roles.Any())
            {
                this.CreateRole(context, "Admin");
                this.CreateRole(context, "User");
            }

            if (!context.Users.Any())
            {
                var adminEmail = "admin@admin.com";
                var adminUserName = adminEmail;
                var adminFullName = "Administrator";
                var adminPassword = "admin";
                this.CreateUser(context, adminEmail, adminUserName, adminFullName, adminPassword);
                this.SetRoleToUser(context, "admin@admin.com", "Admin");

                // Add some users
                this.CreateUser(context, "a@a.com", "a@a.com", "UserA", "a");
                this.SetRoleToUser(context, "a@a.com", "User");
                this.CreateUser(context, "b@b.com", "b@b.com", "UserB", "b");
                this.SetRoleToUser(context, "b@b.com", "User");
                this.CreateUser(context, "c@c.com", "c@c.com", "UserC", "c");
                this.SetRoleToUser(context, "c@c.com", "User");
                this.CreateUser(context, "d@d.com", "d@d.com", "UserD", "d");
                this.SetRoleToUser(context, "d@d.com", "User");
                this.CreateUser(context, "e@e.com", "e@e.com", "UserE", "e");
                this.SetRoleToUser(context, "e@e.com", "User");
                this.CreateUser(context, "f@f.com", "f@f.com", "UserF", "f");
                this.SetRoleToUser(context, "f@f.com", "User");
                this.CreateUser(context, "g@g.com", "g@g.com", "UserG", "g");
                this.SetRoleToUser(context, "g@g.com", "User");

                var db = new ApplicationDbContext();
                var users = db.Users.ToList();
            }

            if (!context.Genres.Any())
            {
                this.CreateGenre(context, "Other");
                this.CreateGenre(context, "Rock");
                this.CreateGenre(context, "Chill Out");
                this.CreateGenre(context, "Folk");
                this.CreateGenre(context, "Jazz");
                this.CreateGenre(context, "Blues");
            }
        }

        private void SetRoleToUser(ApplicationDbContext context, string email, string role)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = context.Users.Where(u => u.Email == email).First();

            var result = userManager.AddToRole(user.Id, role);
            
            if (role == "User")
            {
                user.LockoutEnabled = true;
            }

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

        private void CreateGenre(ApplicationDbContext context, string name)
        {
            var genre = new Genre
            {
                Name = name,
                Songs = new HashSet<Song>()
            };

            context.Genres.Add(genre);
            context.SaveChanges();
        }

        private void CreateUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                FullName = adminFullName,
                Email = adminEmail,
                UserAccessControl = new Models.PandaPlayer.UserAccessControl { UserRegisterDate = DateTime.Now }
            };

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false
            };

            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join(";", userCreateResult.Errors));

            }
        }

        private void CreateRole(ApplicationDbContext context, string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var result = roleManager.Create(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }
    }
}
