//////////////////////////////////////////////////////////////////////
// File Name    : TempDataExtensions
// System Name  : BreezeGoodlife
// Summary      :
// Author       : phuong.tran
// Change Log   : 12/14/2015 1:37:42 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Web.Mvc;

namespace Framework.Utility
{
    public static class TempDataExtensions
    {
        public static void SetStatusMessage(this TempDataDictionary tempData, string message, UtilityEnum.StatusMessageType messageType = UtilityEnum.StatusMessageType.Success)
        {
            tempData[Constants.StatusMessage.StatusMessageText] = message;
            tempData[Constants.StatusMessage.StatusMessageType] = messageType;
        }
    }
}