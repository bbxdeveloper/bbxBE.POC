using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace bbxBE.POC.Domain.Models.ReportService
{
    [DataContract]
    public class ReportParams
    {
        [DataMember]
        public List<ReportParam> Params { get; set; }

        public ReportParams(string parameters)
        {
            Params = new List<ReportParam>(getReportParameters(parameters).Select(x => new ReportParam(x.Key, x.Value.ToString())).ToList());
        }

        public ReportParams(List<ReportParam> @params)
        {
            Params = @params;
        }

        public ReportParams() {}

        public Dictionary<string, object> getReportParameters()
        {
            return Params.ToDictionary(x => x.Key, x => (object)x.Value);
        }

        private Dictionary<string, object> getReportParameters(string parameters)
        {
            var dicPars = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var parsedParameters = HttpUtility.ParseQueryString(parameters);

                foreach (var par in parsedParameters)
                {
                    dicPars.Add((string)par, parsedParameters[(string)par]);
                }
            }
            return dicPars;
        }
    }
}
