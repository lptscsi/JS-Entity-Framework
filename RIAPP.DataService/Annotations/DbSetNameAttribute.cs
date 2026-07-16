using System;

namespace RIAPP.DataService.Annotations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DbSetNameAttribute(string name) : Attribute
    {
        public string DbSetName { get; } = name;
    }
}