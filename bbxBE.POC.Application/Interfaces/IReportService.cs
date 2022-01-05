using bbxBE.POC.Domain.Models.ReportService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces
{
    public interface IReportService
    {
        public IActionResult GetReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters);
        public Task<IActionResult> GetGradesReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters, DateTime from, DateTime to);
    }
}
