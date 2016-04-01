//////////////////////////////////////////////////////////////////////
// File Name    : ISingletonDependency
// System Name  : Framework.DI.Contracts
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Framework.DI.Contracts.Interfaces
{
    /// <summary>
    /// Inherit from <see langword="this"/> <see langword="interface"/> to register unity dependency as a singleton
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }
}