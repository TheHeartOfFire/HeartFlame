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
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if(Guild.GuildID == User.Guild.Id)
                {
                    foreach(var GUser in Guild.Users)
                    {
                        if(GUser.DiscordID == User.Id)
                        {
                            GUser.Perms.Mod = true;
                            GUser.Perms.Admin = false;
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void AddMod(List<SocketGuildUser> Users)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach(var GUser in Guild.Users)
                        {
                            if(GUser.DiscordID == User.Id)
                            {
                                GUser.Perms.Mod = true;
                                GUser.Perms.Admin = false;
                            }
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void AddAdmin(SocketGuildUser User)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Perms.Admin = true;
                            GUser.Perms.Mod = false;
                        }
                    }

                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void AddAdmin(List<SocketGuildUser> Users)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach (var GUser in Guild.Users)
                        {
                            if (GUser.DiscordID == User.Id)
                            {
                                GUser.Perms.Mod = true;
                                GUser.Perms.Admin = false;
                            }
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveMod(SocketGuildUser User)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Perms.Mod = false;
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveMod(List<SocketGuildUser> Users)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach(var GUser in Guild.Users)
                        {
                            if(GUser.DiscordID == User.Id)
                            {
                                if (GUser.Perms.Mod)
                                {
                                    GUser.Perms.Mod = false;
                                }
                            }
                        }
                    }

                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveAdmin(SocketGuildUser User)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Perms.Admin = false;
                        }
                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }

        public static void RemoveAdmin(List<SocketGuildUser> Users)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach (var GUser in Guild.Users)
                        {
                            if (GUser.DiscordID == User.Id)
                            {
                                if (GUser.Perms.Mod)
                                {
                                    GUser.Perms.Admin = false;
                                }
                            }
                        }
                    }

                }
            }
            PersistentData.SaveChangesToJson();
        }

    }
}