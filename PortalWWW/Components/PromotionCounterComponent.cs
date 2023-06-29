using Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace PortalWWW.Components
{
    public class PromotionCounterComponent : ViewComponent
    {
        private readonly PromotionService _promotionService;

        public PromotionCounterComponent(PromotionService promotionService)
        {
            _promotionService = promotionService;    
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Promotion"] = await _promotionService.GetCurrentPromotion();
            return View();
        }
    }
}
