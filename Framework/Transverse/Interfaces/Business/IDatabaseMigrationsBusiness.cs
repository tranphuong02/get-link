using Framework.DI.Contracts.Interfaces;

namespace Transverse.Interfaces.Business
{
    public interface IDatabaseMigrationsBusiness : IDependency
    {
        /// <summary>
        ///     Migration data
        /// </summary>
        void ApplyDatabaseMigrations();
    }
}