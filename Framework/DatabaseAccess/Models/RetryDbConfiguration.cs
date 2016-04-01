using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;

namespace DatabaseAccess.Models
{
    public class RetryDbConfiguration : DbConfiguration
    {
        public RetryDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () =>
                SuspendExecutionStrategy
                    ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
                    : new SqlAzureExecutionStrategy(5, TimeSpan.FromSeconds(15)));
        }

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData
                    ("SuspendExecutionStrategy") ?? false;
            }
            set { CallContext.LogicalSetData("SuspendExecutionStrategy", value); }
        }
    }
}