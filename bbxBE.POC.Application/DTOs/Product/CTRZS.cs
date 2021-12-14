using System;
using System.Text.Json.Serialization;

namespace bbxBE.POC.Application.DTOs
{
    public class CTRZS
    {
        [JsonIgnore]
        public virtual DateTime ARMOD { get; set; }

        [JsonIgnore]
        public virtual int MIN { get; set; }

        [JsonIgnore]
        public virtual int AFA { get; set; }

        [JsonIgnore]
        public virtual bool VALASZT { get; set; }

        [JsonIgnore]
        public virtual bool ARVALT { get; set; }

        [JsonIgnore]
        public virtual bool ARMODLST { get; set; }

        [JsonIgnore]
        public virtual bool ARKARB { get; set; }

        [JsonIgnore]
        public virtual bool ACTIVE { get; set; }

        [JsonIgnore]
        public virtual bool FAFA { get; set; }

        [JsonIgnore]
        public virtual bool NOENG { get; set; }

        [JsonIgnore]
        public virtual bool WEB { get; set; }

        [JsonPropertyName("ProductCode")]
        public virtual string TERMKOD { get; set; }

        [JsonPropertyName("Name")]
        public virtual string TERMNEV { get; set; }

        [JsonPropertyName("Measure")]
        public virtual string ME { get; set; }

        [JsonIgnore]
        public virtual string TCSOP { get; set; }

        [JsonIgnore]
        public virtual string MEGJ { get; set; }

        [JsonIgnore]
        public virtual string TRIAL181 { get; set; }

        [JsonPropertyName("Price")]
        public virtual double AR1 { get; set; }

        [JsonIgnore]
        public virtual double AR2 { get; set; }

        [JsonPropertyName("Value")]
        public virtual double BESZAR { get; set; }

        [JsonIgnore]
        public virtual double TDIJ { get; set; }

        [JsonIgnore]
        public virtual double OVK_EURBAR { get; set; }

        [JsonIgnore]
        public virtual double OLDAR1 { get; set; }

        [JsonIgnore]
        public virtual double OLDAR2 { get; set; }

        [JsonPropertyName("Amount")]
        public virtual double RENDEGYS { get; set; }

        [JsonIgnore]
        public virtual double SULY { get; set; }
    }
}
