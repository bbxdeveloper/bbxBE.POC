using bbxBE.POC.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.POC.Application.Interfaces.Repositories
{
    public interface IProductQueryRepository
    {
        public Task<IEnumerable<ProductDto>> Query(string searchString);
    }
}
