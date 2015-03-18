using System;

namespace AZMapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DbField : Attribute
    {
        public string Name { get; private set; }
        public bool ExcludeFromMapping { get; private set; }

        public DbField(string name, bool exclude = false)
        {
            this.Name = name;
            this.ExcludeFromMapping = exclude;
        }
    }
}
