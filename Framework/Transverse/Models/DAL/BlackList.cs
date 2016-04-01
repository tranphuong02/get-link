//////////////////////////////////////////////////////////////////////
// File Name    : BlackList
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 4/1/2016 11:41:54 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations.Schema;

namespace Transverse.Models.DAL
{
    public class BlackList : BaseModel
    {
        public string Ip { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}