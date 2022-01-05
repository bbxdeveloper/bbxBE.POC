using bbxBE.POC.Application.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    public class ReportDataQueryResponse
    {
        [JsonPropertyName("Result")]
        public IEnumerable<ProductSumReportRow> Result { get; set; }

        [JsonPropertyName("IsError")]
        public bool IsError { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }
    }
}
