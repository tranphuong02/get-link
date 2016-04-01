using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Transverse.Models.Business;

namespace Transverse.Utils.Validations
{
    /// <summary>
    /// Validate model binding
    /// Return: internal server error status (500) and error messages
    /// </summary>
    public class ApiValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errorModel =
                    modelState.Keys.Where(x => modelState[x].Errors.Count > 0).Select(x => new
                    {
                        key = x,
                        errors = string.Join("<br/>", modelState[x].Errors.Select(y => y.ErrorMessage))
                    }).ToList();

                var errorResponse = new BaseModel()
                {
                    IsSuccess = false,
                    Data = errorModel,
                    Message = "Data is invalid.",
                    ErrorCode = (int)HttpStatusCode.InternalServerError
                };
                var response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errorResponse);
                actionContext.Response = response;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}