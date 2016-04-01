//////////////////////////////////////////////////////////////////////
// File Name    : HtmlHelperExtensions
// System Name  : Framework.Utility
// Summary      :
// Author       : phuong.tran
// Change Log   : 14/12/2015 13:30:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.Web.Mvc;
using System.Web.Routing;

namespace Framework.Utility
{
    /// <summary>
    /// Razor Helper Extension
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        ///  If the current <paramref name="controller"/> and action match the values you're passing in, add css class for it
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static string ActivePage(this HtmlHelper helper, string controller, string action, string cssClass)
        {
            string classValue = string.Empty;

            string currentController = helper.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
            string currentAction = helper.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString();
            if (string.IsNullOrWhiteSpace(action))
            {
                if (currentController == controller)
                {
                    classValue = cssClass;
                }
            }
            else
            {
                if (currentController == controller && currentAction == action)
                {
                    classValue = cssClass;
                }
            }

            return classValue;
        }

        public static MvcHtmlString ShowStatusMessage(this HtmlHelper htmlHelper, UtilityEnum.StatusMessageType messageType = UtilityEnum.StatusMessageType.Success, string messageText = null, object htmlAttributes = null)
        {
            // required for legacy webform pages
            if (htmlHelper == null)
                return MvcHtmlString.Empty;

            var tempData = htmlHelper.ViewContext.TempData;

            if (string.IsNullOrEmpty(messageText))
            {
                messageText = (string)tempData[Constants.StatusMessage.StatusMessageText];
            }

            if (string.IsNullOrEmpty(messageText))
                return MvcHtmlString.Empty;

            // if dictionary contains keys for type use appropriate StatusMessage overload
            if (tempData["StatusMessageType"] != null)
            {
                messageType = (UtilityEnum.StatusMessageType)tempData[Constants.StatusMessage.StatusMessageType];
            }

            var innerSpan = new TagBuilder("span") { InnerHtml = messageText };

            var outerDiv = new TagBuilder("div")
            {
                InnerHtml = "<button type='button' class='close' data-dismiss='alert'" +
                            " aria-hidden='true'>&times;</button>" +
                            innerSpan
            };

            var attribs = htmlAttributes == null ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            outerDiv.MergeAttributes(attribs);
            outerDiv.AddCssClass("alert alert-dismissable");

            switch (messageType)
            {
                case UtilityEnum.StatusMessageType.Success:
                    outerDiv.AddCssClass("alert-success");
                    break;

                case UtilityEnum.StatusMessageType.Info:
                    outerDiv.AddCssClass("alert-info");
                    break;

                case UtilityEnum.StatusMessageType.Warning:
                    outerDiv.AddCssClass("alert-warning");
                    break;

                case UtilityEnum.StatusMessageType.Danger:
                    outerDiv.AddCssClass("alert-danger");
                    break;
            }

            return MvcHtmlString.Create(outerDiv.ToString());
        }
    }
}