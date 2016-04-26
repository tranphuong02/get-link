//////////////////////////////////////////////////////////////////////
// File Name    : FShareResultViewModel
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 3/31/2016 3:43:39 PM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Transverse.Models.Business.Getlink
{
    public class FShareResultViewModel
    {
        public bool is_required_ads { get; set; }
        public int ads_type { get; set; }
        public string ads_url { get; set; }
        public string file_name { get; set; }
        public string file_size { get; set; }
    }
}