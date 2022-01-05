using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Domain.Models.ReportService;
using bbxBE.POC.Infrastructure.Shared.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace bbxBE.POC.WebApi.Controllers.v1
{
    [Route("report/v1")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportService _rl;
        private readonly IWebHostEnvironment _p_env;

        private string ContentRootPath => _p_env.ContentRootPath;

        public ReportController(IReportService rl, IWebHostEnvironment p_env)
        {
            _p_env = p_env;
            _rl = rl;
        }

        [Route("render/{ID}/{OutputFormat}")]
        [HttpPost]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public IActionResult RenderReportPost([FromRoute] string ID, [FromRoute] string OutputFormat, ReportParams parameters)
        {
            return _rl.GetReportFile(ContentRootPath, OutputFormat, ID, parameters);
        }

        [Route("render/grades/{ID}/{OutputFormat}")]
        [HttpPost]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public Task<IActionResult> RenderGradesReportPost([FromRoute] string ID, [FromRoute] string OutputFormat, ReportParams parameters)
        {
            return _rl.GetGradesReportFile(ContentRootPath, OutputFormat, ID, parameters);
        }

        [Route("outputs")]
        [HttpGet]
        public ActionResult<string[]> AvailableExportTypes()
        {
            return ReportService.ExportTypes;
        }
    }
}
