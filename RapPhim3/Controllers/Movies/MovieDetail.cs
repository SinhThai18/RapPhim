using Microsoft.AspNetCore.Mvc;

namespace RapPhim3.Controllers.Movies
{
    public class MovieDetail : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
