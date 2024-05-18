using RIAPP.DataService.Annotations.Metadata;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RIAPP.DataService.Core.Metadata
{
    /// <summary>
    ///     Stores information about parameter
    ///     used to check values recieved from client
    ///     before service method invocations
    /// </summary>

    public class ParamMetadata
    {
        public ParamMetadata()
        {
            Name = "";
            DataType = DataType.None;
            Ordinal = -1;
            IsNullable = false;
            IsArray = false;
            DateConversion = DateConversion.None;
        }


        [Description("Parameter name")]
        public string Name { get; set; }


        [Description("Parameter type")]
        public DataType DataType { get; set; }


        [Description("True if parameter is array")]
        public bool IsArray { get; set; }


        [Description("Parameter position")]
        public bool IsNullable { get; set; }


        [Description("How adjust date timezone between server and client")]
        public DateConversion DateConversion { get; set; }


        [Description("Parameter position")]
        public int Ordinal { get; set; }

        internal Type _ParameterType { get; set; }


        /// <summary>
        ///     Extracts from ParameterInfo all information about method parameter
        /// </summary>
        /// <returns>ParamMetadataInfo</returns>
        public static ParamMetadata FromParamInfo(ParameterInfo pinfo, IValueConverter valueConverter)
        {
            Type ptype = pinfo.ParameterType;

            if (pinfo.IsOut)
            {
                throw new DomainServiceException("Out parameters are not supported in service methods");
            }

            ParamMetadata paramInfo = new ParamMetadata
            {
                IsNullable = ptype.IsNullableType(),
                Name = pinfo.Name
            };
            paramInfo.SetParameterType(ptype);
            Type realType = paramInfo.IsNullable ? Nullable.GetUnderlyingType(ptype) : ptype;

            IDateConversionData dateConvert = (IDateConversionData)pinfo.GetCustomAttributes(false).FirstOrDefault(a => a is IDateConversionData);

            if (dateConvert != null)
            {
                paramInfo.DateConversion = dateConvert.DateConversion;
            }

            paramInfo.IsArray = realType.IsArrayType();
            try
            {
                paramInfo.DataType = valueConverter.DataTypeFromType(realType);
            }
            catch (UnsupportedTypeException)
            {
                paramInfo.DataType = DataType.None;
            }

            return paramInfo;
        }
    }
}