//////////////////////////////////////////////////////////////////////
// File Name    : IDbContext
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 08/12/2015 12:05:29 AM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.DataAccess.Contracts.Interfaces;
using Framework.DI.Contracts.Interfaces;

namespace Transverse.Interfaces.DAL
{
    public interface IDbContext : IBaseDbContext, IPerRequestDependency
    {
    }
}