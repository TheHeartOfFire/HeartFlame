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
            if(context.User is SocketGuildUser gUser)
            {
                if(gUser.Id.ToString().Equals(Properties.Resources.CreatorID))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotCreator));

                if (_role == Roles.OWNER)
                    if(gUser.Id == gUser.Guild.OwnerId || gUser.Id.ToString().Equals(Properties.Resources.CreatorID))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotOwner));

                if (!GuildManager.GetGuild(gUser).ModuleControl.IncludePermissions)
                        if (gUser.GuildPermissions.Administrator || gUser.Id.ToString().Equals(Properties.Resources.CreatorID))
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotGuildAdmin));


                if (_role == Roles.MOD)
                    if (GuildManager.GetGuild(gUser).GetUser(gUser).Perms.Mod || gUser.Id.ToString().Equals(Properties.Resources.CreatorID))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotMod));

                if (_role == Roles.ADMIN)
                    if (GuildManager.GetGuild(gUser).GetUser(gUser).Perms.Admin || gUser.Id.ToString().Equals(Properties.Resources.CreatorID))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotAdmin));

            }

            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command. Wait what? How did you get here?"));
        }
    }
}
