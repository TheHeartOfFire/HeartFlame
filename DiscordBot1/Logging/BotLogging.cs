using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using System.Reflection;
using System.Linq;
using HeartFlame.Misc;
using System.Runtime.CompilerServices;
using System;

namespace HeartFlame.Logging
{
    public class BotLogging
    {
        public static void PrintLogMessage(Type Class, string Message, ulong GuildID, SocketGuildUser User, [CallerMemberName] string MethodName = "")
        {
            PrintLogMessage(Class, Message, GuildID, (SocketUser)User, MethodName);
        }

        public static void PrintLogMessage(Type Class, string Message, SocketCommandContext Context, [CallerMemberName] string MethodName = "")
        {
            PrintLogMessage(Class, Message, Context.Guild.Id, Context.User, MethodName);
        }

        public static void PrintLogMessage(Type Class, string Message, ulong GuildID, SocketUser User, [CallerMemberName]string MethodName = "")
        {
            var Attributes = Class.GetMethod(MethodName).GetCustomAttributesData();

            var Attribute = Attributes.First(x => x.AttributeType == typeof(SummaryAttribute));
            var Action = Attribute.ConstructorArguments[0].Value.ToString();

            PrintLogMessage(MethodName, Action, Message, GuildID, GuildManager.GetUser(User));
        }

        public static void PrintLogMessage(string Action, string Message, ulong GuildID, SocketUser User, [CallerMemberName] string MethodName = "")
        {
            PrintLogMessage(MethodName, Action, Message, GuildID, GuildManager.GetUser(User));
        }
        public static void PrintLogMessage(string Action, string Message, ulong GuildID, SocketGuildUser User, [CallerMemberName] string MethodName = "")
        {
            PrintLogMessage(MethodName, Action, Message, GuildID, GuildManager.GetUser(User));
        }

        public static void PrintLogMessage(string Action, string Message, SocketCommandContext Context, [CallerMemberName] string MethodName = "")
        {
            PrintLogMessage(MethodName, Action, Message, Context.Guild.Id, GuildManager.GetUser(Context.User));
        }

        public static async void PrintLogMessage(string Source, string Action, string Message, ulong GuildID, GuildUser User = null)
        {
            foreach(var Guild in PersistentData.Data.Guilds)
            {
                if(Guild.GuildID == GuildID)
                {
                    EmbedBuilder Embed = new EmbedBuilder
                    {
                        Color = Color.DarkRed
                    };
                    Embed.WithAuthor("Log");
                    Embed.WithDescription($"Source: {Source}\n\nAction: {Action}");
                    string Actor = PersistentData.BotName;
                    if (User != null)
                        Actor = User.Name;
                    Embed.AddField(Message, $"This action was taken by: {Actor}");

                    var ID = Guild.Configuration.LogChannel;

                    if(ID > 0)
                    await (Program.Client.GetChannel(ID) as ISocketMessageChannel).SendMessageAsync("", false, Embed.Build());
                    
                }
            }
            
            
        }

        public static string GetMethodInfo(MethodBase Method, bool Params = true)
        {
            var output = Method.ReflectedType.FullName + "." + Method.Name;
            output = output.Replace('+', '.');
            if (Params)
                return output;

            output += '(';
            foreach (var Param in Method.GetParameters())
                output += Param.ParameterType.Name + ' ' + Param.Name + ", ";

            output = output.Substring(0, output.Length - 2) + ")";
            return output;
        }
    }
}