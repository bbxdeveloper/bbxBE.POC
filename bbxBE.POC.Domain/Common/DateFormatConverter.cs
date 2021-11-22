using Newtonsoft.Json.Converters;

namespace bbxBE.POC.Domain.Common
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
