using System;

namespace AZMapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DbFieldName : Attribute
    {
        public string Name { get; private set; }
        public bool ExcludeFromMapping { get; private set; }

        public DbFieldName(string name, bool exclude = false)
        {
            this.Name = name;
            this.ExcludeFromMapping = exclude;
        }
    }
}
