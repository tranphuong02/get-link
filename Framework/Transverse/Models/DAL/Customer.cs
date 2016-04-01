using System.Collections.Generic;

namespace Transverse.Models.DAL
{
    public class Customer : BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}