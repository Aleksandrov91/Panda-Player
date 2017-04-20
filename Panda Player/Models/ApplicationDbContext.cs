using Microsoft.AspNet.Identity.EntityFramework;
using Panda_Player.Models.PandaPlayer;
using System.Data.Entity;

namespace Panda_Player.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<Image> Images { get; set; }
    }
}