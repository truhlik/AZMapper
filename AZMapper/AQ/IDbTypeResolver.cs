using System.Data;
using System;

namespace AZMapper.AQ
{
    public interface IDbTypeResolver
    {
        DbType FromType(Type type);
    }
}
