using Microsoft.Practices.Unity;
using Transverse.Interfaces.Business;
using Transverse.Interfaces.DAL;

namespace BusinessLogic
{
    public class DatabaseMigrations : IDatabaseMigrationsBusiness
    {
        [Dependency]
        public IDatabaseMigrationsRepository DatabaseMigrationsRepository { get; set; }

        public void ApplyDatabaseMigrations()
        {
            DatabaseMigrationsRepository.ApplyDatabaseMigrations();
        }
    }
}