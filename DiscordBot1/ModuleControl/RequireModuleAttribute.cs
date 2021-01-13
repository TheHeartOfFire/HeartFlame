using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ModuleControl
{
    public class RequireModuleAttribute : PreconditionAttribute
    {
        private readonly Modules _module;

        public RequireModuleAttribute(Modules Module) => _module = Module;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser gUser)
            {
                var mods = GuildManager.GetGuild(gUser).ModuleControl;
                switch (_module)
                {
                    case Modules.CHAT:
                        if (mods.IncludeChat)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotChat));

                    case Modules.COMPENDIUM:
                        if (mods.IncludeCompendium)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotComp));

                    case Modules.LOGGING:
                        if (mods.IncludeLogging)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotLogging));

                    case Modules.PERMISSIONS:
                        if (mods.IncludePermissions)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotPerms));

                    case Modules.SELFASSIGN:
                        if (mods.IncludeSelfAssign)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotSelf));

                    case Modules.MODERATION:
                        if (mods.IncludeModeration)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotModeration));

                    case Modules.SERVERLOGGING:
                        if (mods.IncludeServerLogging)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotServerLogging));
                }
            }

            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command. Wait what? How did you get here?"));
        }
    }
}
