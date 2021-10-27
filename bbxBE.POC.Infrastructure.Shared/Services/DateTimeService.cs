using bbxBE.POC.Application.Interfaces;
using System;

namespace bbxBE.POC.Infrastructure.Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}