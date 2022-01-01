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

                    case Modules.TIME:
                        if (mods.IncludeTime)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotTime));

                    case Modules.COMMANDS:
                        if (mods.IncludeCustomCommands)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotCustomCommands));

                    case Modules.JOINMESSAGES:
                        if (mods.IncludeJoinMessages)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotJoin));

                    case Modules.PATCHNOTES:
                        if (mods.IncludePatchNotes)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotPatchNotes));

                    case Modules.COMP:
                        if (mods.IncludeComp)
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        else
                            return Task.FromResult(PreconditionResult.FromError(Properties.Resources.NotComp));
                }
            }

            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command. Wait what? How did you get here?"));
        }

        public static bool isModuleError(string Message)
        {
            if (Message.Equals(Properties.Resources.NotCustomCommands))return true;
            if (Message.Equals(Properties.Resources.NotTime))return true;
            if (Message.Equals(Properties.Resources.NotServerLogging))return true;
            if (Message.Equals(Properties.Resources.NotModeration))return true;
            if (Message.Equals(Properties.Resources.NotSelf))return true;
            if (Message.Equals(Properties.Resources.NotPerms))return true;
            if (Message.Equals(Properties.Resources.NotLogging))return true;
            if (Message.Equals(Properties.Resources.NotComp))return true;
            if (Message.Equals(Properties.Resources.NotChat))return true;
            if (Message.Equals(Properties.Resources.NotJoin)) return true;
            if (Message.Equals(Properties.Resources.NotPatchNotes)) return true;
            if (Message.Equals(Properties.Resources.NotComp)) return true;
            return false;
        }
    }
}
