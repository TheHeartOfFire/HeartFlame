using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System.Collections.Generic;

namespace HeartFlame.Permissions
{
    public class Permissions
    {
        public static void AddMod(SocketGuildUser User)
        {
            var GUser = GuildManager.GetUser(User);

            GUser.Perms.Mod = true;
            GUser.Perms.Admin = false;
            PersistentData.SaveChangesToJson();
        }

        public static void AddMod(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                var GUser = GuildManager.GetUser(User);

                GUser.Perms.Mod = true;
                GUser.Perms.Admin = false;
            }
            PersistentData.SaveChangesToJson();
        }

        public static void AddAdmin(SocketGuildUser User)
        {
            var GUser = GuildManager.GetUser(User);
            GUser.Perms.Admin = true;
            GUser.Perms.Mod = false;
            PersistentData.SaveChangesToJson();
        }

        public static void AddAdmin(List<SocketGuildUser> Users)
        {

            foreach (var User in Users)
            {
                var GUser = GuildManager.GetUser(User);

                GUser.Perms.Mod = true;
                GUser.Perms.Admin = false;
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveMod(SocketGuildUser User)
        {
            GuildManager.GetUser(User).Perms.Mod = false;
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveMod(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                var GUser = GuildManager.GetUser(User);
                if (GUser.Perms.Mod)
                {
                    GUser.Perms.Mod = false;
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveAdmin(SocketGuildUser User)
        {
            GuildManager.GetUser(User).Perms.Admin = false;
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveAdmin(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                var GUser = GuildManager.GetUser(User);
                if (GUser.Perms.Mod)
                {
                    GUser.Perms.Admin = false;
                }
            }
            PersistentData.SaveChangesToJson();
        }

    }
}