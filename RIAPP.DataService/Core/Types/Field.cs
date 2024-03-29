﻿using System;
using System.ComponentModel;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    /// <summary>
    ///     Stores field description (it's attributes)
    /// </summary>
    public class Field
    {
        private readonly Lazy<FieldsList> _nested;
        private readonly Lazy<Field[]> _nestedInResultFields;

        public Field()
        {
            _nested = new Lazy<FieldsList>(() => fieldType == FieldType.Object ? new FieldsList() : null, true);
            _nestedInResultFields = new Lazy<Field[]>(() => fieldType == FieldType.Object
                            ? nested.Where(f => f.GetIsIncludeInResult()).OrderBy(f => f.GetOrdinal()).ToArray()
                            : new Field[0], true);
            isPrimaryKey = 0;
            dataType = DataType.None;
            isNullable = true;
            maxLength = -1;
            isReadOnly = false;
            isAutoGenerated = false;
            allowClientDefault = false;
            dateConversion = DateConversion.None;
            fieldType = FieldType.None;
            isNeedOriginal = true;

            dataTypeName = "";
            range = "";
            regex = "";
            dependentOn = "";
            _ordinal = -1;
        }


        public string fieldName { get; set; }

        [DefaultValue((short)0)]

        [Description("If this field is a primary key then it must be greater then 0")]
        public short isPrimaryKey { get; set; }


        [Description("Sets value type - None, String, Bool, Integer, Decimal, Float, DateTime, Date, Time, Guid, Binary, Custom")]
        public DataType dataType { get; set; }

        [Description("For Custom dataType, for other types it is ignored. It is used for code generation, and not serialized to the client")]
        internal string dataTypeName { get; set; }

        [DefaultValue(true)]

        public bool isNullable { get; set; }

        [DefaultValue(false)]

        public bool isReadOnly { get; set; }

        [DefaultValue(false)]

        [Description("On insert the value is automatically generated on the server")]
        public bool isAutoGenerated { get; set; }

        [DefaultValue(true)]

        public bool isNeedOriginal { get; set; }

        [DefaultValue(-1)]

        [Description("Sets the limit on text size, on value editing")]
        public int maxLength { get; set; }

        [DefaultValue(DateConversion.None)]

        [Description("Determines how to convert dates between server and client")]
        public DateConversion dateConversion { get; set; }

        [DefaultValue(false)]

        [Description("Applies when value is set readonly, and means that on insert it's value can be assigned on the client")]
        public bool allowClientDefault { get; set; }

        /// <summary>
        ///     to check values for being inside the allowed range
        ///     set range as minBound,maxBound to check for lower and upper bounds
        ///     set range as minBound, to check for only lower bound or
        ///     set range as ,naxBound to check for only upper bound
        ///     as for a example 0,100 or 0, or ,100
        ///     for dates format must be yyyy-MM-dd
        /// </summary>
        [DefaultValue("")]

        public string range { get; set; }

        /// <summary>
        ///  checks values with regex expression
        /// </summary>
        [DefaultValue("")]

        public string regex { get; set; }

        [DefaultValue(FieldType.None)]

        [Description("Sets field type - None, ClientOnly, Calculated, Navigation, RowTimeStamp, Object, ServerCalculated")]
        public FieldType fieldType { get; set; }

        /// <summary>
        ///     if this field is a calculated field
        ///     this property can return a string of fields on which this calculated field is dependent
        ///     each field is separated by comma
        /// </summary>
        [DefaultValue("")]

        public string dependentOn { get; set; }

        /// <summary>
        ///     If the field is a complex type field
        ///     it returns a list of its properties
        ///     and each child property can be also a complex type property
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public FieldsList nested => _nested.Value;

        internal int _ordinal { get; set; }

        // used only for Navigation and ComplexType fields
        internal string _TypeScriptDataType { get; set; }

        internal string _FullName { get; set; }

        public Field[] GetNestedInResultFields()
        {
            return _nestedInResultFields.Value;
        }

        public bool GetIsIncludeInResult()
        {
            return
                !(fieldType == FieldType.Calculated || fieldType == FieldType.ClientOnly ||
                  fieldType == FieldType.Navigation);
        }

        public bool IsHasNestedFields()
        {
            return _nested.IsValueCreated && _nested.Value.Any();
        }
    }
}