using System.ComponentModel.DataAnnotations;

namespace DiscordBot1.Permissions
{
    public class Permissions_User
    {
        [Required(ErrorMessage = "User must have a name")]
        public string Name;

        [Required(ErrorMessage = "User must have an ID")]
        public ulong ID;
    }
}