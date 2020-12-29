using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord.WebSocket;
using HeartFlame.Misc;
using System.Threading.Tasks;
using HeartFlame.GuildControl;

namespace HeartFlame.SelfAssign
{
    public class SelfAssign
    {
        private static readonly GuildPermissions NoPerms = new GuildPermissions();


        public static async Task<Embed> PrefabConsoleAsync(ulong GuildID)
        { 
            foreach(var BotGuild in PersistentData.Data.Guilds)
            {
                if(BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.Consoles = new RoleCategory<RoleObject>()
                    {
                        Roles = new List<RoleObject>()
                    };

                    var Guild = Program.Client.GetGuild(GuildID);
                    var GuildRoles = new List<SocketRole>(Guild.Roles);

                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣"))
                        await Guild.CreateRoleAsync("⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣", NoPerms, new Color(0x2f3136), false, false);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Xbox"))
                        await Guild.CreateRoleAsync("Xbox", NoPerms, Color.Green, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "PlayStation"))
                        await Guild.CreateRoleAsync("PlayStation", NoPerms, Color.Blue, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "PC"))
                        await Guild.CreateRoleAsync("PC", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Nintendo"))
                        await Guild.CreateRoleAsync("Nintendo", NoPerms, Color.Red, false, true);
                    GuildRoles = new List<SocketRole>(Guild.Roles);

                    BotGuild.SelfAssign.Consoles.Name = "Consoles";
                    BotGuild.SelfAssign.Consoles.Title = "Consoles";

                    AddConsole("Xbox", EmoteRef.Emotes.GetValueOrDefault("Xbox"), 1, GuildRoles.FirstOrDefault(x => x.Name == "Xbox").Id, GuildID);
                    AddConsole("PlayStation", EmoteRef.Emotes.GetValueOrDefault("PlayStation"), 2, GuildRoles.FirstOrDefault(x => x.Name == "PlayStation").Id, GuildID);
                    AddConsole("PC", EmoteRef.Emotes.GetValueOrDefault("PC"), 3, GuildRoles.FirstOrDefault(x => x.Name == "PC").Id, GuildID);
                    AddConsole("Nintendo", EmoteRef.Emotes.GetValueOrDefault("Nintendo"), 4, GuildRoles.FirstOrDefault(x => x.Name == "Nintendo").Id, GuildID);

                    SetConsoleDivider(GuildRoles.FirstOrDefault(x => x.Name == "⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣").Id, GuildID);

                    return ConsoleEmbed(GuildID);
                }
            }

            return null;
        }

