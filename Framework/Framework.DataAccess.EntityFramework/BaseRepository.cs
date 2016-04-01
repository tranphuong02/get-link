using EntityFramework.Extensions;
using Framework.DataAccess.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.DataAccess.EntityFramework
{
    /// <summary>
    /// Repository contain all of base method to work with db, very easy to inheritance, very easy to upgrade or change method
    /// one time for all project
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        #region Fields

        /// <summary>
        /// <see cref="DbContext"/>
        /// </summary>
        private readonly IBaseDbContext _dbContext;

        /// <summary>
        /// <see cref="DbSet"/>
        /// </summary>
        private IDbSet<TEntity> _dbSet;

        #endregion Fields

        /// <summary>
        /// Set <c>Dbcontext</c> for repository
        /// </summary>
        /// <param name="dbContext"></param>
        public BaseRepository(IBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Properties

        /// <summary>
        /// Getter Setter for DbSet
        /// </summary>
        private IDbSet<TEntity> DbSet
        {
            get
            {
                if (_dbSet != null)
                {
                    return _dbSet;
                }
                _dbSet = _dbContext.Set<TEntity>();
                return _dbSet;
            }
        }

        #endregion Properties

        #region Get

        /// <summary>
        /// <see cref="Get"/> Table
        /// </summary>
        public IQueryable<TEntity> Table => DbSet;

        /// <summary>
        /// Get by <paramref name="ids"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual TEntity GetById(params object[] ids)
        {
            return DbSet.Find(ids);
        }

        /// <summary>
        /// <see cref="Get"/> records all with <paramref name="includes"/>
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAll(params string[] includes)
        {
            return this.Get(null, null, includes);
        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            IQueryable<TEntity> dbQuery = this.Table;

            //Apply eager loading
            dbQuery = navigationProperties.Aggregate(dbQuery, (current, navigationProperty) => current.Include(navigationProperty));

            if (where != null)
                dbQuery = dbQuery.Where(where);

            return orderBy != null ? orderBy(dbQuery) : dbQuery;
        }

        /// <summary>
        /// Get records with condition and includes
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        /// <exception cref="Exception">A <see langword="delegate"/> callback throws an exception.</exception>
        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            var query = Table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return orderBy != null ? orderBy(query) : query;
        }

        #endregion Get

        #region Insert

        /// <summary>
        /// Insert an <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity Insert(TEntity entity)
        {
            return DbSet.Add(entity);
        }

        /// <summary>
        /// <see cref="Insert"/> list <paramref name="listEntity"/>
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual List<TEntity> InsertRange(IEnumerable<TEntity> listEntity)
        {
            return listEntity.Select(entity => DbSet.Add(entity)).ToList();
        }

        #endregion Insert

        #region Update

        /// <summary>
        /// Update and <paramref name="entity"/> with specific properties
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="changedProes"></param>
        public virtual void Update(TEntity entity, params string[] changedProes)
        {
            if (changedProes == null)
            {
                changedProes = new string[] { };
            }

            DbSet.Attach(entity);
            if (changedProes.Any())
            {
                //Only change some properties
                foreach (string property in changedProes)
                {
                    _dbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion Update

        #region Delete

        /// <summary>
        /// Delete an <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="Exception">cannot delete an entity.</exception>
        public virtual void Delete(TEntity entity)
        {
            try
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                DbSet.Remove(entity);
            }
            catch (Exception)
            {
                RefreshEntity(entity);
                throw;
            }
        }

        /// <summary>
        /// Delete an entity by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Delete(dynamic id)
        {
            var entity = GetById(id);
            Delete(entity);
            return entity;
        }

        /// <summary>
        /// <c>Delete</c> list entity by condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).DeleteAsync();
        }

        /// <summary>
        /// <c>Delete</c> list entity by condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.Where(predicate).Delete();
        }

        /// <summary>
        /// <c>Delete</c> list entity by condition
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Cannot delete entity.</exception>
        public virtual int DeleteRange(IEnumerable<TEntity> entities)
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            try
            {
                var result = ((DbSet<TEntity>)DbSet).RemoveRange(enumerable);

                return result.Count();
            }
            catch
            {
                foreach (var entity in enumerable)
                {
                    RefreshEntity(entity);
                }
                throw;
            }
        }

        #endregion Delete

        #region Count

        /// <summary>
        /// Count by condition
        /// </summary>
        /// <param name="filter"></param>
        /// <exception cref="OverflowException">The number of elements in <paramref name="Table" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ? Table.Count() : Table.Count(filter);
        }

        /// <summary>
        /// Check is exists by condition
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="Table" /> is null.</exception>
        public bool Any(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ? Table.AsNoTracking().Any() : Table.AsNoTracking().Any(filter);
        }

        /// <summary>
        /// Check exists by <paramref name="ids"/>
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool Any(params object[] ids)
        {
            return GetById(ids) != null;
        }

        #endregion Count

        #region Helpers

        /// <summary>
        /// Create <paramref name="query"/> with parameters
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual DbSqlQuery<TEntity> GetWithRawSql(string query, object[] parameters)
        {
            return ((DbSet<TEntity>)DbSet).SqlQuery(query, parameters);
        }

        /// <summary>
        /// Refresh state of entity
        /// </summary>
        /// <param name="entityToReload"></param>
        public virtual void RefreshEntity(TEntity entityToReload)
        {
            _dbContext.Entry(entityToReload).Reload();
        }

        #endregion Helpers
    }
}