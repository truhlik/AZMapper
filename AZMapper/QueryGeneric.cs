using System.Data.Common;

namespace AZMapper
{
    public abstract class Query<C, P> : Query
        where C : DbCommand, new()
        where P : DbParameter, new()
    {
        protected Query()
        {
            _command = new C();
        }

        protected override DbParameter GetEmptyParameter()
        {
            return new P();
        }
    }
}
