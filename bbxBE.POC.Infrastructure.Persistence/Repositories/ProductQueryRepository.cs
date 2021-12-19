using bbxBE.POC.Application.DTOs;
using bbxBE.POC.Application.Extensions;
using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Contexts;
using bbxBE.POC.Infrastructure.Persistence.Query;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
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

        private const string FIND_PRODUCT = "SELECT * FROM CTRZS WHERE TERMKOD = @kod";

        // Search - only TERMNEV and TERMKOD

        private const string QUERY_PRODUCTS_FOR_SEARCH = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS";

        private const string SEARCH_PRODUCTS = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMKOD LIKE @searchString OR TERMNEV LIKE @searchString";
        private const string COUNT_SEARCH_PRODUCTS = "SELECT COUNT(*) FROM CTRZS WHERE TERMKOD LIKE @searchString OR TERMNEV LIKE @searchString";
        private const string SEARCH_PRODUCTS_BY_CODE = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMKOD LIKE @searchString";
        private const string SEARCH_PRODUCTS_BY_NAME = "SELECT TOP (@topCount) TERMKOD, TERMNEV FROM CTRZS WHERE TERMNEV LIKE @searchString";

        #endregion SQL

        #region Messages

        private const string EMPTY_SEARCHSTRING = "A keresőszöveg üres!";
        private const string EMPTY_SEARCH_RESULT = "Nem található a keresésnek megfelelő termék!";
        private const string EMPTY_RESULT = "Nem található megjeleníthető termék!";

        private const string SEARCH_TOO_MUCH_RESULT = "A találatok száma ({0}) meghaladja a maximális megengedett mennyiséget ({1}). Kérem próbálja meg pontosítani a keresését!";
        private const string QUERY_TOO_MUCH_RESULT = "A találatok száma ({0}) meghaladja a maximális megengedett mennyiséget ({1}). Kérem próbálja meg szűrni a találatokat egy kereséssel!";

        private const string RESULT_COUNT = "Találatok száma: {0}, megjelenített: {1}";

        private const string ERROR = "Hiba: {0}";

        #endregion Messages

        private readonly ConcurrentBag<CTRZS> _searchCache;

        private readonly DapperContext _context;

        private readonly int _rowCountThreshold;

        private readonly int _cacheExpirationInMinutes;
        private DateTime _cacheLastHeartBeat;

        public ProductQueryRepository(DapperContext context, IConfiguration _configuration)
        {
            _context = context;

            _rowCountThreshold = _configuration.GetValue<int>("RowCountThreshold");

            _searchCache = new ConcurrentBag<CTRZS>();

            _cacheExpirationInMinutes = _configuration.GetValue<int>("CacheExpirationInMinutes");
            _cacheLastHeartBeat = DateTime.UtcNow;
        }

        private void ClearCacheIfExpired()
        {
            var momentOfCheck = DateTime.UtcNow;
            if ((momentOfCheck - _cacheLastHeartBeat).TotalMinutes >= _cacheExpirationInMinutes)
            {
                _cacheLastHeartBeat = momentOfCheck;
                _searchCache.Clear();
            }
        }

        public async Task<ProductListQueryResponse> Read(ProductListQueryRequest req)
        {
            var res = new ProductListQueryResponse
            {
                Result = new List<CTRZS>(),
                IsError = false
            };

            try
            {
                using var connection = _context.CreateConnection();

                ClearCacheIfExpired();

                var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@kod", req.QueryString },
                    });
                var products = await connection.QueryAsync<CTRZS>(FIND_PRODUCT, parameters);
                res.Result = products;

                return res;
            }
            catch (Exception ex)
            {
                res.IsError = true;
                res.Message = string.Format(ERROR, ex.Message); ;
                return res;
            }
        }

        public async Task<ProductListQueryResponse> QueryForSearch(ProductListQueryRequest req)
        {
            var res = new ProductListQueryResponse
            {
                Result = new List<CTRZS>(),
                IsError = false
            };

            try
            {
                using var connection = _context.CreateConnection();

                req.TopCount = Math.Clamp(req.TopCount, 1, _rowCountThreshold);

                ClearCacheIfExpired();

                var count = (await connection.QueryAsync<int>(COUNT_PRODUCTS,
                    new DynamicParameters(new Dictionary<string, object>() { { "@searchString", req.QueryString } }))).FirstOrDefault();
                if (count > _rowCountThreshold)
                {
                    res.Message = string.Format(QUERY_TOO_MUCH_RESULT, count, _rowCountThreshold);
                }
                else
                {
                    res.Message = string.Format(RESULT_COUNT, count, req.TopCount);
                }

                if (!_searchCache.IsEmpty)
                {
                    res.Result = _searchCache.ToList();

                    if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                    {
                        if (res.Result.Count() > _rowCountThreshold)
                        {
                            res.Message = string.Format(QUERY_TOO_MUCH_RESULT, res.Result.Count(), _rowCountThreshold);
                        }
                        else
                        {
                            res.Message = string.Format(RESULT_COUNT, count, req.TopCount);
                        }

                        res.Result = res.Result.Take(req.TopCount);
                        return res;
                    }
                    else
                    {
                        _searchCache.Clear();
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

                foreach (var p in products)
                {
                    _searchCache.Add(p);
                }

                return res;
            }
            catch (Exception ex)
            {
                res.IsError = true;
                res.Message = string.Format(ERROR, ex.Message); ;
                return res;
            }
        }

        public async Task<ProductListQueryResponse> SearchProduct(ProductListQueryRequest req)
        {
            var res = new ProductListQueryResponse
            {
                Result = new List<CTRZS>(),
                IsError = false
            };

            try
            {
                using var connection = _context.CreateConnection();

                req.TopCount = Math.Clamp(req.TopCount, 1, _rowCountThreshold);

                if (!string.IsNullOrWhiteSpace(req.QueryString))
                {

                    ClearCacheIfExpired();

                    var count = (await connection.QueryAsync<int>(COUNT_SEARCH_PRODUCTS,
                        new DynamicParameters(new Dictionary<string, object>() { { "@searchString", req.QueryString } }))).FirstOrDefault();
                    if (count > _rowCountThreshold)
                    {
                        res.Message = string.Format(SEARCH_TOO_MUCH_RESULT, count, _rowCountThreshold);
                    }
                    else
                    {
                        res.Message = string.Format(RESULT_COUNT, count, req.TopCount);
                    }

                    // At first, we try to do the quicksearch in the cache
                    var cacheSearch = _searchCache.Where(c => c.TERMKOD.Contains(req.QueryString) || c.TERMNEV.Contains(req.QueryString));
                    if (cacheSearch.Any())
                    {
                        res.Result = cacheSearch;

                        if (res.Result.Any() && res.Result.Count() >= req.TopCount)
                        {
                            if (res.Result.Count() > _rowCountThreshold)
                            {
                                res.Message = string.Format(SEARCH_TOO_MUCH_RESULT, res.Result.Count(), _rowCountThreshold);
                            }
                            else
                            {
                                res.Message = string.Format(RESULT_COUNT, count, req.TopCount);
                            }

                            res.Result = res.Result.Take(req.TopCount);
                            return res;
                        }
                        // In case of no match, we run the query on the database - and clear the cache from the previous try.
                        else
                        {
                            _searchCache.Clear();
                        }
                    }

                    var parameters = new DynamicParameters(new Dictionary<string, object>
                        {
                            { "@topCount", req.TopCount },
                            { "@searchString", req.QueryString.EncodeForLikeOperator() },
                        });
                    var products = await connection.QueryAsync<CTRZS>(SEARCH_PRODUCTS, parameters);

                    if (!products.Any())
                    {
                        res.Message = EMPTY_SEARCH_RESULT;
                        return res;
                    }

                    res.Result = products;

                    foreach (var p in products)
                    {
                        _searchCache.Add(p);
                    }

                    return res;
                }

                res.IsError = true;
                res.Message = EMPTY_SEARCHSTRING;

                return res;
            }
            catch (Exception ex)
            {
                res.IsError = true;
                res.Message = string.Format(ERROR, ex.Message); ;
                return res;
            }
        }
    }
}
