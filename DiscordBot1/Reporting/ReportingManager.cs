using Discord;
using HeartFlame.GuildControl;
using System;
using Discord.WebSocket;
using HeartFlame.Misc;
using System.Collections.Generic;

namespace HeartFlame.Reporting
{
    public class ReportingManager
    {
        public static async void OnReactionAdded(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            SocketGuildUser user = (SocketGuildUser)Reaction.User;
            SocketGuild guild = user.Guild;

            if (user.IsBot)
                return;

            if (guild.Id == PersistentData.Data.Config.Reporting.GuildID)
                if (Reaction.MessageId == PersistentData.Data.Config.Reporting.MessageID)
                    if (Reaction.Emote.Name.Equals("ref"))
                        Utils.UpdateMessage(Channel, PersistentData.Data.Config.Reporting.MessageID, PrimaryReport());
                    
        }

        public static void OnReactionRemoved(Cacheable<IUserMessage, ulong> Cache, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            OnReactionAdded(Cache, Channel, Reaction);
        }
        public static async void RemovePrimaryReport()
        {
            var Client = Program.Client;
            var Guild = Client.GetGuild(PersistentData.Data.Config.Reporting.GuildID);
            var Chnls = new List<SocketGuildChannel>(Guild.Channels);
            var Channels = new List<ISocketMessageChannel>();
            foreach (var chan in Chnls)
                Channels.Add(chan as ISocketMessageChannel);

            var Channel = Channels.Find(x => x.GetMessageAsync(PersistentData.Data.Config.Reporting.MessageID) != null);
            var Message = Channel.GetMessageAsync(PersistentData.Data.Config.Reporting.MessageID);
            if (Message.Result is null) return;
            await Message.Result.DeleteAsync();
            
        }

        public static Embed PrimaryReport()
        {
            var Embed = new EmbedBuilder();
            Embed.WithDescription("Primary report");

            Embed.AddField("Total number of guilds", PersistentData.Data.Guilds.Count);
            Embed.AddField("Most popular guild", PopularGuild());
            Embed.AddField("Least popular guild", UnpopularGuild());
            var (User, Guild) = HighestChat();
            Embed.AddField("Most experience", 
                $"Guild Name: {Guild.Name}\n" +
                $"User Name: {User.Name}\n" +
                $"User Experience: {User.Chat.ChatExp}\n" +
                $"User Level: {User.Chat.ChatLevel}");
            Embed.AddField("Average user level", AverageLevel());
            return Embed.Build();
        }

        public static string PopularGuild()
        {
            int Qty = 0;
            string Name = "";
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.Users.Count > Qty)
                {
                    Qty = Guild.Users.Count;
                    Name = Guild.Name;
                }
            }
            return Name;
        }
        public static string UnpopularGuild()
        {
            int Qty = int.MaxValue;
            string Name = "";
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                if (Guild.Users.Count < Qty)
                {
                    Qty = Guild.Users.Count;
                    Name = Guild.Name;
                }
            }
            return Name;
        }

        public static (GuildUser User, GuildData Guild) HighestChat()
        {
            (GuildUser User, GuildData Guild) Output = (null, null);

            foreach(var Guild in PersistentData.Data.Guilds)
            {
                foreach(var User in Guild.Users)
                {
                    if(Output.User is null)
                    {
                        Output = (User, Guild);
                    }

                    if (User.Chat.ChatExp > Output.User.Chat.ChatExp)
                        Output = (User, Guild);
                    
                }
            }
            GuildManager.SetGlobalRank1(Output.User);
            return Output;
        }

        public static int AverageLevel()
        {
            float Total = 0;
            float Qty = 0;
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                foreach (var User in Guild.Users)
                {
                    Total += User.Chat.ChatLevel;
                    Qty++;
                }
            }

            return (int)MathF.Floor(Total / Qty);
        }
    }
}
