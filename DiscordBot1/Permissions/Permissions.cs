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
                    Guild.Permissions.Mods.Add(new Permissions_User
                    {
                        Name = User.Username,
                        ID = User.Id
                    });

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
                        Guild.Permissions.Mods.Add(new Permissions_User
                        {
                            Name = User.Username,
                            ID = User.Id
                        });
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
                    Guild.Permissions.Admins.Add(new Permissions_User
                    {
                        Name = User.Username,
                        ID = User.Id
                    });

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
                        Guild.Permissions.Admins.Add(new Permissions_User
                        {
                            Name = User.Username,
                            ID = User.Id
                        });
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
                    foreach (var user in Guild.Permissions.Mods)
                    {
                        if (User.Id == user.ID)
                            Guild.Permissions.Mods.Remove(user);
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
                        if (IsMod(User))
                        {
                            foreach (var user in Guild.Permissions.Mods)
                            {
                                if (User.Id == user.ID)
                                    Guild.Permissions.Mods.Remove(user);
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
                    foreach (var user in Guild.Permissions.Admins)
                    {
                        if (User.Id == user.ID)
                            Guild.Permissions.Admins.Remove(user);
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
                        if (IsMod(User))
                        {
                            foreach (var user in Guild.Permissions.Admins)
                            {
                                if (User.Id == user.ID)
                                    Guild.Permissions.Admins.Remove(user);
                            }
                        }
                    }

                }
            }
            GuildManager.SaveChangesToJson();
        }

        public static bool IsMod(SocketGuildUser User)
        {
            if (User is null) return false;

            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var user in Guild.Permissions.Mods)
                    {
                        if (user.ID == User.Id)
                            return true;
                    }
                    foreach (var user in Guild.Permissions.Admins)
                    {
                        if (user.ID == User.Id)
                            return true;
                    }

                }
            }

            return false;
        }

        public static bool IsAdmin(SocketGuildUser User)
        {
            if (User is null) return false;

            foreach (var Guild in GuildManager.Guilds)
            {
                if (Guild.GuildID == User.Guild.Id)
                {
                    foreach (var user in Guild.Permissions.Admins)
                    {
                        if (user.ID == User.Id)
                            return true;
                    }

                }
            }

            return false;
        }
    }
}