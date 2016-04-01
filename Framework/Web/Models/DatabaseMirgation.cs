using Framework.DI.Unity;
using Transverse.Interfaces.Business;

namespace Web.Models
{
    public static class DatabaseMirgation
    {
        public static void ApplyDatabaseMigrations()
        {
            IDatabaseMigrationsBusiness databaseMigrationsBusiness = IoCFactory.Instance.GetObjectInstance<IDatabaseMigrationsBusiness>();
            databaseMigrationsBusiness?.ApplyDatabaseMigrations();
        }
    }
}