using Data.Services;
using Microsoft.AspNetCore.Mvc;


namespace PortalWWW.Components
{
    public class FooterComponent : ViewComponent
    {
        private readonly ContentService _service;

        public FooterComponent(ContentService service) => _service = service;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["FooterLinks"] = await _service.GetWhereContentTypeSlugAsync("strona-www");
            return View();
        }

    }
}
