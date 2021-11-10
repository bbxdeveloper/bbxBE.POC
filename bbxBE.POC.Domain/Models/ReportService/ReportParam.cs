using System.Runtime.Serialization;

namespace bbxBE.POC.Domain.Models.ReportService
{
    [DataContract]
    public class ReportParam
    {
        public ReportParam(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
