using System;
using System.Data.Entity;
using System.Linq;
using Framework.Utility;
using Transverse.Interfaces.DAL;
using Transverse.Models.DAL;

namespace DatabaseAccess
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        public RequestRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public int TotalIpRequest(string ipAddress, DateTime date)
        {
            return Count(x => x.IsDelete == false && x.IsActive && x.Ip == ipAddress && DbFunctions.TruncateTime(x.CreatedDate) == date.Date);
        }

        public int TotalCustomerRequest(int customerId, DateTime date)
        {
            return Count(x => x.IsDelete == false && x.IsActive && x.CustomerId == customerId && DbFunctions.TruncateTime(x.CreatedDate) == date.Date);
        }
    }
}