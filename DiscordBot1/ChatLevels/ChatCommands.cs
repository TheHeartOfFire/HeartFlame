using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChatLevels
{
    [Group("Level"), Alias("lv", "lvl")]
    [RequireModule(Modules.CHAT)]
    public partial class ChatCommands : ModuleBase<SocketCommandContext>
    {
        [Command(""), Summary("Get the user's current chat level. Optionally mention another user to get their level.")]
        public async Task GetLevel(SocketGuildUser User = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            var DisUser = (SocketGuildUser)Context.User;
            if (User != null)
                DisUser = User;

            var img = await BannerMaker.BuildBannerAsync(DisUser, false);

            await GuildManager.GetChatChannel(Context, BotGuild).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
        }

        [Command("Ranking"), Alias("Top", "Leaders", "LeaderBoard", "Leader Board", "rank", "r"), Summary("Get the top 10 chat levels.")]
        public async Task GetRankings()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            await BotGuild.GetChatChannel(Context).SendMessageAsync("", false, LevelManagement.Top10(Context.Guild.Id));
        }

        [Command("Help"), Alias("h", "?"), Summary("Get all of the commands in the Permissions Group"), Remarks("ChatCommandHelp"), Priority(1)]
        public async Task ChatCommandsHelp()
        {
            var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Help", "ChatCommandHelp", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        public static bool BadColor(string Hex)
        {
            if (Hex.Length != 6 && !OnlyHexInString(Hex))
                return true;
            return false;
        }

        public static bool BadColor(uint R, uint B, uint G)
        {
            if (R > 255 || B > 255 || G > 255)
                return true;
            return false;
        }

        

        public static bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static List<Embed> GetColorNames()
        {
            var output = new List<Embed>();

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] ColorInfo = colorType.GetProperties();

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(PersistentData.BotName);
            Embed.WithTitle("The following color names are recognized.");

            foreach (var color in ColorInfo)
            {
                Embed.AddField($"Name: {color.Name}", $"Value: {color}",true);
                if (Embed.Fields.Count >= 20)
                {
                    output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
            }

            return output;
        }
    }
}