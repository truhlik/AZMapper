using System;
using AZMapper;

namespace Example
{
    public class TestEntity
    {
        [DbFieldName("id")]
        public int Id { get; set; }

        [DbFieldName("name")]
        public string Name { get; set; }
        
        [DbFieldName("dateofbirth")]
        public DateTime DateOfBirth { get; set; }
        
        public byte[] Xml { get; set; }
    }
}
