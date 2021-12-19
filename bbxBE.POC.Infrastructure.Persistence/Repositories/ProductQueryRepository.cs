using bbxBE.POC.Application.DTOs;
using bbxBE.POC.Application.Extensions;
using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Contexts;
using bbxBE.POC.Infrastructure.Persistence.Query;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.POC.Infrastructure.Persistence.Repositories
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        #region SQL

        private const string COUNT_PRODUCTS = "SELECT COUNT(*) FROM CTRZS";
        private const string QUERY_PRODUCTS = "SELECT TOP (@topCount) * FROM CTRZS";

        // Search - only TERMNEV and TERMKOD

        private const string QUERY_PRODUCTS_FOR_SEARCH = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS";

        private const string SEARCH_PRODUCTS = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMKOD LIKE @searchString OR TERMNEV LIKE @searchString";
        private const string SEARCH_PRODUCTS_BY_CODE = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMKOD LIKE @searchString";
        private const string SEARCH_PRODUCTS_BY_NAME = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMNEV LIKE @searchString";

        #endregion SQL

        #region Messages

        private const string EMPTY_SEARCHSTRING = "A keresőszöveg üres!";
        private const string EMPTY_SEARCH_RESULT = "Nem található a keresésnek megfelelő termék!";
        private const string EMPTY_RESULT = "Nem található megjeleníthető termék!";

        #endregion Messages

        private readonly Dictionary<string, IEnumerable<CTRZS>> _SearchCache;

        private readonly DapperContext _context;

        private readonly int _rowCountThreshold;

        private readonly int _cacheExpirationInMinutes;
        private DateTime _cacheLastHeartBeat;

        public ProductQueryRepository(DapperContext context, IConfiguration _configuration)
        {
            _context = context;

            _rowCountThreshold = _configuration.GetValue<int>("RowCountThreshold");

            _SearchCache = new Dictionary<string, IEnumerable<CTRZS>>();

            _cacheExpirationInMinutes = _configuration.GetValue<int>("CacheExpirationInMinutes");
            _cacheLastHeartBeat = DateTime.UtcNow;
        }

        private void ClearCacheIfExpired()
        {
            var momentOfCheck = DateTime.UtcNow;
            if ((momentOfCheck - _cacheLastHeartBeat).TotalMinutes >= _cacheExpirationInMinutes)
            {
                _cacheLastHeartBeat = momentOfCheck;
                _SearchCache.Clear();
            }
        }

        public async Task<ProductListQueryResponse> QueryForSearch(ProductListQueryRequest req)
        {
            using (var connection = _context.CreateConnection())
            {
                var res = new ProductListQueryResponse
                {
                    Result = new List<CTRZS>(),
                    IsError = false
                };

                Math.Clamp(req.TopCount, 1, _rowCountThreshold);

                ClearCacheIfExpired();

                _SearchCache.Remove(nameof(SearchProduct));

                if (_SearchCache.ContainsKey(nameof(QueryForSearch)))
                {
                    res.Result = _SearchCache[nameof(QueryForSearch)];

                    if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                    {
                        res.Result = res.Result.Take(req.TopCount);
                        return res;
                    }
                    else
                    {
                        _SearchCache.Remove(nameof(QueryForSearch));
                    }
                }

                var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@topCount", req.TopCount },
                    });
                var products = await connection.QueryAsync<CTRZS>(QUERY_PRODUCTS_FOR_SEARCH, parameters);

                if (!products.Any())
                {
                    res.Message = EMPTY_RESULT;
                    return res;
                }

                res.Result = products;

                _SearchCache.Add(nameof(QueryForSearch), products);

                return res;
            }
        }

        public async Task<ProductListQueryResponse> SearchProduct(ProductListQueryRequest req)
        {
            using (var connection = _context.CreateConnection())
            {
                var res = new ProductListQueryResponse
                {
                    Result = new List<CTRZS>(),
                    IsError = false
                };

                Math.Clamp(req.TopCount, 1, _rowCountThreshold);

                if (!string.IsNullOrWhiteSpace(req.SearchString))
                {

                    ClearCacheIfExpired();

                    if (_SearchCache.ContainsKey(nameof(SearchProduct)))
                    {
                        res.Result = _SearchCache[nameof(SearchProduct)];

                        if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                        {
                            // At first, we try to do the quicksearch in the cache
                            res.Result = res.Result.Where(x => x.TERMKOD.Contains(req.SearchString)).Take(req.TopCount);

                            // In case of no match, we run the query on the database - and clear the cache from the previous try.
                            if (!res.Result.Any())
                            {
                                _SearchCache.Remove(nameof(SearchProduct));
                            }
                            else
                            {
                                return res;
                            }
                        }
                        else
                        {
                            _SearchCache.Remove(nameof(SearchProduct));
                        }
                    }

                    var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@topCount", req.TopCount },
                        { "@searchString", req.SearchString.EncodeForLikeOperator() },
                    });
                    var products = await connection.QueryAsync<CTRZS>(SEARCH_PRODUCTS, parameters);

                    if (!products.Any())
                    {
                        res.Message = EMPTY_SEARCH_RESULT;
                        return res;
                    }

                    res.Result = products;

                    _SearchCache.Add(nameof(SearchProduct), products);

                    return res;
                }

                res.IsError = true;
                res.Message = EMPTY_SEARCHSTRING;

                return res;
            }
        }
    }
}