        public static async Task<Embed> PrefabTimeAsync(ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.TimeZones = new RoleCategory<RoleObject>()
                    {
                        Roles = new List<RoleObject>()
                    };

                    var Guild = Program.Client.GetGuild(GuildID);
                    var GuildRoles = new List<SocketRole>(Guild.Roles);

                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣"))
                        await Guild.CreateRoleAsync("⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣", NoPerms, new Color(0x2f3136), false, false);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Pacific (-7)")) await Guild.CreateRoleAsync("US Pacific (-7)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Mountain (-6)")) await Guild.CreateRoleAsync("US Mountain (-6)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Central (-5)")) await Guild.CreateRoleAsync("US Central (-5)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Eastern (-4)")) await Guild.CreateRoleAsync("US Eastern (-4)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Greenwich (+0)")) await Guild.CreateRoleAsync("Greenwich (+0)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "UK (+1)")) await Guild.CreateRoleAsync("UK (+1)", NoPerms, Color.Default, false, true);
                    if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Australia (+10)")) await Guild.CreateRoleAsync("Australia (+10)", NoPerms, Color.Default, false, true);

                    GuildRoles = new List<SocketRole>(Guild.Roles);

                    BotGuild.SelfAssign.TimeZones.Name = "TimeZones";
                    BotGuild.SelfAssign.TimeZones.Title = "TimeZones";

                    AddTime("US Pacific (-7)", EmoteRef.Emotes.GetValueOrDefault("1"), 1, GuildRoles.FirstOrDefault(x => x.Name == "US Pacific (-7)").Id, GuildID);
                    AddTime("US Mountain (-6)", EmoteRef.Emotes.GetValueOrDefault("2"), 2, GuildRoles.FirstOrDefault(x => x.Name == "US Mountain (-6)").Id, GuildID);
                    AddTime("US Central (-5)", EmoteRef.Emotes.GetValueOrDefault("3"), 3, GuildRoles.FirstOrDefault(x => x.Name == "US Central (-5)").Id, GuildID);
                    AddTime("US Eastern (-4)", EmoteRef.Emotes.GetValueOrDefault("4"), 4, GuildRoles.FirstOrDefault(x => x.Name == "US Eastern (-4)").Id, GuildID);
                    AddTime("Greenwich (+0)", EmoteRef.Emotes.GetValueOrDefault("5"), 5, GuildRoles.FirstOrDefault(x => x.Name == "Greenwich (+0)").Id, GuildID);
                    AddTime("UK (+1)", EmoteRef.Emotes.GetValueOrDefault("6"), 6, GuildRoles.FirstOrDefault(x => x.Name == "UK (+1)").Id, GuildID);
                    AddTime("Australia (+10)", EmoteRef.Emotes.GetValueOrDefault("7"), 7, GuildRoles.FirstOrDefault(x => x.Name == "Australia (+10)").Id, GuildID);

                    SetTimeDivider(GuildRoles.FirstOrDefault(x => x.Name == "⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣").Id, GuildID);

                    return TimeEmbed(GuildID);
                }
            }
            return null;
        }

        public static void AddConsole(string Name, string Emoji, int Position, ulong RoleID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    foreach (var role in BotGuild.SelfAssign.Consoles.Roles)
                    {
                        if (role.Position >= Position)
                            role.Position++;
                    }

                    BotGuild.SelfAssign.Consoles.Roles.Add(new RoleObject
                    {
                        Name = Name,
                        Emoji = Emoji,
                        Position = Position,
                        RoleID = RoleID
                    });

                    BotGuild.SelfAssign.Consoles.Roles.Sort();
                    PersistentData.SaveChangesToJson();
                }
            }
        }

        public static void AddTime(string Name, string Emoji, int Position, ulong RoleID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    foreach (var role in BotGuild.SelfAssign.TimeZones.Roles)
                    {
                        if (role.Position >= Position)
                        {
                            role.Position++;
                            Emoji = EmoteRef.Emotes.GetValueOrDefault(Position.ToString());
                        }
                    }
                    BotGuild.SelfAssign.TimeZones.Roles.Add(new RoleObject
                    {
                        Name = Name,
                        Emoji = Emoji,
                        Position = Position,
                        RoleID = RoleID
                    });

                    BotGuild.SelfAssign.TimeZones.Roles.Sort();
                    PersistentData.SaveChangesToJson();
                }
            }
        }

        public static Embed ConsoleEmbed(ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithDescription($"Please select the reaction corresponding to the consoles you use. Removing your reaction will remove the role from you.");

                    foreach (var role in BotGuild.SelfAssign.Consoles.Roles)
                    {
                        Embed.AddField($"{role.Emoji} for the {role.Name} role.", ".");
                    }
                    Embed.WithFooter("");//edit information
                    return Embed.Build();
                }
            }
            return null;
        }

        public static Embed TimeEmbed(ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithDescription($"Please select the reaction corresponding to your timezone. Removing your reaction will remove the role from you.");

                    foreach (var role in BotGuild.SelfAssign.TimeZones.Roles)
                    {
                        Embed.AddField($"{role.Emoji} for the {role.Name} role.", ".");
                    }
                    Embed.WithFooter("");//edit information
                    return Embed.Build();
                }
            }
            return null;
        }

        public static void SetConsoleMessageID(ulong MessageID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.Consoles.MsgID = MessageID;
                    PersistentData.SaveChangesToJson();
                }
            }
        }

        public static void SetTimeMessageID(ulong MessageID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.TimeZones.MsgID = MessageID;
                    PersistentData.SaveChangesToJson();
                }
            }
        }

        public static void SetConsoleDivider(ulong RoleID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.Consoles.DividerRoleID = RoleID;
                    PersistentData.SaveChangesToJson();
                }
            }
        }

        public static void SetTimeDivider(ulong RoleID, ulong GuildID)
        {
            foreach (var BotGuild in PersistentData.Data.Guilds)
            {
                if (BotGuild.GuildID == GuildID)
                {
                    BotGuild.SelfAssign.TimeZones.DividerRoleID = RoleID;
                    PersistentData.SaveChangesToJson();
                }
            }
        }
    }
}