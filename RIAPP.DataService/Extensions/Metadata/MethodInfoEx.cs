using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Metadata
{
    public static class MethodInfoEx
    {
        /// <summary>
        /// Gets only the requested method types from the supplied list
        /// </summary>
        /// <param name="allList"></param>
        /// <param name="methodTypes"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfoData> GetMethods(this IEnumerable<MethodInfoData> allList, MethodType methodTypes)
        {
            return allList.Where(info => (info.MethodType & methodTypes) == info.MethodType);
        }

        /// <summary>
        /// Gets only Query and Invoke methods from the supplied method lists
        /// </summary>
        /// <param name="allList"></param>
        /// <param name="valueConverter"></param>
        /// <returns></returns>
        public static MethodsList GetSvcMethods(this IEnumerable<MethodInfoData> allList, IValueConverter valueConverter)
        {
            MethodInfoData[] queryAndInvokes = allList.GetMethods(MethodType.Query | MethodType.Invoke).ToArray();
            MethodsList methodList = new MethodsList();

            Array.ForEach(queryAndInvokes, info =>
            {
                MethodDescription methodDescription = MethodDescription.FromMethodInfo(info, valueConverter);
                methodList.Add(methodDescription);
            });

            return methodList;
        }
    }
}