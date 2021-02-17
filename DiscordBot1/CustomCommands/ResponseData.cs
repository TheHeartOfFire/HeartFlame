using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.CustomCommands
{
    public class ResponseData
    {
        public Dictionary<string, string> Commands { get; set; }

        public ResponseData()
        {
            Commands = new Dictionary<string, string>();
        }
        /// <summary>
        /// True if successful
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool AddCommand(string Name, string Message)
        {
            if (CommandExists(Name)) return false;

            foreach (var Command in Program.Commands.Commands)
            {
                if (Name.Equals(Command.Name, StringComparison.OrdinalIgnoreCase)) return false;
                foreach (var Alias in Command.Aliases)
                    if (Name.Equals(Alias, StringComparison.OrdinalIgnoreCase)) return false;
            }

            Commands.Add(Name, Message);
            PersistentData.SaveChangesToJson();
            return true;
        }

        /// <summary>
        /// True if successful
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool RemoveCommand(string Name)
        {
            if (!CommandExists(Name)) return false;

            Commands.Remove(Name);
            PersistentData.SaveChangesToJson();
            return true;
        }
        /// <summary>
        /// True if successful
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool UpdateCommand(string Name, string Message)
        {
            if (!CommandExists(Name)) return false;

            Commands[Name] = Message;
            PersistentData.SaveChangesToJson();
            return true;
        }

        public string GetCommand(string Name)
        {
            if (!CommandExists(Name)) return string.Empty;
            return Commands[Name];
        }

        public bool CommandExists(string Name)
        => Commands.ContainsKey(Name);

        public List<Embed> GetAllCommands()
        {
            var Output = new List<Embed>();
            var Embed = new EmbedBuilder();

            Embed.WithDescription("The following custom commands have been created for this server.");

            foreach (var Command in Commands)
            {
                Embed.AddField($"Command Name: {Command.Key}", $"Message: {Command.Value}");

                if (Embed.Fields.Count >= 20)
                {
                    Output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
            }
            Output.Add(Embed.Build());
            return Output;
        }

        public async Task Client_MessageReceived(SocketMessage Message, int argpos)
        {
            string message = Message.Content;
            message = message[argpos..];

            if (CommandExists(message))
                await Message.Channel.SendMessageAsync(GetCommand(message));
        }
    }
}
