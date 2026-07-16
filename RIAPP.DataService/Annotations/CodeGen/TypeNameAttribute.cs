using System;

namespace RIAPP.DataService.Annotations.CodeGen
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TypeNameAttribute(string name) : Attribute
    {
        public string Name { get; set; } = name;
    }
}