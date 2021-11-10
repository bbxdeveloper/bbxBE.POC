using System;
using System.Collections.Generic;

namespace bbxBE.POC.Domain.Models.ReportService
{
    public class ReportDesc
    {
        public string ID { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Version { get; set; }

        public byte[] Image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Uploaded { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

    }
}
