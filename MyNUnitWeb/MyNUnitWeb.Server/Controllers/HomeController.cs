using Microsoft.AspNetCore.Mvc;

namespace MyNUnitWeb.Server.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "this is index...";
        }
    }
}
