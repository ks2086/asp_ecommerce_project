using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace Intranet.Controllers
{
    public class ProductsController : Controller
    {

        private readonly ProductService _productsService;
        private readonly ProductCategoryService _productCategoryService;
        private readonly ProductImagesService _productImagesService;
        private readonly ProductChaptersService _productChapterService;

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ProductService productService,
            ProductCategoryService productCategoryService,
            ProductChaptersService productChaptersService,
            ProductImagesService productImagesService,
            ILogger<ProductsController> logger
            )
        {
            _productsService = productService;
            _productCategoryService = productCategoryService;
            _productChapterService = productChaptersService;
            _productImagesService = productImagesService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productsService.GetListAsync();
            ViewData["items"] = response;
 
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _productCategoryService.GetListAsync();
            ViewData["Gallery"] = await _productImagesService.GetListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(int Tax, string Image, string Category, bool IsPromotion, bool IsBestseller, string ShortDescription, int DifficultyLevel, decimal PriceNetto, [Bind("Title,Text")] Product model)
        {

            SlugHelper sh = new SlugHelper();
            model.Slug = sh.GenerateSlug(model.Title);
            model.Created_at = DateTime.Now;
            model.Tax = Tax;
            model.ImageId = Image;
            model.CategoryId = Category;
            model.IsPromotion = IsPromotion;
            model.IsBestseller = IsBestseller;
            model.ShortDescription = ShortDescription;
            model.DifficultyLevel = DifficultyLevel;
            model.PriceNetto = PriceNetto;
            model.PriceBrutto = PriceNetto + (PriceNetto * Tax / 100);

            await _productsService.CreateAsync(model);

            return Redirect("/Products");
        }

        public async Task<IActionResult> Edit(string id = null)
        {
            if (id == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            else
            {

                var item = await _productsService.GetWhereIdAsync(id);
                if (item == null)
                {
                    return NotFound("Nie znaleziono żądanego zasobu.");
                }

                ViewData["Item"] = item;
                ViewData["Categories"] = await _productCategoryService.GetListAsync();
                ViewData["Gallery"] = await _productImagesService.GetListAsync();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string Id, string Title, string Text, decimal PriceNetto, int Tax, string Image, string Category, bool IsPromotion, bool IsBestseller, string ShortDescription, int DifficultyLevel)
        {
            var item = await _productsService.GetWhereIdAsync(Id);
            if (item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }

            Product model = new Product();

            SlugHelper sh = new SlugHelper();
            model.Title = Title;
            model.Slug = sh.GenerateSlug(Title);
            model.Text = Text;
            model.PriceNetto = PriceNetto;
            model.Tax = Tax;
            model.ImageId = Image;
            model.CategoryId = Category;
            model.IsPromotion = IsPromotion;
            model.IsBestseller = IsBestseller;
            model.ShortDescription = ShortDescription;
            model.DifficultyLevel = DifficultyLevel;
            model.PriceBrutto = PriceNetto + (PriceNetto * Tax / 100);

            await _productsService.UpdateAsync(Id, model);

            return Redirect("/Products");
        }

        public async Task<IActionResult> Delete(string id = null)
        {
            if (id == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            else
            {
                var item = await _productsService.GetWhereIdAsync(id);
                if (item == null)
                {
                    return NotFound("Nie znaleziono żądanego zasobu.");
                }
                ViewData["Item"] = item;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string Id)
        {
            var item = await _productsService.GetWhereIdAsync(Id);
            if (item == null)
            {
                return NotFound("Nie znaleziono żądanego zasobu.");
            }
            await _productsService.RemoveAsync(Id);
            return Redirect("/Products");
        }
    }
}
