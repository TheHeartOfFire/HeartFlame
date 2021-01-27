﻿using Discord;
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
using Discord.Rest;

namespace HeartFlame.SelfAssign
{
    public class SelfAssign
    {
        private static readonly GuildPermissions NoPerms = new GuildPermissions();


        public static Embed PrefabConsoleAsync(SocketGuild Guild)
        {
            var Module = GuildManager.GetGuild(Guild).SelfAssign.Consoles;

            Module = new RoleCategory()
            {
                Roles = new List<RoleObject>(),
                Name = "Consoles",
                Title = "Consoles"
            };

            Module.SetDivider(CreateDivider(Guild, "Console").Result);

            Module.AddRole("Xbox", 
                1,
                AddRoleIfNotExist(Guild, "Xbox", Color.Green).Result, 
                EmoteRef.Emotes.GetValueOrDefault("Xbox"));
            
            Module.AddRole("PlayStation", 
                2,
                AddRoleIfNotExist(Guild, "PlayStation", Color.Blue).Result,
                EmoteRef.Emotes.GetValueOrDefault("PlayStation"));

            Module.AddRole("PC", 
                3,
                AddRoleIfNotExist(Guild, "PC").Result,
                EmoteRef.Emotes.GetValueOrDefault("PC"));

            Module.AddRole("Nintendo", 
                4,
                AddRoleIfNotExist(Guild, "Nintendo", Color.Red).Result,
                EmoteRef.Emotes.GetValueOrDefault("Nintendo"));

            return GenerateEmbed(Module, "the consoles you use");
        }

        public static Embed PrefabTimeAsync(SocketGuild Guild)
        {
            var Module = GuildManager.GetGuild(Guild).SelfAssign.TimeZones;


            Module = new RoleCategory()
            {
                Roles = new List<RoleObject>(),
                Name = "TimeZones",
                Title = "TimeZones"
            };

            Module.SetDivider(CreateDivider(Guild, "TimeZone").Result);

            Module.AddRole("US Pacific (-7)", 
                1,
                AddRoleIfNotExist(Guild, "US Pacific (-7)").Result,
                EmoteRef.Emotes.GetValueOrDefault("1"));

            Module.AddRole("US Mountain (-6)",
                2,
                AddRoleIfNotExist(Guild, "US Mountain (-6)").Result,
                EmoteRef.Emotes.GetValueOrDefault("2"));

            Module.AddRole("US Central (-5)", 
                3,
                AddRoleIfNotExist(Guild, "US Central (-5)").Result,
                EmoteRef.Emotes.GetValueOrDefault("3"));

            Module.AddRole("US Eastern (-4)", 
                4,
                AddRoleIfNotExist(Guild, "US Eastern (-4)").Result,
                EmoteRef.Emotes.GetValueOrDefault("4"));

            Module.AddRole("Greenwich (+0)", 
                5,
                AddRoleIfNotExist(Guild, "US Eastern (-4)").Result,
                EmoteRef.Emotes.GetValueOrDefault("5"));

            Module.AddRole("UK (+1)", 
                6,
                AddRoleIfNotExist(Guild, "UK (+1)").Result,
                EmoteRef.Emotes.GetValueOrDefault("6"));

            Module.AddRole("Australia (+10)",
                7,
                AddRoleIfNotExist(Guild, "Australia (+10)").Result,
                EmoteRef.Emotes.GetValueOrDefault("7"));

            return GenerateEmbed(Module, "your timezone");
        }

        public static Embed CustomModule(List<SocketRole> Roles, RoleCategory Module, string Description = "the role you want")
        {
            int pos = 0;
            foreach (var Role in Roles)
                Module.AddRole(Role.Name, pos, Role.Id, EmoteRef.Emotes.GetValueOrDefault(pos++.ToString()));

            return GenerateEmbed(Module, Description);
        }

        public static Embed GenerateEmbed(RoleCategory Module, string Description)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithDescription($"Please select the reaction corresponding to {Description}. Removing your reaction will remove the role from you.");

            foreach (var role in Module.Roles)
            {
                Embed.AddField($"{role.Emoji} for the {role.Name} role.", ".");
            }
            Embed.WithFooter("");//TODOL: Additional command info
            return Embed.Build();
        }

        public static async Task<ulong> AddRoleIfNotExist(SocketGuild Guild, string Name, Color? Color = null, bool Mentionable = true)
        {
            if (!Color.HasValue)
                Color = Discord.Color.Default;

            var GuildRoles = new List<SocketRole>(Guild.Roles);

            ulong ID = 0;

            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == Name))
            {
                var Role = await Guild.CreateRoleAsync(Name, NoPerms, Color.Value, false, Mentionable);
                await Role.ModifyAsync(x => x.Position = 0);
                ID = Role.Id;
            }
            else
                ID = GuildRoles.FirstOrDefault(x => x.Name == Name).Id;


            return ID;
        }

        public static async Task<ulong> CreateDivider(SocketGuild Guild, string Name)
        {
            var GuildRoles = new List<SocketRole>(Guild.Roles);

            ulong ID = 0;

            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == Name))
            {
                var Role = await Guild.CreateRoleAsync($"⁣⁣ 	  	  	  	  	 ☚{Name}☛ 	  	  	  	  	 ⁣", NoPerms, new Color(0x2f3136), false, false);
                await Role.ModifyAsync(x => x.Position = 0);
                ID = Role.Id;
            }
            else
                ID = GuildRoles.FirstOrDefault(x => x.Name == Name).Id;


            return ID;
        }
    }
}