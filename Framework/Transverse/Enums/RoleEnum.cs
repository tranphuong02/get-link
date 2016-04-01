using System.ComponentModel;

namespace Transverse.Enums
{
    public enum RoleEnum
    {
        [Description("Supper Administrator")]
        SupperAdmin = 1,

        [Description("Administrator")]
        Admin = 2,

        [Description("Moderator")]
        Mod = 3,

        [Description("Writer")]
        Writer = 4
    }


}