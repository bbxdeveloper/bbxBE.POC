using bbxBE.POC.Infrastructure.Persistence.Query;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bbxBE.POC.WebApi.Controllers.v1
{
    [Route("product/v1")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _p_env;

        private readonly ProductListQuery _productListQuery;

        private string ContentRootPath => _p_env.ContentRootPath;

        public ProductController(ProductListQuery productListQuery, IWebHostEnvironment p_env)
        {
            _p_env = p_env;

            _productListQuery = productListQuery;
        }

        [Route("list")]
        [HttpPost]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListProducts()
        {
            return Ok(_productListQuery.Execute(string.Empty));
        }

        [Route("search/{searchString}")]
        [HttpPost]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> SearchProducts([FromRoute] string searchString)
        {
            return Ok(_productListQuery.Execute(searchString));
        }
    }
}
