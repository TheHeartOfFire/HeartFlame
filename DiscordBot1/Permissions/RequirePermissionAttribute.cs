using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Permissions
{
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        private readonly Roles _role;

        public RequirePermissionAttribute(Roles Role) => _role = Role;
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.User is SocketGuildUser User)
            {
                var Guild = GuildManager.GetGuild(User);
                var GUser = Guild.GetUser(User);
                bool Creator = User.Id.ToString().Equals(Properties.Resources.CreatorID);
                bool Owner = User.Id == User.Guild.OwnerId;

                if (Creator) return Task.FromResult(PreconditionResult.FromSuccess());//if creator. Always true

                if (_role == Roles.CREATOR) return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotCreator));//If you're here and the _role is CREATOR then fail

                if (Owner) return Task.FromResult(PreconditionResult.FromSuccess());//If _role is not Creator and user is Owner always return true

                if (_role == Roles.OWNER) return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotOwner));//If you're here you're not owner or creator. If _role is Owner then fail

                if (!Guild.ModuleControl.IncludePermissions)//If permissions are not used then Creator, Owner, and Administrators will all return true. Else fail
                    if (User.GuildPermissions.Administrator) return Task.FromResult(PreconditionResult.FromSuccess());
                    else return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotGuildAdmin));

                if (GUser.Perms.Admin) return Task.FromResult(PreconditionResult.FromSuccess());//At this point if bot Admin then return true

                if (_role == Roles.ADMIN) return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotAdmin));//if you're here then you're not Creator, Owner or bot Admin. if _role is Admin then fail

                if (GUser.Perms.Mod) return Task.FromResult(PreconditionResult.FromSuccess());//At this point if you have any permissions at all return true

                if (_role == Roles.MOD) return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotMod));//if _role is Mod and you have no permissions at all, fail.
            }

            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command. Wait what? How did you get here?"));
        }

        public static bool isPermissionsError(string Message)
        {
            if (Message.Equals(Properties.Resources.NotGuildAdmin)) return true;
            if (Message.Equals(Properties.Resources.NotOwner)) return true;
            if (Message.Equals(Properties.Resources.NotCreator)) return true;
            if (Message.Equals(Properties.Resources.NotAdmin)) return true;
            if (Message.Equals(Properties.Resources.NotMod)) return true;
            return false;
        }
    }
}
