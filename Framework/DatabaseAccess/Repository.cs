using Framework.DataAccess.EntityFramework;
using Transverse.Interfaces.DAL;

namespace DatabaseAccess
{
    public class Repository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public Repository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}