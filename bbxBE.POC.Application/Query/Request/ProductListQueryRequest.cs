namespace bbxBE.POC.Infrastructure.Persistence.Query
{
    public class ProductListQueryRequest
    {
        public int TopCount { get; set; }

        public bool Active { get; set; }

        public string QueryString { get; set; }
    }
}
