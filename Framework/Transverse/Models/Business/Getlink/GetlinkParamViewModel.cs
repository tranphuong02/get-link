//////////////////////////////////////////////////////////////////////
// File Name    : GetlinkParamViewModel
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 3/31/2016 3:49:06 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace Transverse.Models.Business.Getlink
{
    public class GetlinkParamViewModel
    {
        [Required(ErrorMessage = @"Bạn vui lòng chọn host cần lấy link")]
        public int Type { get; set; }

        [Required(ErrorMessage = @"Bạn vui lòng nhập vào đường dẫn")]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = @"Đường dẫn không đúng")]
        public string Url { get; set; }

        public string Password { get; set; }

        public int? CustomerId { get; set; }

        public int CustomerType { get; set; }
    }
}