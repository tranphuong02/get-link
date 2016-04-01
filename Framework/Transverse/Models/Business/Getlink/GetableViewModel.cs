//////////////////////////////////////////////////////////////////////
// File Name    : TotalRequestTodayViewModel
// System Name  : VST
// Summary      :
// Author       : phuong.tran
// Change Log   : 4/1/2016 1:08:02 PM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Transverse.Models.Business.Getlink
{
    public class GetableViewModel
    {
        public bool CanGet { get; set; }
        public bool IsRequiredAds { get; set; }
        public int TotalRequestToday { get; set; }
    }
}