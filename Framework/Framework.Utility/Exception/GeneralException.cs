//////////////////////////////////////////////////////////////////////
// File Name    : GeneralException
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/12/2015 11:22:58 AM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Framework.Utility.Exception
{
    /// <summary>
    /// General exception
    /// </summary>
    public class GeneralException : System.Exception
    {
        /// <summary>
        /// </summary>
        public GeneralException()
            : base()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public GeneralException(string message)
            : base(message)
        {
        }
    }
}