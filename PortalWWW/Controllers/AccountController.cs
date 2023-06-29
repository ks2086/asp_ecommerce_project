using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using BCrypt.Net;

namespace PortalWWW.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserService _userService;
        private readonly OrderService _orderService;
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AccountController(UserService userService, OrderService orderService, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _orderService = orderService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckLoginAccount()
        {
            if (string.IsNullOrEmpty(_session.GetString("UserSessionId")))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Brak zalogowanego użytkownika" });
            }
            else
            {
                UserModel user = await _userService.GetActiveUserAsync(_session.GetString("UserSessionId"));
                if (user == null) {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { message = "Użytkownik nie istnieje" });
                }
                else
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                    return Json(new { email = user.Email });
                }
            }
        }

        public async Task<IActionResult> LogIn()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        public async Task<IActionResult> Panel()
        {


            UserModel user = !string.IsNullOrEmpty(_session.GetString("UserSessionId")) ? await _userService.GetActiveUserAsync(_session.GetString("UserSessionId")) : null;
            if (user == null)
            {
                return Redirect("/Account/LogIn");
            }

            ViewData["Account"] = user;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            _session.Remove("UserSessionId");
            return Redirect("/Account/LogIn");
        }

        [HttpPost]
        public async Task<IActionResult> Store(string username, string password)
        {
 
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Formularz zawiera błędy" });
            }
            else
            {
                UserModel user = await _userService.GetUserWhereEmailAsync(username);
                if (user == null)
                {
                    UserModel newUser = new UserModel();
                    newUser.Email = username;
                    newUser.Password = HashPassword(password);
                    newUser.Created_at = DateTime.Now;

                    bool userCreated = await _userService.StoreUserAsync(newUser);
                    if (userCreated)
                    {

                        UserModel currentUser = await _userService.GetUserWhereEmailAsync(username);
                        _session.SetString("UserSessionId", currentUser.Id.ToString());

                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                        return Json(new { message = "Użytkownik został zapisany" });
                    }

                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { message = "Użytkownik nie został zapisany" });
                }

                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Podany adres e-mail jest zajęty" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Signin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Formularz zawiera błędy" });
            }
            else
            {
                UserModel user = await _userService.GetUserWhereEmailAsync(username);
                if (user == null || !VerifyPassword(password, user.Password))
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Json(new { message = "Podane dane są błędne" });
                }
                else
                {
                    _session.SetString("UserSessionId", user.Id.ToString());
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                    return Json(new { message = "Autoryzacja poprawna" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckUsername(string username) 
        {
            if (string.IsNullOrEmpty(username))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Formularz zawiera błędy" });
            }
            else
            {
                UserModel user = await _userService.GetUserWhereEmailAsync(username);
                if (user == null)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                    return Json(new { message = "Podany adres e-mail jest poprawny" });
                }

                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Podany adres e-mail jest zajęty" });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountOrders()
        {
            UserModel user = !string.IsNullOrEmpty(_session.GetString("UserSessionId")) ? await _userService.GetActiveUserAsync(_session.GetString("UserSessionId")) : null;
            if(user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Brak zalogowanego użytkownika" });
            }

            List<object> orderDataList = new List<object>();
            List<OrderModel> orders = await _orderService.GetUserOrdersAsync(user.Id.ToString());
            if (orders.Count > 0)
            {
                foreach (OrderModel order in orders)
                {
                    var orderData = new
                    {
                        OrderId = order.SessionOrderId,
                        OrderSum = Convert.ToDecimal(order.OrderSum),
                        OrderCdate = order.Created_at
                    };
                    orderDataList.Add(orderData);
                }
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(orderDataList);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(string id = null)
        {
            UserModel user = !string.IsNullOrEmpty(_session.GetString("UserSessionId")) ? await _userService.GetActiveUserAsync(_session.GetString("UserSessionId")) : null;
            if (user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "Brak zalogowanego użytkownika" });
            }

            List<object> orderProductsList = new List<object>();
            List<CartModel> cartItems = await _orderService.GetWhereSessionIdListAsync(id);
            if (cartItems != null && cartItems.Count > 0)
            {
                foreach (CartModel item in cartItems)
                {
                    var itemData = new
                    {
                        ProductTitle = item.ProductTitle,
                        ProductPrice = Convert.ToDecimal(item.UnitPrice),
                    };
                    orderProductsList.Add(itemData);
                }
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(orderProductsList);
        }


        //  STATIC

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
