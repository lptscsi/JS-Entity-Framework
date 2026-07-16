using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core
{
    internal class SubsetsGenerator(RunTimeMetadata metadata, IDataHelper dataHelper)
    {
        private readonly IDataHelper _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
        private readonly RunTimeMetadata _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

        public SubsetList CreateSubsets(IEnumerable<SubResult> subResults)
        {
            SubsetList result = [];
            if (subResults == null)
            {
                return result;
            }

            foreach (SubResult subResult in subResults)
            {
                DbSetInfo dbSetInfo = _metadata.DbSets[subResult.dbSetName];

                if (result.Any(r => r.DbSetName == subResult.dbSetName))
                {
                    throw new DomainServiceException(string.Format("The included sub results already have DbSet {0} entities", dbSetInfo.dbSetName));
                }

                RowGenerator rowGenerator = new(dbSetInfo, subResult.Result, _dataHelper);

                Subset current = new()
                {
                    DbSetName = dbSetInfo.dbSetName,
                    Rows = rowGenerator.CreateDistinctRows(),
                    Columns = dbSetInfo.GetColumns()
                };

                result.Add(current);
            }

            return result;
        }
    }
}