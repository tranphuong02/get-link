using Microsoft.Practices.Unity;
using System.Web.Http;
using Transverse.Interfaces.Business;
using Transverse.Models.Business.Getlink;

namespace Web.Controllers.API
{
    [RoutePrefix("api/getlink")]
    public class GetlinkApiController : BaseApiController
    {
        [Dependency]
        public IGetlinkBusiness GetlinkBusiness { get; set; }

        [HttpPost]
        [Route("v1")]
        //[Transverse.Attributes.ValidateAntiForgeryToken]
        public IHttpActionResult GetLink([FromBody]GetlinkParamViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = GetlinkBusiness.Getlink(viewModel);
            return Ok(result);
        }
    }
}