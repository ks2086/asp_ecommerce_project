using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;


namespace PortalWWW.Components
{
    public class CartButtonComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly ILogger<CartButtonComponent> _logger;
        private readonly OrderService _orderService;

        public CartButtonComponent(IHttpContextAccessor httpContextAccessor, OrderService orderService, ILogger<CartButtonComponent> logger = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderId = _session.GetString("OrderId");
            int count = 0;
            if(orderId != null)
            {
                count = await _orderService.CountWhereSessionIdAsync(orderId);
            }
  
            ViewData["CartCount"] = count;
            return View();
        }

    }
}
