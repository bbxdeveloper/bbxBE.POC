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

        private const string COUNT_PRODUCTS = "SELECT COUNT(*) FROM CTRZS WHERE ACTIVE = @active";
        private const string QUERY_PRODUCTS = "SELECT TOP (@topCount) * FROM CTRZS WHERE ACTIVE = @active";
        private const string SEARCH_PRODUCTS_BY_CODE = "SELECT TOP (@topCount) * FROM CTRZS WHERE ACTIVE = @active AND TERMKOD LIKE @searchString";
        private const string SEARCH_PRODUCTS_BY_NAME = "SELECT TOP (@topCount) * FROM CTRZS WHERE ACTIVE = @active AND TERMNEV LIKE @searchString";

        #endregion SQL

        #region Messages

        private const string EMPTY_SEARCHSTRING = "A keresőszöveg üres!";
        private const string EMPTY_SEARCH_RESULT = "Nem található a keresésnek megfelelő termék!";
        private const string EMPTY_RESULT = "Nem található megjeleníthető termék!";

        #endregion Messages

        private readonly Dictionary<string, IEnumerable<CTRZS>> _cache;

        private readonly DapperContext _context;

        private readonly int _rowCountThreshold;

        private readonly int _cacheExpirationInMinutes;
        private DateTime _cacheLastHeartBeat;

        public ProductQueryRepository(DapperContext context, IConfiguration _configuration)
        {
            _context = context;

            _rowCountThreshold = _configuration.GetValue<int>("RowCountThreshold");

            _cache = new Dictionary<string, IEnumerable<CTRZS>>();

            _cacheExpirationInMinutes = _configuration.GetValue<int>("CacheExpirationInMinutes");
            _cacheLastHeartBeat = DateTime.UtcNow;
        }

        private void ClearCacheIfExpired()
        {
            var momentOfCheck = DateTime.UtcNow;
            if ((momentOfCheck - _cacheLastHeartBeat).TotalMinutes >= _cacheExpirationInMinutes)
            {
                _cacheLastHeartBeat = momentOfCheck;
                _cache.Clear();
            }
        }

        public async Task<ProductListQueryResponse> Query(ProductListQueryRequest req)
        {
            using (var connection = _context.CreateConnection())
            {
                var res = new ProductListQueryResponse
                {
                    Result = new List<CTRZS>(),
                    IsError = false
                };

                Math.Clamp(req.TopCount, 1, _rowCountThreshold);
                int active = req.Active ? 0 : 1;

                ClearCacheIfExpired();

                if (_cache.ContainsKey(nameof(Query)))
                {
                    res.Result = _cache[nameof(Query)];

                    if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                    {
                        res.Result = res.Result.Take(req.TopCount);
                        return res;
                    }
                    else
                    {
                        _cache.Remove(nameof(Query));
                    }
                }

                var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@topCount", req.TopCount },
                        { "@active", active },
                    });
                var products = await connection.QueryAsync<CTRZS>(QUERY_PRODUCTS, parameters);

                if (!products.Any())
                {
                    res.Message = EMPTY_RESULT;
                    return res;
                }

                res.Result = products;

                _cache.Add(nameof(Query), products);

                return res;
            }
        }

        public async Task<ProductListQueryResponse> SearchProductByCode(ProductListQueryRequest req)
        {
            using (var connection = _context.CreateConnection())
            {
                var res = new ProductListQueryResponse
                {
                    Result = new List<CTRZS>(),
                    IsError = false
                };
                if (!string.IsNullOrWhiteSpace(req.SearchString))
                {
                    Math.Clamp(req.TopCount, 1, _rowCountThreshold);
                    int active = req.Active ? 0 : 1;

                    ClearCacheIfExpired();

                    if (_cache.ContainsKey(nameof(SearchProductByCode)))
                    {
                        res.Result = _cache[nameof(SearchProductByCode)];

                        if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                        {
                            // At first, we try to do the quicksearch in the cache
                            res.Result = res.Result.Where(x => x.TERMKOD.Contains(req.SearchString)).Take(req.TopCount);

                            // In case of no match, we run the query on the database - and clear the cache from the previous try.
                            if (!res.Result.Any())
                            {
                                _cache.Remove(nameof(SearchProductByCode));
                            }
                            else
                            {
                                return res;
                            }
                        }
                        else
                        {
                            _cache.Remove(nameof(SearchProductByCode));
                        }
                    }

                    var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@topCount", req.TopCount },
                        { "@active", active },
                        { "@searchString", req.SearchString.EncodeForLikeOperator() },
                    });
                    var products = await connection.QueryAsync<CTRZS>(SEARCH_PRODUCTS_BY_CODE, parameters);

                    if (!products.Any())
                    {
                        res.Message = EMPTY_SEARCH_RESULT;
                        return res;
                    }

                    res.Result = products;

                    _cache.Add(nameof(SearchProductByCode), products);

                    return res;
                }

                res.IsError = true;
                res.Message = EMPTY_SEARCHSTRING;

                return res;
            }
        }

        public async Task<ProductListQueryResponse> SearchProductByName(ProductListQueryRequest req)
        {
            using (var connection = _context.CreateConnection())
            {
                var res = new ProductListQueryResponse
                {
                    Result = new List<CTRZS>(),
                    IsError = false
                };
                if (!string.IsNullOrWhiteSpace(req.SearchString))
                {
                    Math.Clamp(req.TopCount, 1, _rowCountThreshold);
                    int active = req.Active ? 0 : 1;

                    ClearCacheIfExpired();

                    if (_cache.ContainsKey(nameof(SearchProductByName)))
                    {
                        res.Result = _cache[nameof(SearchProductByName)];

                        if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                        {
                            // At first, we try to do the quicksearch in the cache
                            res.Result = res.Result.Where(x => x.TERMNEV.Contains(req.SearchString)).Take(req.TopCount);

                            // In case of no match, we run the query on the database - and clear the cache from the previous try.
                            if (!res.Result.Any())
                            {
                                _cache.Remove(nameof(SearchProductByCode));
                            }
                            else
                            {
                                return res;
                            }
                        }
                        else
                        {
                            _cache.Remove(nameof(SearchProductByName));
                        }
                    }

                    var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@topCount", req.TopCount },
                        { "@active", active },
                        { "@searchString", req.SearchString.EncodeForLikeOperator() },
                    });
                    var products = await connection.QueryAsync<CTRZS>(SEARCH_PRODUCTS_BY_NAME, parameters);

                    if (!products.Any())
                    {
                        res.Message = EMPTY_SEARCH_RESULT;
                        return res;
                    }

                    res.Result = products;

                    _cache.Add(nameof(SearchProductByName), products);

                    return res;
                }

                res.IsError = true;
                res.Message = EMPTY_SEARCHSTRING;

                return res;
            }
        }
    }
}
