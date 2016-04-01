using Framework.DI.Contracts.Interfaces;

namespace Transverse.Interfaces.DAL
{
    public interface IDatabaseMigrationsRepository : IDependency
    {
        /// <summary>
        ///     Migration data
        /// </summary>
        void ApplyDatabaseMigrations();
    }
}