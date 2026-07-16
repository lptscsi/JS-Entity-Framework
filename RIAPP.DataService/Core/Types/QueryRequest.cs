namespace RIAPP.DataService.Core.Types
{

    public class QueryRequest : IUseCaseRequest<QueryResponse>
    {

        public string DbSetName { get; set; }


        public string QueryName { get; set; }


        public FilterInfo FilterInfo { get; set; } = new FilterInfo();


        public SortInfo SortInfo { get; set; } = new SortInfo();


        public MethodParameters ParamInfo { get; set; } = new MethodParameters();


        public int PageIndex { get; set; }


        public int PageSize { get; set; }


        public int PageCount { get; set; }



        public bool IsIncludeTotalCount { get; set; }

        internal DbSetInfo _dbSetInfo { get; set; }
    }
}