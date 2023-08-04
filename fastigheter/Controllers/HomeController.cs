using fastigheter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using fastigheter.Services;

namespace fastigheter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Intresse intresse)
        {
            AzureServices.CreateJson(intresse);
            AzureServices.SendEmailIntresse(intresse);
            return Redirect("~/Home/Index");
        }


        public IActionResult Lägenheter()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Skärstad()
        {
            return View();
        }
        public IActionResult Ölmstad()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Boende()
        {
            return View();
        }
        public ActionResult ErrorReport(ErrorReport errorReport)
        {
            AzureServices.CreateJsonErrors(errorReport);
            AzureServices.SendEmailIssueReport(errorReport);
            return Redirect("~/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}