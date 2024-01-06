using System;

namespace RIAPP.DataService.Annotations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DbSetNameAttribute : Attribute
    {
        public DbSetNameAttribute(string name)
        {
            this.DbSetName = name;
        }

        public string DbSetName { get; }
    }
}