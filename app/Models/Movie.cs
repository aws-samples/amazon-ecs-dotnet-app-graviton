using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Required]
        public int Year { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Director  { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z ]*$")]
        [Required]
        [StringLength(30)]
        public string Genre { get; set; }
    }
}