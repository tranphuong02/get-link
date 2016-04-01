using System.Linq;
using Transverse.Interfaces.DAL;
using Transverse.Models.DAL;

namespace DatabaseAccess
{
    public class BlackListRepository : Repository<BlackList>, IBlackListRepository
    {
        public BlackListRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public bool IsInBlackList(string ipAddress)
        {
            return GetAll(x => x.IsActive && x.IsDelete == false && x.Ip == ipAddress).Any();
        }

        public bool IsInBlackList(int customerId)
        {
            return GetAll(x => x.IsActive && x.IsDelete == false && x.CustomerId == customerId).Any();
        }
    }
}