using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    public interface IFieldsList: IReadOnlyList<Field>
    {
        public Field[] GetInResultFields();

        public Field[] GetPKFields();

        public Field GetTimeStampField();

        public IReadOnlyDictionary<string, Field> GetFieldByNames();
    }

    public class FieldsList : List<Field>, IFieldsList
    {
        private Dictionary<string, Field> _fieldsByNames = new Dictionary<string, Field>();
        private readonly Lazy<Field[]> _inResultFields;
        private readonly Lazy<Field[]> _pkFields;
        private readonly Lazy<Field> _timestampField;
        private bool _isInitialized = false;

        public FieldsList() {
            _inResultFields = new Lazy<Field[]>(
              () => this.Where(f => f.GetIsIncludeInResult()).OrderBy(f => f.GetOrdinal()).ToArray(), true);
            _pkFields = new Lazy<Field[]>(
                    () => this.Where(fi => fi.isPrimaryKey > 0).OrderBy(fi => fi.isPrimaryKey).ToArray(), true);
            _timestampField = new Lazy<Field>(() => this.Where(fi => fi.fieldType == FieldType.RowTimeStamp).FirstOrDefault(),
                    true);
        }

        public FieldsList(IFieldsList other)
            : this()
        {
           this.AddRange(other);
        }

        public Field[] GetInResultFields()
        {
            return _inResultFields.Value;
        }

        public Field[] GetPKFields()
        {
            return _pkFields.Value;
        }

        public Field GetTimeStampField()
        {
            return _timestampField.Value;
        }

        public IReadOnlyDictionary<string, Field> GetFieldByNames()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("The FieldList is not initialized");
            }
            return _fieldsByNames;
        }

        public void Initialize(IDataHelper dataHelper)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("The FieldList is already initialized");
            }
   
            static void SetOrdinal(Field[] fieldInfos)
            {
                int cnt = fieldInfos.Length;
                for (int i = 0; i < cnt; ++i)
                {
                    fieldInfos[i].SetOrdinal(i);
                    if (fieldInfos[i].fieldType == FieldType.Object)
                    {
                        SetOrdinal(fieldInfos[i].nested.ToArray());
                    }
                }
            }

            Field[] fieldInfos = this.ToArray();
            int cnt = fieldInfos.Length;

            for (int i = 0; i < cnt; ++i)
            {
                dataHelper.ForEachFieldInfo("", fieldInfos[i], (fullName, fieldInfo) =>
                {
                    fieldInfo.SetFullName(fullName);
                    _fieldsByNames.Add(fullName, fieldInfo);
                });
            }

            SetOrdinal(fieldInfos);
            Field[] pkFields = this.GetPKFields();

            if (pkFields.Length < 1)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_DBSET_HAS_NO_PK, ""));
            }

            _isInitialized = true;
        }
    }
}
