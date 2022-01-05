using System;

namespace bbxBE.POC.Application.DTOs
{
    public class ProductSumReportRow
    {
        public virtual string RKTKOD { get; set; }
        public virtual string SZAMLASZ { get; set; }
        public virtual DateTime SZAMLAE { get; set; }
        public virtual DateTime SZAMLAD { get; set; }
        public virtual DateTime SZAMLAF { get; set; }
        public virtual double OSSZ { get; set; }
        public virtual double AFAERT { get; set; }
        public virtual double BRUTTO { get; set; }
        public virtual string VEVOID { get; set; }
        public virtual string VEVONEV { get; set; }
    }
}
