using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace Intranet.Controllers
{
    public class ProductImageController : BaseController<ProductImagesService, ProductImage>
    {

        private readonly ILogger<ProductImageController> _logger;
        public ProductImageController( ProductImagesService productImagesService, ILogger<ProductImageController> logger) : base(productImagesService)
        {
           _logger = logger;
        }

        //  Abstract methods from BaseController
        public override async Task<List<ProductImage>> GetEntityList()
        {
            return await _service.GetListAsync();
        }

        public override async Task<ProductImage> GetWhereIdAsync(string Id)
        {
            return await _service.GetWhereIdAsync(Id);
        }

        public override async Task<bool> ServiceCreateAsync(ProductImage model)
        {
            await _service.CreateAsync(model);
            return true;
        }

        public override async Task<bool> ServiceUpdateAsync(string Id, ProductImage model)
        {
            return await _service.UpdateAsync(Id, model);
        }

        public override async Task<bool> ServiceDeleteAsync(string Id)
        {
            return await _service.RemoveAsync(Id);
        }


        //  Actions

        public async Task<IActionResult> Index()
        {
            return await BaseIndex();
        }

        public async Task<IActionResult> Create()
        {
            return await BaseCreate();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store([Bind("Title,Url")] ProductImage model)
        {

            SlugHelper sh = new SlugHelper();
            model.Slug = sh.GenerateSlug(model.Title);
            model.Created_at = DateTime.Now;

            await ServiceCreateAsync(model);

            return Redirect("/ProductImage");
        }

        public async Task<IActionResult> Edit(string id = null)
        {
            return await BaseEdit(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string Id, [Bind("Title,Url")] ProductImage model)
        {
            SlugHelper sh = new SlugHelper();
            model.Slug = sh.GenerateSlug(model.Title);
            model.Updated_at = DateTime.Now;

            await ServiceUpdateAsync(Id, model);
            return Redirect("/ProductImage");
        }

        public async Task<IActionResult> Delete(string id = null)
        {
            return await BaseDelete(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string Id)
        {
            var item = await GetWhereIdAsync(Id);
            if (item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            await ServiceDeleteAsync(Id);
            return Redirect("/ProductImage");
        }

       
    }
}
