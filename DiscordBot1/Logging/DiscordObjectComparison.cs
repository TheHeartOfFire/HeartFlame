using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Logging
{
    public class DiscordObjectComparison
    {
        public static List<CompareResult> User(SocketGuildUser Before, SocketGuildUser After)
        {
            var Name = After.Username;
            if (!(After.Nickname is null))
                Name = After.Nickname;
            var Results = new List<CompareResult>();
            if (Before.Nickname != After.Nickname)
            {
                string Action = "updated";
                string StartName = Before.Nickname;
                if (Before.Nickname is null)
                {
                    Action = "set";
                    StartName = Before.Username;
                }
                Action += $" to { After.Nickname}";

                if (After.Nickname is null)
                    Action = "removed";


                Results.Add(new CompareResult("Nickname Update", $"{StartName}'s nickname has been {Action}"));
            }


            foreach(var Perm in ComparePermissions(Before, After))
            {
                string Action = "no longer";
                if (Perm.Gained)
                    Action = "now";

                Results.Add(new CompareResult("Permission Update", $"{Name} can {Action} {Perm.Permission}"));
            }

            return Results;
        }

        public static List<CompareResult> Role(SocketRole Before, SocketRole After)
        {
            var Results = new List<CompareResult>();

            if(Before.Name != After.Name)
                Results.Add(new CompareResult("Name Update", $"The {Before.Name} role's name has been changed to {After.Name}"));

            if(Before.Color != After.Color)
                Results.Add(new CompareResult("Color Update", $"The {After.Name} role's color has been changed to {After.Color}"));

            if(Before.IsHoisted != After.IsHoisted)
            {
                string Action = "no longer";
                if (After.IsHoisted)
                    Action = "now";

                Results.Add(new CompareResult("Hoisted Update", $"The {After.Name} role will {Action} be displayed separately from other roles"));

            }

            if (Before.IsMentionable != After.IsMentionable)
            {
                string Action = "no longer";
                if (After.IsMentionable)
                    Action = "now";

                Results.Add(new CompareResult("Mentionable Update", $"The {After.Name} role can {Action} be mentioned by all users"));

            }

            foreach (var Perm in ComparePermissions(Before.Permissions.ToList(), After.Permissions.ToList()))
            {
                string Action = "no longer";
                if (Perm.Gained)
                    Action = "now";

                Results.Add(new CompareResult("Permission Update", $"{After.Name} can {Action} {Perm.Permission}"));
            }

            return Results;
        }

        public static List<CompareResult> Channel(SocketGuildChannel Before, SocketGuildChannel After)
        {
            var Results = new List<CompareResult>();
            if(Before.Name != After.Name)
                Results.Add(new CompareResult("Name Update", $"The {Before.Name} channel's name has been changed to {After.Name}"));


            return Results;
        }

        public static List<CompareResult> Guild(SocketGuild Before, SocketGuild After)
        {
            var Results = new List<CompareResult>();
            if (Before.Name != After.Name)
                Results.Add(new CompareResult("Name Update", $"The {Before.Name} server's name has been changed to {After.Name}"));

            if (Before.DefaultChannel != After.DefaultChannel)
                Results.Add(new CompareResult("Default Channel Update", $"The {After.Name} server's Default Channel has been changed to {After.DefaultChannel.Name}"));

            if (Before.Description != After.Description)
                Results.Add(new CompareResult("Description Update", $"The {After.Name} server's description has been changed to {After.Description}"));

            var owner = After.Owner.Username;
            if (!(After.Owner.Nickname is null))
                owner = After.Owner.Nickname;

            if (Before.Owner.Id != After.Owner.Id)
                Results.Add(new CompareResult("Owner Update", $"The {After.Name} server's owner has been changed to {owner}"));

            if (Before.RulesChannel != After.RulesChannel)
                Results.Add(new CompareResult("Rules Channel Update", $"The {After.Name} server's Rules Channel has been changed to {After.RulesChannel.Name}"));

            if (Before.SystemChannel != After.SystemChannel)
                Results.Add(new CompareResult("Rules Channel Update", $"The {After.Name} server's System Channel has been changed to {After.SystemChannel.Name}"));

            return Results;
        }

        private static List<PermissionComparisonResult> ComparePermissions(SocketGuildUser Before, SocketGuildUser After)
        {
            return ComparePermissions(Before.GuildPermissions.ToList(), After.GuildPermissions.ToList());
        }

        private static List<PermissionComparisonResult> ComparePermissions(List<GuildPermission> Before, List<GuildPermission> After)
        {
            var Permissions = new List<PermissionComparisonResult>();

            //get losses
            foreach (var Perm in Before)
            {
                if (!After.Contains(Perm))
                    Permissions.Add(new PermissionComparisonResult(Perm, false));
            }

            //get gains
            foreach (var Perm in After)
            {
                if (!Before.Contains(Perm))
                    Permissions.Add(new PermissionComparisonResult(Perm, true));
            }

            return Permissions;
        }
    }
}
