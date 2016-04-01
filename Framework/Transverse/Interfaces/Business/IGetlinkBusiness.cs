//////////////////////////////////////////////////////////////////////
// File Name    : IFShareBusiness
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 3/31/2016 3:37:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Threading.Tasks;
using Framework.DI.Contracts.Interfaces;
using HtmlAgilityPack;
using Transverse.Models.Business;
using Transverse.Models.Business.Getlink;

namespace Transverse.Interfaces.Business
{
    public interface IGetlinkBusiness : IDependency
    {
        HtmlDocument HtmlDocument(string html);

        string GetFsCsrf(HtmlDocument document);

        BaseModel Getlink(GetlinkParamViewModel viewModel);

        FShareResultViewModel GetFShareLink(string url, string password, int totalRequestToday, bool isRequiredAds);
    }
}