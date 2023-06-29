using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PortalWWW.Controllers
{
    public class SearchController : Controller
    {
        private readonly ProductService _productService;

        public SearchController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            var searchResult = await _productService.GetSearchListAsync(query);
            ViewData["SearchResults"] = searchResult;
            ViewData["SearchQuery"] = query;
            return View();
        }

        public async Task<IActionResult> GetSearchTitle(string query = null)
        {
            List<object> productsDataList = new List<object>();
            List<Product> products = await _productService.GetSearchListAsync(query);
            if (products.Count > 0)
            {
                foreach (Product product in products)
                {
                    var orderData = new
                    {
                        value = product.Title
                    };
                    productsDataList.Add(orderData);
                }
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(productsDataList);
        }
    }
}
