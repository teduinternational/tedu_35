using Microsoft.AspNetCore.Mvc;

namespace KnowledgeSpace.BackendServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}