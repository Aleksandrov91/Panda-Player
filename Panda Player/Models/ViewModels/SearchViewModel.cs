using Microsoft.Ajax.Utilities;
using Panda_Player.Models.PandaPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Panda_Player.Models.ViewModels
{
    public class SearchViewModel
    {
        public static Expression<Func<Song, SearchViewModel>> FromSong
        {
            get
            {
                return song => new SearchViewModel
                {
                    Id = song.Id,
                    Artist = song.Artist,
                    Title = song.Title,
                    Description = song.Description
                };
            }
        }
        public int Id { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}