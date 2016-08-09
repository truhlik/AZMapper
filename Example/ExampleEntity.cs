using System;
using AZMapper;
using AZMapper.Extensions;

namespace Example
{
    public class ExampleEntity : IMapable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte[] Xml { get; set; }

        public void Map(System.Data.IDataReader reader)
        {
            this.Id = reader.GetInt32Ext("id");
            this.Name = reader.GetNullableStringExt("name");
            this.DateOfBirth = reader.GetDateTimeExt("dateofbirth");

            int index = reader.GetOrdinal("Xml");
            var val = reader.GetObjectExt(index) as byte[];
            this.Xml = (val != null) ? val : null;
        }
    }
}
