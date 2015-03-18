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
            this.Id = reader.GetInt32("id");
            this.Name = reader.GetNullableString("name");
            this.DateOfBirth = reader.GetDateTime("dateofbirth");

            int index = reader.GetFieldIndex("Xml");
            var val = reader.GetObject(index) as byte[];
            this.Xml = (val != null) ? val : null;
        }
    }
}
