using Data.Services;
using Microsoft.AspNetCore.Mvc;


namespace PortalWWW.Components
{
    public class SearchbarComponent : ViewComponent
    {
        private readonly ContentService _service;

        public SearchbarComponent(ContentService service) => _service = service;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Searchbar"] = await _service.GetWhereIdAsync("6493117dbd41dda00a46bfa0");
            return View();
        }

    }
}
