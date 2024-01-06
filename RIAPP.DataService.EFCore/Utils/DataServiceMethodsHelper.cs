using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System;
using System.Linq;
using System.Text;

namespace RIAPP.DataService.EFCore.Utils
{
    public static class DataServiceMethodsHelper
    {
        private static string GetTableName(DbContext DB, Type entityType)
        {
            Type tableType = typeof(DbSet<>).MakeGenericType(entityType);
            System.Reflection.PropertyInfo propertyInfo =
                DB.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType == tableType)
                    .FirstOrDefault();
            if (propertyInfo == null)
            {
                return string.Empty;
            }

            return propertyInfo.Name;
        }

        private static string CreateDbSetMethods(DbSetInfo dbSetInfo, string tableName)
        {
            StringBuilder sb = new StringBuilder(512);

            sb.AppendLine(string.Format("#region {0}", dbSetInfo.dbSetName));
            sb.AppendLine("[Query]");
            sb.AppendLine($"public async Task<QueryResult<{dbSetInfo.GetEntityType().Name}>> Read{dbSetInfo.dbSetName}()");
            sb.AppendLine("{");
            sb.AppendLine($"\tvar queryRes = await this.PerformQuery(this.DB.{tableName}, (countQuery) => countQuery.CountAsync());");
            sb.AppendLine("\tint? totalCount = await queryRes.Count;");
            sb.AppendLine("\tvar resList = await queryRes.Data.ToListAsync();");
            sb.AppendLine($"\treturn new QueryResult<{dbSetInfo.GetEntityType().Name}>(resList, totalCount);");
            sb.AppendLine("}");
            sb.AppendLine("");

            sb.AppendLine("[Insert]");
            sb.AppendLine($"public void Insert{dbSetInfo.dbSetName}({dbSetInfo.GetEntityType().Name} {dbSetInfo.dbSetName.ToLower()})");
            sb.AppendLine("{");
            sb.AppendLine($"\tthis.DB.{tableName}.Add({dbSetInfo.dbSetName.ToLower()});");
            sb.AppendLine("}");
            sb.AppendLine("");

            sb.AppendLine("[Update]");
            sb.AppendFormat("public void Update{1}({0} {2})", dbSetInfo.GetEntityType().Name, dbSetInfo.dbSetName, dbSetInfo.dbSetName.ToLower());
            sb.AppendLine("");
            sb.AppendLine("{");
            sb.AppendLine(string.Format("\t{0} orig = this.GetOriginal<{0}>();", dbSetInfo.GetEntityType().Name));
            sb.AppendLine(string.Format("\tvar entry = this.DB.{0}.Attach({1});", tableName, dbSetInfo.dbSetName.ToLower()));
            sb.AppendLine("\tentry.OriginalValues.SetValues(orig);");
            sb.AppendLine("}");
            sb.AppendLine("");

            sb.AppendLine("[Delete]");
            sb.AppendFormat("public void Delete{1}({0} {2})", dbSetInfo.GetEntityType().Name, dbSetInfo.dbSetName, dbSetInfo.dbSetName.ToLower());
            sb.AppendLine("");
            sb.AppendLine("{");
            sb.AppendLine(string.Format("\tthis.DB.{0}.Attach({1});", tableName, dbSetInfo.dbSetName.ToLower()));
            sb.AppendLine(string.Format("\tthis.DB.{0}.Remove({1});", tableName, dbSetInfo.dbSetName.ToLower()));
            sb.AppendLine("}");
            sb.AppendLine("");

            sb.AppendLine("#endregion");
            return sb.ToString();
        }

        public static string CreateMethods(RunTimeMetadata metadata, DbContext DB)
        {
            StringBuilder sb = new StringBuilder(4096);

            System.Collections.Generic.List<DbSetInfo> dbSets = metadata.DbSets.Values.OrderBy(d => d.dbSetName).ToList();
            dbSets.ForEach(dbSetInfo =>
            {
                string tableName = GetTableName(DB, dbSetInfo.GetEntityType());
                if (tableName == string.Empty)
                {
                    return;
                }

                sb.AppendLine(CreateDbSetMethods(dbSetInfo, tableName));
            });
            return sb.ToString();
        }
    }
}