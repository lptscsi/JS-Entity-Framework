using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    public class QueryResult
    {
        private readonly Lazy<SubResultList> _subResults;

        public QueryResult()
        {
            _subResults = new Lazy<SubResultList>(() => [], true);
        }

        public int? totalCount { get; set; }

        public IEnumerable<object> result { get; set; }

        public object extraInfo { get; set; }

        public SubResultList subResults => _subResults.Value;
    }

    public class QueryResult<T> : QueryResult
        where T : class
    {
        public QueryResult()
            : this(Enumerable.Empty<T>(), null)
        {
        }

        public QueryResult(IEnumerable<T> result)
            : this(result, null)
        {
        }

        public QueryResult(IEnumerable<T> result, int? totalCount)
        {
            base.result = result;
            base.totalCount = totalCount;
        }

        public IEnumerable<T> getResult()
        {
            return (IEnumerable<T>)result;
        }
    }
}