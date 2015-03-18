using System.Data;

namespace AZMapper
{
    public interface IMapable
    {
        void Map(IDataReader reader);
    }
}
