using bbxBE.POC.Domain.Common;
using Newtonsoft.Json;
using System;

namespace bbxBE.POC.Domain.Entities
{
    public class CRecord
    {
        public string RKTKOD { get; set; }
        public string SZAMLASZ { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime SZAMLAE { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime SZAMLAD { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime SZAMLAF { get; set; }
        public double OSSZ { get; set; }
        public double AFAERT { get; set; }
        public double BRUTTO { get; set; }
        public string VEVOID { get; set; }
        public string VEVONEV { get; set; }
    }
}
