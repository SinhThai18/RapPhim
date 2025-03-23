using RapPhim3.Models;

namespace RapPhim3.ViewModel
{
    public class MovieTypeViewModel
    {
        public List<Genre> Genres { get; set; }
        public List<Country> Countries { get; set; }
        public List<Movie> AllMovies { get; set; }
        public List<Movie> MoviesShowingToday { get; set; }

    }
}
