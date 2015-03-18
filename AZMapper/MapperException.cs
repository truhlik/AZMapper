using System;

namespace AZMapper
{
    public class MapperException : ApplicationException
    {
        public MapperException()
        {
        }

        public MapperException(string message)
            : base(message)
        {
        }

        public MapperException(string message, Exception innerExcetption)
            : base(message, innerExcetption)
        {

        }
    }
}
