using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    /// <summary>
    ///  Stores parameter description (it's attributes)
    /// </summary>

    public class MethodParameter
    {
        public MethodParameter()
        {
            Name = "";
            Value = null;
        }


        [Description("Parameter name")]
        public string Name { get; set; }



        [Description("Parameter value as string")]
        public string Value { get; set; }
    }


    public class MethodParameters
    {
        public MethodParameters()
        {
            Parameters = new List<MethodParameter>();
        }


        public List<MethodParameter> Parameters { get; set; }

        public object GetValue(string name, MethodDescription methodDescription, IDataHelper dataHelper)
        {
            MethodParameter par = Parameters.Where(p => p.Name == name).FirstOrDefault();
            if (par == null)
            {
                return null;
            }

            ParamMetadata paraminfo = methodDescription.parameters.Where(p => p.Name == name).FirstOrDefault();
            if (paraminfo == null)
            {
                throw new DomainServiceException(string.Format("Method: {0} has no parameter with a name: {1}",
                    methodDescription.methodName, name));
            }
            return dataHelper.ParseParameter(paraminfo.GetParameterType(), paraminfo, paraminfo.IsArray,
                par.Value);
        }
    }
}