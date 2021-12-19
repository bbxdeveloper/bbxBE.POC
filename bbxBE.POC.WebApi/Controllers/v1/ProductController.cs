using bbxBE.POC.Infrastructure.Persistence.Query;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.POC.WebApi.Controllers.v1
{
    [Route("product/v1")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _p_env;

        private readonly ProductListQueryForSearch _productListQueryForSearch;
        private readonly ProductSearchQuery _productSearchQuery;

        private readonly int _defaultTopCount;

        private string ContentRootPath => _p_env.ContentRootPath;

        public ProductController(
            ProductListQueryForSearch productListQueryForSearch,
            ProductSearchQuery productSearchQuery,
            IWebHostEnvironment p_env,
            IConfiguration _configuration)
        {
            _p_env = p_env;

            _productListQueryForSearch = productListQueryForSearch;
            _productSearchQuery = productSearchQuery;

            _defaultTopCount = _configuration.GetValue<int>("DefaultTopCount");
        }

        [Route("list/some/{topCount}")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListSomeProducts([FromRoute] int topCount)
        {
            return Ok(await _productListQueryForSearch.Execute(new ProductListQueryRequest
            {
                TopCount = topCount,
            }));
        }

        [Route("list/some")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListSomeProducts()
        {
            return Ok(await _productListQueryForSearch.Execute(new ProductListQueryRequest
            {
                TopCount = _defaultTopCount,
            }));
        }

        [Route("list/all")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListAllProducts()
        {
            return Ok(await _productListQueryForSearch.Execute(new ProductListQueryRequest
            {
                TopCount = int.MaxValue,
            }));
        }

        [Route("search/{searchString}")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> SearchProduct([FromRoute] string searchString)
        {
            return Ok(await _productSearchQuery.Execute(new ProductListQueryRequest
            {
                TopCount = _defaultTopCount,
                SearchString = searchString
            }));
        }
    }
}
