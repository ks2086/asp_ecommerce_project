using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestfulApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {

        private readonly ContentService _contentService;

        public ContentController(  ContentService contentService )
        {
            _contentService = contentService;
        }

        [HttpGet(Name = "GetAllContent")]
        public async Task<IActionResult> Get() {
            List<ContentModel> contentModels = await _contentService.GetWhereContentTypeSlugAsync("aktualnosc");
            return Ok(contentModels);
        }
    }
}
