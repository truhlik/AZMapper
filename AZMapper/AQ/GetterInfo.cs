using System;

namespace AZMapper.AQ
{
    public class GetterInfo
    {
        public Func<object, object> Getter { get; set; }
        public Type ReturnType { get; set; }
    }
}
