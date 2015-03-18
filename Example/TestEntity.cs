using System;
using AZMapper;

namespace Example
{
    public class TestEntity
    {
        [DbField("id")]
        public int Id { get; set; }

        [DbField("name")]
        public string Name { get; set; }
        
        [DbField("dateofbirth")]
        public DateTime DateOfBirth { get; set; }
        
        public byte[] Xml { get; set; }
    }
}
