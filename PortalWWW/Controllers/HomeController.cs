using Data.Services;
using Microsoft.AspNetCore.Mvc;
using PortalWWW.Models;
using System.Diagnostics;

namespace PortalWWW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly ContentService _contentService;

        public HomeController(ILogger<HomeController> logger, ProductService productService, ContentService contentService)
        {
            _logger = logger;
            _productService = productService;
            _contentService = contentService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Bestsellers"] = await _productService.GetBestsellersListAsync();
            ViewData["NewProducts"] = await _productService.GetLimitedListAsync(3);
            ViewData["WelcomeElement"] = await _contentService.GetWhereIdAsync("64949a70dc385d61379d56f5");
            ViewData["MobileElement"] = await _contentService.GetWhereIdAsync("64949a80dc385d61379d56f6");
            return View();
        }

        public async Task<IActionResult> Page(string slug)
        {
            var page = await _contentService.GetWhereSlugAsync(slug);
            if(page == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            ViewData["Page"] = page;
            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}