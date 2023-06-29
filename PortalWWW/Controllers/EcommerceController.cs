using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace PortalWWW.Controllers
{
    public class EcommerceController : Controller
    {
        private readonly ProductCategoryService _productCategoryService;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;



        public EcommerceController(ProductCategoryService productCategoryService, ProductService productService, OrderService orderService, UserService userService, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string category = "all", int dificultyLevel = 0, decimal price_from = 0, decimal price_to = 0)
        {
            ViewData["Categories"] = await _productCategoryService.GetListAsync();
            ViewData["SelectedCategory"] = category;

            var allCourses = await _productService.GetListAsync();

            ViewData["Products"] = allCourses;
            if (category != "all")
            {
                var selctedCategoryObj = await _productCategoryService.GetWhereSlugAsync(category);
                if (selctedCategoryObj != null)
                {
                    category = selctedCategoryObj.Id.ToString();
                }
            }

            ViewData["Products"] = await _productService.GetFilteredAsync(category != "all" ? category : null, dificultyLevel, price_from, price_to);

            ViewData["AllProductsCount"] = allCourses != null ? allCourses.Count() : 0;
            ViewData["DifficultyLevel"] = dificultyLevel > 0 ? dificultyLevel.ToString() : null; ;
            ViewData["PriceFrom"] = price_from > 0 ? price_from.ToString() : null;
            ViewData["PriceTo"] = price_to > 0 ? price_to.ToString() : null;

            return View();
        }

        public async Task<IActionResult> Promotion()
        {
            ViewData["Products"] = await _productService.GetPromotionsListAsync();
            return View();
        }

        public async Task<IActionResult> Product(string slug = null)
        {
            if(string.IsNullOrEmpty(slug))
            {
                return NotFound(404);
            }

            var product = await _productService.GetWhereSlugAsync(slug);
            ViewData["Product"] = product;
            ViewData["NewProducts"] = await _productService.GetLimitedListAsync(3);

            return View();
        }

        public async Task<IActionResult> Cart()
        {
            string cartSessionID = _session.GetString("OrderId");
            if (string.IsNullOrEmpty(cartSessionID))
            {
                Guid u = Guid.NewGuid();
                cartSessionID = u.ToString();
                _session.SetString("OrderId", cartSessionID);
            }

            ViewData["Cart"] = await _orderService.GetWhereSessionIdListAsync(cartSessionID);
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> AddToCart(string product)
        {
            string cartSessionID = _session.GetString("OrderId");
            if(string.IsNullOrEmpty(cartSessionID))
            {
                Guid u = Guid.NewGuid();
                cartSessionID = u.ToString();
                _session.SetString("OrderId", cartSessionID);
            }

            Product productItem = await _productService.GetWhereIdAsync(product);
            if(productItem == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return Json(new { message = "No Content" });
            }

            CartModel cartProductExists = await _orderService.GetWhereProductId(productItem.Id.ToString(), cartSessionID);
            if(cartProductExists != null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Ten kurs ("+productItem.Title+") istnieje już w koszyku." });
            }

            CartModel newCartItem = new CartModel();
            newCartItem.ProductId = productItem.Id.ToString();
            newCartItem.SessionOrderId = cartSessionID;
            newCartItem.ProductTitle = productItem.Title;
            newCartItem.UnitPrice = productItem.PriceBrutto;
            newCartItem.Created_at = DateTime.Now;

            await _orderService.AddToCartAsync(newCartItem);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(new { message = "Produkt " + productItem.Title + " został dodany do koszyka." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromCart (string cart = null)
        {
            string cartSessionID = _session.GetString("OrderId");
            if (cart != null && !string.IsNullOrEmpty(cartSessionID))
            {
                await _orderService.DeleteWhereProductIdAsync(cart, cartSessionID);
            }

            return Redirect("/Ecommerce/Cart");
        }

        [HttpPost]
        public async Task<IActionResult> MakeOrder()
        {
            UserModel user = string.IsNullOrEmpty(_session.GetString("UserSessionId")) ? null : await _userService.GetActiveUserAsync(_session.GetString("UserSessionId"));
            if (user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Akcja zabroniona, brak zalogowanego użytkownika" });
            }
            else
            {
                string cartSessionID = _session.GetString("OrderId");
                List<CartModel> cartList = await _orderService.GetWhereSessionIdListAsync(cartSessionID);
                if (string.IsNullOrEmpty(cartSessionID) || cartList.Count < 1)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { message = "Sesja koszyka nie istnieje lub brak elementów w koszyku" });
                }
                else
                {
                    decimal sum = 0;
                    foreach (var cart in cartList)
                    {
                        sum += cart.UnitPrice;
                    }

                    OrderModel order = new OrderModel();
                    order.SessionOrderId = cartSessionID;
                    order.AccountId = user.Id.ToString();
                    order.OrderSum = sum;
                    order.Created_at = DateTime.Now;

                    bool makeOrderRequest = await _orderService.MakeOrderAsync(order);
                    if (!makeOrderRequest)
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Json(new { message = "Wystąpił błąd w trakcie skłądania zamówienia, spróbuj ponownie później" });
                    }
                    else
                    {
                        _session.Remove("OrderId");
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                        return Json(new { message = "Zamówienie zostało złożone" });
                    }
                }
            }
        }




    }
}
