using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Components
{
    public class ContentTypesComponent : ViewComponent
    {
        private readonly ContentTypeService _service;

        public ContentTypesComponent(ContentTypeService service) => _service = service;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["ListOfTypes"] = await _service.GetListAsync();
            return View();
        }

    }
}
