using Microsoft.AspNetCore.Mvc;

using Data.Services;
using Data.Models;
using Microsoft.Extensions.Logging;
using Slugify;
using System.Net.Mime;

namespace Intranet.Controllers
{
    public class ContentController : Controller
    {
        private readonly ContentService _contentService;
        private readonly ContentTypeService _contentTypeService;

        private readonly ILogger<ContentController> _logger;

        public ContentController(
            ContentService contentService, 
            ContentTypeService contentTypeService,
            ILogger<ContentController> logger
            )
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string type = null)
        {
            if(type == null)
            {
                var defaultType = await _contentTypeService.GetFirstAsync(); 
                if(defaultType != null)
                {
                    type = defaultType.Slug;
                }
            }

            type = type ?? "aktualnosc";

            //_logger.LogInformation(type);
            var response = await _contentService.GetWhereContentTypeSlugAsync(type);
            //_logger.LogInformation(response.Count().ToString());
            ViewData["items"] = response;
            ViewData["type"] = type;
            return View();
        }

        public async Task<IActionResult> Create(string type = null) 
        {
            if(type == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            else
            {
                var contentType = await _contentTypeService.GetWhereSlugAsync(type);
                if(contentType == null)
                {
                    return NotFound("Nie znaleziono żądanego zasobu.");
                }

                ViewData["type"] = contentType.Slug;
                return View();
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store([Bind("Type,Title,Text")]ContentModel contentModel) {

            /*
            if (!ModelState.IsValid) 
            {
                var errorFields = ModelState.Keys.Where(key => ModelState[key].Errors.Any()).ToList();
                foreach (var field in errorFields)
                {
                    _logger.LogInformation($"Błąd w polu: {field}");
                }

                ViewData["type"] = contentModel.Type;
                return View("Create", contentModel);
            }
            */

            SlugHelper sh = new SlugHelper();
            contentModel.Slug = sh.GenerateSlug(contentModel.Title);
            contentModel.Created_at = DateTime.Now;

            await _contentService.CreateAsync(contentModel);

            return Redirect("/Content?type=" + contentModel.Type);
        }

        public async Task<IActionResult> Edit(string id = null)
        {
            if (id == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            else
            {

                var item = await _contentService.GetWhereIdAsync(id);
                if(item == null)
                {
                    return NotFound("Nie znaleziono żądanego zasobu.");
                }
                ViewData["Id"] = item.Id.ToString();
                ViewData["type"] = item.Type;
                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string Id, [Bind("Title,Text")] ContentModel contentModel)
        {
            SlugHelper sh = new SlugHelper();
            contentModel.Slug = sh.GenerateSlug(contentModel.Title);
            contentModel.Updated_at = DateTime.Now;

            //_logger.LogInformation(Id);
            await _contentService.UpdateAsync(Id, contentModel);
          
            return Redirect("/Content?type=" + contentModel.Type);
        }

        public async Task<IActionResult> Delete(string id = null)
        {
            if (id == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            else
            {

                var item = await _contentService.GetWhereIdAsync(id);
                if (item == null)
                {
                    return NotFound("Nie znaleziono żądanego zasobu.");
                }
                ViewData["Id"] = item.Id.ToString();
                ViewData["type"] = item.Type;
                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string Id)
        {
            var item = await _contentService.GetWhereIdAsync(Id);
            if(item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }

            await _contentService.RemoveAsync(Id);
            return Redirect("/Content?type=" + item.Type);
        }

    }

    
}
