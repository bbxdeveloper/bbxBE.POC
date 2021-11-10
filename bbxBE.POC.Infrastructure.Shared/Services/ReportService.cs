using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Domain.Exceptions.ReportService;
using bbxBE.POC.Domain.Models.ReportService;
using bbxBE.POC.Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.POC.Infrastructure.Shared.Services
{
    public class ReportService : IReportService
    {
        private readonly ReportSettings _settings;

        public readonly static string[] ExportTypes =
        {
            "PDF",
            "XLS", "XLSX", "CSV",
            "DOCX", "RTF",
            "XPS",
            "PPTX",
            "XAML", "WPFXAML"
        };

        public ReportService(IOptions<ReportSettings> settings)
        {
            _settings = settings.Value;
        }

        public virtual IActionResult GetReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters)
        {
            return GenerateReportFile(rootPath, outputFormat, ID, parameters);
        }

        private IActionResult GenerateReportFile(string rootPath, string outputFormat, string ID, ReportParams parameters)
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
            string dtSrcJSON = GetDataSourceFromXML(rootPath, "C-03176S21.xml");
            rs.Parameters.Add(new Telerik.Reporting.Parameter("JsonDataSourceValue", dtSrcJSON));
            rs.Parameters.Add(new Telerik.Reporting.Parameter("JsonSummaryDataSourceValue", dtSrcJSON));

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

        private string GetDataSourceFromXML(string contentRootPath, string xmlFileName)
        {
            string path = Path.Combine(contentRootPath, _settings.DataDir, xmlFileName);
            var doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
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
