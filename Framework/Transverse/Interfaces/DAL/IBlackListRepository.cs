//////////////////////////////////////////////////////////////////////
// File Name    : IBlackListRepository
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 4/1/2016 11:29:16 AM - Create Date
/////////////////////////////////////////////////////////////////////

using Transverse.Models.DAL;

namespace Transverse.Interfaces.DAL
{
    public interface IBlackListRepository : IRepository<BlackList>
    {
        bool IsInBlackList(string ipAddress);
        bool IsInBlackList(int customerId);
    }
}