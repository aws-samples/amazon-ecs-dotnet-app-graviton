using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcMovie.Models
{
    public class MovieGenreViewModel
    {
        public List<Movie> Movies { get; set; }
        public SelectList Genres { get; set; }
         public SelectList Years { get; set; }
        public string MovieGenre { get; set; }
        public string SearchString { get; set; }
        public int? WinYear { get; set; }
    }
}