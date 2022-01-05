using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Domain.Exceptions.ReportService;
using bbxBE.POC.Domain.Models.ReportService;
using bbxBE.POC.Domain.Settings;
using bbxBE.POC.Infrastructure.Persistence.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.POC.Infrastructure.Shared.Services
{
    public class ReportService : IReportService
    {
        private readonly ReportSettings _settings;
        private readonly SumReportQuery _sumReportQuery;

        public readonly static string[] ExportTypes =
        {
            "PDF",
            "XLS", "XLSX", "CSV",
            "DOCX", "RTF",
            "XPS",
            "PPTX",
            "XAML", "WPFXAML"
        };

        public ReportService(IOptions<ReportSettings> settings, SumReportQuery sumReportQuery)
        {
            _settings = settings.Value;
            _sumReportQuery = sumReportQuery;
        }

        public virtual IActionResult GetReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters)
        {
            return GenerateReportFileAsync(rootPath, outputFormat, ID, parameters, "C-03176S21.xml");
        }

        public virtual Task<IActionResult> GetGradesReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters, DateTime from, DateTime to)
        {
            return GenerateSumReportFileAsync(rootPath, outputFormat, ID, parameters, "bbx_report_poc2.json", from, to);
        }

        private IActionResult GenerateReportFileAsync(string rootPath, string outputFormat, string ID, ReportParams parameters, string dtSrcFileName)
        {
            InstanceReportSource rs = GetInstanceReportSource(rootPath, ID);
            if (rs == null)
                throw new ReportException("ReportSource is null!");

            var dicPars = parameters.getReportParameters();
            foreach (var par in dicPars)
            {
                rs.Parameters.Add(new Telerik.Reporting.Parameter(par.Key, par.Value));
            }

            // JSON DataSources as JSON string parameters
            string dtSrcJSON = GetFileContentAsJsonString(rootPath, dtSrcFileName);
            rs.Parameters.Add(new Telerik.Reporting.Parameter("JsonDataSourceValue", dtSrcJSON));

            var reportProcessor = new ReportProcessor();
            var deviceInfo = new System.Collections.Hashtable();
            var reportSource = rs;

            var result = reportProcessor.RenderReport(outputFormat, reportSource, deviceInfo);
            if (result == null)
                throw new Exception("report result is null!");

            var stream = new MemoryStream(result.DocumentBytes);
            string fileName = ID + "." + outputFormat.ToLower();

            return new FileStreamResult(stream, $"application/{outputFormat.ToLower()}") { FileDownloadName = fileName };
        }

        private async Task<IActionResult> GenerateSumReportFileAsync(string rootPath, string outputFormat, string ID, ReportParams parameters, string dtSrcFileName, DateTime from, DateTime to)
        {
            InstanceReportSource rs = GetInstanceReportSource(rootPath, ID);
            if (rs == null)
                throw new ReportException("ReportSource is null!");

            var dicPars = parameters.getReportParameters();
            foreach (var par in dicPars)
            {
                rs.Parameters.Add(new Telerik.Reporting.Parameter(par.Key, par.Value));
            }

            // JSON DataSources as JSON string parameters
            var data = await _sumReportQuery.Execute(new ReportDataQueryRequest { From = from, To = to });

            if (data.IsError)
            {
                throw new Exception(data.Message);
            }

            string dtSrcJSON = JsonConvert.SerializeObject(data);
            rs.Parameters.Add(new Telerik.Reporting.Parameter("JsonDataSourceValue", dtSrcJSON));

            var reportProcessor = new ReportProcessor();
            var deviceInfo = new System.Collections.Hashtable();
            var reportSource = rs;

            var result = reportProcessor.RenderReport(outputFormat, reportSource, deviceInfo);
            if (result == null)
                throw new Exception("report result is null!");

            var stream = new MemoryStream(result.DocumentBytes);
            string fileName = ID + "." + outputFormat.ToLower();

            return new FileStreamResult(stream, $"application/{outputFormat.ToLower()}") { FileDownloadName = fileName };
        }

        private string GetFileContentAsJsonString(string contentRootPath, string fileName)
        {
            if (fileName.EndsWith(".xml"))
            {
                return GetXMLFileContentAsJsonString(contentRootPath, fileName);
            }
            else if (fileName.EndsWith(".json"))
            {
                return GetJsonFileContentAsJsonString(contentRootPath, fileName);
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetXMLFileContentAsJsonString(string contentRootPath, string fileName)
        {
            string path = Path.Combine(contentRootPath, _settings.DataDir, fileName);
            var doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }

        private string GetJsonFileContentAsJsonString(string contentRootPath, string fileName)
        {
            string res;
            string path = Path.Combine(contentRootPath, _settings.DataDir, fileName);
            using (var file = File.OpenText(path))
            {
                res = file.ReadToEnd();
            }
            return res;
        }

        private InstanceReportSource GetInstanceReportSource(string contentRootPath, string p_reportname)
        {
            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep;

            var reportsPath = Path.Combine(contentRootPath, _settings.ReportsDir);
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true
            };

            var reportFile = Path.Combine(reportsPath, p_reportname + ".trdx");
            using (var xmlReader = XmlReader.Create(reportFile, settings))
            {
                var xmlSerializer = new ReportXmlSerializer();

                rep = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                reportSource = new InstanceReportSource
                {
                    ReportDocument = rep
                };
            }
            return reportSource;
        }
    }
}
