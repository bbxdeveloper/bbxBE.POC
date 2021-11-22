using System;

namespace bbxBE.POC.Domain.Exceptions.ReportService
{
    public class ReportException : Exception
    {
        public ReportException(string message) : base(message)
        {
        }
    }
}
