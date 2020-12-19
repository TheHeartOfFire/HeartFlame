using Discord.WebSocket;
using HeartFlame.GuildControl;
using System.Collections.Generic;

namespace HeartFlame.Permissions
{
    public class Permissions
    {
        public static void AddMod(SocketGuildUser User)
        {
            foreach(var Guild in GuildManager.Guilds)
            {
                if(Guild.GuildID == User.Guild.Id)
                {
                    foreach(var GUser in Guild.Users)
                    {
                        if(GUser.DiscordID == User.Id)
                        {
                            GUser.Mod = true;
                            GUser.Admin = false;
                        }
                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void AddMod(List<SocketGuildUser> Users)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach(var GUser in Guild.Users)
                        {
                            if(GUser.DiscordID == User.Id)
                            {
                                GUser.Mod = true;
                                GUser.Admin = false;
                            }
                        }
                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void AddAdmin(SocketGuildUser User)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Admin = true;
                            GUser.Mod = false;
                        }
                    }

                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void AddAdmin(List<SocketGuildUser> Users)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach (var GUser in Guild.Users)
                        {
                            if (GUser.DiscordID == User.Id)
                            {
                                GUser.Mod = true;
                                GUser.Admin = false;
                            }
                        }
                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void RemoveMod(SocketGuildUser User)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Mod = false;
                        }
                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void RemoveMod(List<SocketGuildUser> Users)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach(var GUser in Guild.Users)
                        {
                            if(GUser.DiscordID == User.Id)
                            {
                                if (GUser.isMod())
                                {
                                    GUser.Mod = false;
                                }
                            }
                        }
                    }

                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void RemoveAdmin(SocketGuildUser User)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var GUser in Guild.Users)
                    {
                        if (GUser.DiscordID == User.Id)
                        {
                            GUser.Admin = false;
                        }
                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static void RemoveAdmin(List<SocketGuildUser> Users)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == Users[0].Guild.Id)
                {
                    foreach (var User in Users)
                    {
                        foreach (var GUser in Guild.Users)
                        {
                            if (GUser.DiscordID == User.Id)
                            {
                                if (GUser.isMod())
                                {
                                    GUser.Admin = false;
                                }
                            }
                        }
                    }

                }
            }
            GuildManager.SaveChangesToJson();
        }

    }
}