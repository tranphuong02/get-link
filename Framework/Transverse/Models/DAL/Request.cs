using System.ComponentModel.DataAnnotations.Schema;

namespace Transverse.Models.DAL
{
    public class Request : BaseModel
    {
        public string Ip { get; set; }
        public string RequestUrl { get; set; }
        public string RequestPassword { get; set; }
        public int RequestType { get; set; }
        public string ResultUrl { get; set; }
        public string ResultName { get; set; }
        public string ResultSize { get; set; }
        public string ResultAdsUrl { get; set; }
        public int ResultAdsType { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}