using bbxBE.POC.Domain.Models.ReportService;
using Microsoft.AspNetCore.Mvc;

namespace bbxBE.POC.Application.Interfaces
{
    public interface IReportService
    {
        public IActionResult GetReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters);
        public IActionResult GetGradesReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters);
    }
}
