using Data.Models;

namespace Intranet.Models
{
    public class ProductModelsAggregation
    {
        public Product Product { get; set; }
        public ProductImage Image { get; set; }
        public ProductCategory Category { get; set; }
    }
}
