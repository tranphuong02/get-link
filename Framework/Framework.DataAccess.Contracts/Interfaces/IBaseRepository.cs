using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.DataAccess.Contracts.Interfaces
{
    /// <summary>
    /// IBaseRepository for entity framework
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// <see cref="Get"/> <see cref="IQueryable"/> list of all entities from DbSet
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        ///  Get an entity by ID
        /// </summary>
        /// <param name="ids">IDs of entity</param>
        TEntity GetById(params object[] ids);

        /// <summary>
        /// <see cref="Get"/> all entities from a DbSet
        /// </summary>
        IQueryable<TEntity> GetAll(params string[] includes);

        /// <summary>
        ///
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="navigationProperties"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] navigationProperties);

        /// <summary>
        /// Get ordered <see cref="IEnumerable"/> list of entities from a
        /// DbSet by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties);

        /// <summary>
        /// Insert a new <paramref name="entity"/> to DbSet
        /// </summary>
        /// <param name="entity">entity to insert</param>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// <see cref="Insert"/> a list entity to DbSet
        /// </summary>
        /// <param name="listEntity">list entity to insert</param>
        List<TEntity> InsertRange(IEnumerable<TEntity> listEntity);

        /// <summary>
        /// Update an existing <paramref name="entity"/> in DbSet
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="changedProes"></param>
        void Update(TEntity entity, params string[] changedProes);

        /// <summary>
        /// Delete an <paramref name="entity"/> in DbSet by set Deleted to true or remove from DbSet
        /// </summary>
        /// <param name="entity">entity to delete</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete an entity in DbSet by set Deleted to true or remove from DbSet
        /// </summary>
        /// <param name="id">id of entity to delete</param>
        TEntity Delete(dynamic id);

        /// <summary>
        /// Batch delete based on condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Batch delete based on condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int DeleteRange(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// <see cref="Delete"/> an list of <paramref name="entities"/> in DbSet by set Delete to true or remove from DbSet
        /// </summary>
        /// <param name="entities">list of entities to delete</param>
        int DeleteRange(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Count total entry in database by condition
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        ///     Check is exists any entry in database by condition
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Any(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        ///     Check is exists any entry in database by keys
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Any(params object[] ids);

        /// <summary>
        /// Reload the entity from database
        /// </summary>
        /// <param name="entityToReload"></param>
        void RefreshEntity(TEntity entityToReload);
    }
}