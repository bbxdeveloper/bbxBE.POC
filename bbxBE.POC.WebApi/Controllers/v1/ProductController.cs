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

        private readonly ProductListQuery _productListQuery;
        private readonly ProductSearchByCodeQuery _productSearchByCodeQuery;
        private readonly ProductSearchByNameQuery _productSearchByNameQuery;

        private readonly int _defaultTopCount;

        private string ContentRootPath => _p_env.ContentRootPath;

        public ProductController(
            ProductListQuery productListQuery,
            ProductSearchByCodeQuery productSearchByCodeQuery,
            ProductSearchByNameQuery productSearchByNameQuery,
            IWebHostEnvironment p_env,
            IConfiguration _configuration)
        {
            _p_env = p_env;

            _productListQuery = productListQuery;
            _productSearchByCodeQuery = productSearchByCodeQuery;
            _productSearchByNameQuery = productSearchByNameQuery;

            _defaultTopCount = _configuration.GetValue<int>("DefaultTopCount");
        }

        [Route("list/some/{topCount}")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListSomeProducts([FromRoute] int topCount)
        {
            return Ok(await _productListQuery.Execute(new ProductListQueryRequest
            {
                TopCount = topCount,
                Active = true
            }));
        }

        [Route("list/some")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListSomeProducts()
        {
            return Ok(await _productListQuery.Execute(new ProductListQueryRequest
            {
                TopCount = _defaultTopCount,
                Active = true
            }));
        }

        [Route("list/all")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ListAllProducts()
        {
            return Ok(await _productListQuery.Execute(new ProductListQueryRequest
            {
                TopCount = int.MaxValue,
                Active = true
            }));
        }

        [Route("search/code/{searchString}")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> SearchProductByCode([FromRoute] string searchString)
        {
            return Ok(await _productSearchByCodeQuery.Execute(new ProductListQueryRequest
            {
                TopCount = _defaultTopCount,
                Active = true,
                SearchString = searchString
            }));
        }

        [Route("search/name/{searchString}")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> SearchProductByName([FromRoute] string searchString)
        {
            return Ok(await _productSearchByNameQuery.Execute(new ProductListQueryRequest
            {
                TopCount = _defaultTopCount,
                Active = true,
                SearchString = searchString
            }));
        }
    }
}
