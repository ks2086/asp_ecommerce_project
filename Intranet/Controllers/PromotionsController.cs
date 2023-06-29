using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using Slugify;
using System.Globalization;

namespace Intranet.Controllers
{
    public class PromotionsController :  BaseController<PromotionService, Promotion>
    {

        private readonly PromotionService _promotionService;

        public PromotionsController(PromotionService promotionService) : base(promotionService)
        {
            _promotionService = promotionService;
        }

        public override async Task<List<Promotion>> GetEntityList()
        {
            return await _promotionService.GetListAsync();
        }

        public override async Task<Promotion> GetWhereIdAsync(string Id)
        {
            return await _promotionService.GetWhereIdAsync(Id);
        }

        public override async Task<bool> ServiceCreateAsync(Promotion model)
        {
            try
            {
                await _promotionService.CreateAsync(model);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public override async Task<bool> ServiceDeleteAsync(string Id)
        {
            try
            {
                await _promotionService.DeleteAsync(Id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override async Task<bool> ServiceUpdateAsync(string Id, Promotion model)
        {
            try
            {
                await _service.UpdateAsync(Id, model);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

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
        public async Task<IActionResult> Store(string Title, int Value, DateTime Starts_at, DateTime Ends_at)
        {

            Promotion model = new Promotion();
            model.Title = Title;
            model.Value = Value;
            model.Start_at = Starts_at;
            model.Ends_at = Ends_at;


            model.Created_at = DateTime.Now;

            await ServiceCreateAsync(model);

            return Redirect("/Promotions");
        }

        public async Task<IActionResult> Edit(string id = null)
        {
            return await BaseEdit(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string Id, string Title, int Value, DateTime Starts_at, DateTime Ends_at)
        {
            Promotion item = await _promotionService.GetWhereIdAsync(Id);
            item.Title = Title;
            item.Value = Value;
            item.Start_at = Starts_at;
            item.Ends_at = Ends_at;
            item.Updated_at = DateTime.Now;

            await ServiceUpdateAsync(Id, item);
            return Redirect("/Promotions");
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
            return Redirect("/Promotions");
        }


    }
}
