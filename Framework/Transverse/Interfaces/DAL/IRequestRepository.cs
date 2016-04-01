//////////////////////////////////////////////////////////////////////
// File Name    : IRequestRepository
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 4/1/2016 11:29:16 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using Transverse.Models.DAL;

namespace Transverse.Interfaces.DAL
{
    public interface IRequestRepository : IRepository<Request>
    {
        int TotalIpRequest(string ipAddress, DateTime date);
        int TotalCustomerRequest(int customerId, DateTime date);
    }
}