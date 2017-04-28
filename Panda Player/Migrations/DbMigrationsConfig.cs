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
                this.CreateUser(context, "h@h.com", "h@h.com", "UserH", "h");
                this.SetRoleToUser(context, "h@h.com", "User");
                this.CreateUser(context, "i@i.com", "i@i.com", "UserI", "i");
                this.SetRoleToUser(context, "i@i.com", "User");
                this.CreateUser(context, "j@j.com", "j@j.com", "UserJ", "j");
                this.SetRoleToUser(context, "j@j.com", "User");
                this.CreateUser(context, "k@k.com", "k@k.com", "UserK", "k");
                this.SetRoleToUser(context, "k@k.com", "User");
                this.CreateUser(context, "l@l.com", "l@l.com", "UserL", "l");
                this.SetRoleToUser(context, "l@l.com", "User");
                this.CreateUser(context, "m@m.com", "m@m.com", "UserM", "m");
                this.SetRoleToUser(context, "m@m.com", "User");
                this.CreateUser(context, "n@n.com", "n@n.com", "UserN", "n");
                this.SetRoleToUser(context, "n@n.com", "User");
                this.CreateUser(context, "o@o.com", "o@o.com", "UserO", "o");
                this.SetRoleToUser(context, "o@o.com", "User");
                this.CreateUser(context, "p@p.com", "p@p.com", "UserP", "p");
                this.SetRoleToUser(context, "p@p.com", "User");
                this.CreateUser(context, "q@q.com", "q@q.com", "UserQ", "m");
                this.SetRoleToUser(context, "q@q.com", "User");
                this.CreateUser(context, "r@r.com", "r@r.com", "UserR", "r");
                this.SetRoleToUser(context, "r@r.com", "User");
                this.CreateUser(context, "s@s.com", "s@s.com", "UserS", "s");
                this.SetRoleToUser(context, "s@s.com", "User");
                this.CreateUser(context, "t@t.com", "t@t.com", "UserT", "t");
                this.SetRoleToUser(context, "t@t.com", "User");
                this.CreateUser(context, "u@u.com", "u@u.com", "UserU", "u");
                this.SetRoleToUser(context, "u@u.com", "User");
                this.CreateUser(context, "v@v.com", "v@v.com", "UserV", "v");
                this.SetRoleToUser(context, "v@v.com", "User");
                this.CreateUser(context, "w@w.com", "w@w.com", "UserW", "w");
                this.SetRoleToUser(context, "w@w.com", "User");
                this.CreateUser(context, "x@x.com", "x@x.com", "UserX", "w");
                this.SetRoleToUser(context, "x@x.com", "User");
                this.CreateUser(context, "y@y.com", "y@y.com", "UserY", "y");
                this.SetRoleToUser(context, "y@y.com", "User");
                this.CreateUser(context, "z@z.com", "z@z.com", "UserZ", "z");
                this.SetRoleToUser(context, "z@z.com", "User");
                this.CreateUser(context, "sasho@sasho.com", "sasho@sasho.com", "Sasho", "sasho");
                this.SetRoleToUser(context, "sasho@sasho.com", "User");
                this.CreateUser(context, "krasi@krasi.com", "krasi@krasi.com", "Krasi", "krasi");
                this.SetRoleToUser(context, "krasi@krasi.com", "User");
                this.CreateUser(context, "emo@emo.com", "emo@emo.com", "Emo", "emo");
                this.SetRoleToUser(context, "emo@emo.com", "User");
                this.CreateUser(context, "vili@vili.com", "vili@vili.com", "Vili", "vili");
                this.SetRoleToUser(context, "vili@vili.com", "User");
                this.CreateUser(context, "hristo@hristo.com", "hristo@hristo.com", "Hristo", "hristo");
                this.SetRoleToUser(context, "hristo@hristo.com", "User");

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
