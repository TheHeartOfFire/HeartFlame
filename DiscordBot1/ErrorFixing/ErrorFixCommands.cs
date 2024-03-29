﻿using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Permissions;
using HeartFlame.SelfAssign;
using HeartFlame.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ErrorFixing
{
    [Group("Error")]
    [RequirePermission(Roles.CREATOR)]
    public class ErrorFixCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Beta")]
        public async Task Beta()
        {
            foreach (var _User in GuildManager.GetAllUsers())
            {
                _User.Banner.Badges.Global.BetaTester = true;
            }
            PersistentData.SaveChangesToJson();

            await ReplyAsync($"All users have bee set as beta testers.");
        }

        [Command("Badges")]
        public async Task Badges(SocketGuildUser User = null)
        {
            bool Beta = false;
            bool Rank1 = false;
            bool Patreon = false;
            var GUser = GuildManager.GetUser(User is null ? Context.User : User);

            var Profiles = GuildManager.GetGlobalUser(GUser);

            foreach (var Profile in Profiles)
            {
                var Badges = Profile.Banner.Badges.Global;
                if (Badges.BetaTester)
                    Beta = true;
                if (Badges.Patreon)
                    Patreon = true;
                if (Badges.Rank1)
                    Rank1 = true;
            }

            foreach (var Profile in Profiles)
            {
                var Badges = Profile.Banner.Badges.Global;
                Badges.BetaTester = Beta;
                Badges.Rank1 = Rank1;
                Badges.Patreon = Patreon;
            }

            PersistentData.SaveChangesToJson();

            await ReplyAsync($"{GUser.Name}'s various profiles have had their badge data updated.");
        }

        [Command("Time")]
        public async Task Time()
        {
            var Guild = GuildManager.GetGuild(Context.Guild);

            await Context.Guild.DownloadUsersAsync();

            foreach (var User in Context.Guild.Users)
            {
                foreach (var Role in User.Roles)
                {
                    if (Role.Name.Equals("US Pacific (-7)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("pst")).RoleID));
                    }
                    if (Role.Name.Equals("US Mountain (-6)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("mst")).RoleID));
                    }
                    if (Role.Name.Equals("US Central (-5)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("cst")).RoleID));
                    }
                    if (Role.Name.Equals("US Eastern (-4)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("est")).RoleID));
                    }
                    if (Role.Name.Equals("Greenwich (+0)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("gmt")).RoleID));
                    }
                    if (Role.Name.Equals("UK (+1)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("west")).RoleID));
                    }
                    if (Role.Name.Equals("Australia (+10)"))
                    {
                        await User.RemoveRoleAsync(Role);
                        await User.AddRoleAsync(Context.Guild.GetRole(Guild.SelfAssign.TimeZones.GetRole(TimeManager.GetTimezone("aus")).RoleID));
                    }
                }
            }
        }

        [Command("Time2")]
        public async Task Time2()
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.ModuleControl.IncludeSelfAssign && Guild.ModuleControl.IncludeTime)
                {
                    if (Guild.SelfAssign.TimeZones.Roles.Count > 0)
                    {
                        var NewRoles = new List<TimeZoneRole>();

                        foreach (var Role in Guild.SelfAssign.TimeZones.Roles)
                        {
                            NewRoles.Add(new TimeZoneRole
                            {
                                Emoji = Role.Emoji,
                                Name = Role.Name,
                                Position = Role.Position,
                                RoleID = Role.RoleID,
                                TimeZoneID = TimeManager.GetTimezone(Role.Name).Id
                            });
                        }

                        Guild.SelfAssign.TimeZones.Roles = new List<RoleObject>();
                        foreach (var Role in NewRoles)
                        {
                            Guild.SelfAssign.TimeZones.AddRole(Role);
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
            await ReplyAsync("Error fix implimented");
        }
        [Command("JoinCustomInit")]
        public async Task JoinCustomInit()
        { 
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.Commands is null)
                    Guild.Commands = new CustomCommands.ResponseData();
                if (Guild.Join is null)
                    Guild.Join = new JoinMessage.JoinData();
            }
            PersistentData.SaveChangesToJson();

            await ReplyAsync("Join message and Custom command modules initialized for non-null cases.");
        }

        [Command("PatchInit")]
        public async Task PatchInit()
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.PatchNotes is null)
                    Guild.PatchNotes = new PatchNotes.PatchNotesData();
            }
            PersistentData.SaveChangesToJson();

            await ReplyAsync("Patch notes module initialized for non-null cases.");
        }
    }
}
