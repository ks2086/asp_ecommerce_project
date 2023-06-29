using Microsoft.AspNetCore.Mvc;
using Data.Data;
using Data.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Intranet.Controllers
{
    public abstract class BaseController <ServiceEntity, ModelEntity> : Controller
    {
        protected readonly ServiceEntity _service;

        public BaseController( ServiceEntity service )
        {
            _service = service;
        }

        public abstract Task<List<ModelEntity>> GetEntityList();
        public abstract Task<ModelEntity> GetWhereIdAsync(string Id);
        public abstract Task<bool> ServiceCreateAsync(ModelEntity model);
        public abstract Task<bool> ServiceUpdateAsync(string Id, ModelEntity model);
        public abstract Task<bool> ServiceDeleteAsync(string Id);

        protected async Task<IActionResult> BaseIndex()
        {
            ViewData["items"] = await GetEntityList();
            return View();
        }

        public async Task<IActionResult> BaseCreate()
        {
            return View();
        }

        public async Task<IActionResult> BaseEdit(string id = null)
        {
            var item = await GetWhereIdAsync(id);
            if (item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            ViewData["Id"] = id;
            return View(item);
        }

        public async Task<IActionResult> BaseDelete(string id = null)
        {
            var item = await GetWhereIdAsync(id);
            if (item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            ViewData["Id"] = id;
            return View(item);
        }
    }
}
