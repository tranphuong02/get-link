using Framework.DataAccess.Contracts.Interfaces;
using Framework.DI.Contracts.Interfaces;

namespace Transverse.Interfaces.DAL
{
    public interface IRepository<TEntity> : IBaseRepository<TEntity>, IDependency where TEntity : class
    {
    }
}