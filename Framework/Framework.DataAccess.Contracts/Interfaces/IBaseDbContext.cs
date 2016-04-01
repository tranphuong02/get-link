using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Framework.DataAccess.Contracts.Interfaces
{
    /// <summary>
    /// A <see cref="DbContext"/> instance represents a combination of the Unit Of Work and Repository patterns
    /// such that it can be used to query from a database and group together changes that will then be written back to the store as a unit.
    /// DbContext is conceptually similar to ObjectContext.
    /// </summary>
    public interface IBaseDbContext
    {
        /// <summary>
        /// Get <see cref="DbSet"/>
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Saves all changes made in <see langword="this"/> context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        int SaveChanges();

        /// <summary>
        /// Saves all changes made in this context to the underlying database async.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Gets a System.Data.Entity.Infrastructure.<see cref="DbEntityEntry"/> of TEntity object for
        /// the given entity providing access to information about the entity and the
        /// ability to perform actions on the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>An entry for the entity</returns>
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : class, new();

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.
        /// The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type.
        /// The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of <see langword="object"/> returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The <paramref name="parameters"/> to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="doNotEnsureTransaction"><see langword="false"/> - the transaction creation is not ensured; true - the transaction creation is ensured.</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);
    }
}