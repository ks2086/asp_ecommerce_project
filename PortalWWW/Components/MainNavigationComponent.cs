using Data.Services;
using Microsoft.AspNetCore.Mvc;


namespace PortalWWW.Components
{
    public class MainNavigationComponent : ViewComponent
    {
        private readonly ContentService _service;

        public MainNavigationComponent(ContentService service) => _service = service;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["NavigationLinks"] = await _service.GetWhereContentTypeSlugAsync("strona-www");
            return View();
        }

    }
}
