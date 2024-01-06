namespace RIAPP.DataService.Core.Types
{
    public static class DataServiceEx
    {
        public static DbSetInfo GetSetInfoByName(this IDataServiceComponent component, string name)
        {
            Metadata.RunTimeMetadata metadata = component.DataService.GetMetadata();
            return metadata.DbSets[name];
        }
    }
}