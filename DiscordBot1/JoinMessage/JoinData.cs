using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.JoinMessage
{
    public class JoinData
    {
        public List<string> Messages { get; set; }
        public bool Greet { get; set; }

        public JoinData()
        {
            Messages = new List<string>();
            Greet = true;
            ResetToDefault();
        }

        private static readonly string Token = "~user~";
        private static readonly string UserInsert = "{0}";
        private static readonly Random RNGeesus = new Random();

        public async void OnUserJoined(SocketGuildUser User)
        {
            await User.Guild.DefaultChannel.SendMessageAsync(GetRandomFormattedMessage(User));
        }


        public string GetFormattedMessage(int ID, IUser User) => GetFormattedMessage(ID, (SocketGuildUser)User);
        public string GetFormattedMessage(int ID, SocketGuildUser User) => string.Format(GetMessage(ID), User.Nickname ?? User.Username);

        public string GetRandomFormattedMessage(IUser User) => GetRandomFormattedMessage((SocketGuildUser)User);
        public string GetRandomFormattedMessage(SocketGuildUser User) => string.Format(GetRandomMessage(), User.Nickname ?? User.Username);

        public string GetMessage(int ID)
        {
            return Messages[ID];
        }

        public int GetMessageID(string Message)
        {
            return Messages.IndexOf(Message);
        }

        public string GetRandomMessage() => GetMessage(RNGeesus.Next(0, Messages.Count - 1));

        public void RemoveMessage(int ID)
        {
            Messages.RemoveAt(ID);
            PersistentData.SaveChangesToJson();
        }

        public int AddMessage(string Message)
        {
            Messages.Add(Message);
            PersistentData.SaveChangesToJson();
            return GetMessageID(Message);
        }

        public List<Embed> GetMessages(IUser User) => GetMessages((SocketGuildUser)User);
        public List<Embed> GetMessages(SocketGuildUser User)
        {
            var Output = new List<Embed>();
            var Embed = new EmbedBuilder();

            Embed.WithDescription("The following messages are being used to greet new users.");

            foreach (var Message in Messages)
            {
                Embed.AddField($"Message ID: {GetMessageID(Message)}", $"Message: {string.Format(Message, User.Nickname ?? User.Username)}");

                if (Embed.Fields.Count >= 20)
                {
                    Output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
            }
            Output.Add(Embed.Build());
            return Output;
        }

        public string ParseMessage(string Input)
        {
            if (!Input.Contains(Token, StringComparison.OrdinalIgnoreCase))
                return Input;

            int Index = Input.IndexOf(Token);

            string Part1 = Input.Substring(0, Index);
            string Part2 = Input.Substring(Index + Token.Length);
            return ParseMessage(Part1 + UserInsert + Part2);
        }

        public void ResetToDefault()
        {
            Messages = new List<string>();
            AddMessage("Hmmmm. Smells like {0} in here!");
            AddMessage("Look what the cat dragged in... It's {0}!");
            AddMessage("Hey! Who let {0} in here?");
            AddMessage("Check it out! Even {0} is here!");
            AddMessage("I think this server is pretty awesome! What do you think {0}?");
            AddMessage("Come see {0} in tonights premium circus act!");
            AddMessage("Alright we can start the party now that {0} is here!");
        }
    }
}
