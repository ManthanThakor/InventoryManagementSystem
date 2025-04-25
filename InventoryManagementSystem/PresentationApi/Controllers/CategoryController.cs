using Microsoft.AspNetCore.Mvc;

namespace PresentationApi.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
