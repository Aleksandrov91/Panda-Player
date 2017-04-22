using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using Panda_Player.Models.PandaPlayer;
using System.Collections.Generic;

namespace Panda_Player.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UploadedSongs = new HashSet<Song>();
            Playlists = new HashSet<Playlist>();
        }

        [Required]
        public string FullName { get; set; }

        public string ProfilePicPath { get; set; }

        public virtual ICollection<Song> UploadedSongs { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}