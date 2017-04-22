
using System.ComponentModel.DataAnnotations;

namespace Panda_Player.Models.ViewModels
{
    public class PlaylistViewModel
    {
        public int Id { get; set; }

        [Required]
        public string PlaylistName { get; set; }

        public bool IsPublic { get; set; }
    }
}