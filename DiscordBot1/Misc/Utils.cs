using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame
{
    public class Utils
    {
        public static async void UpdateMessage(ISocketMessageChannel Channel, ulong MsgID, Embed Embed, bool RemoveText = false)
        {
            var msg = Channel.GetMessageAsync(MsgID);
            var Message = ((IUserMessage)msg.Result);
            if (RemoveText)
                await Message.ModifyAsync(x => x.Content = ".");

            await Message.ModifyAsync(x => x.Embed = Embed);
        }

        public static async void UpdateMessage(ISocketMessageChannel Channel, ulong MsgID, string Text, bool RemoveText = false)
        {
            var msg = Channel.GetMessageAsync(MsgID);
            var Message = ((IUserMessage)msg.Result);
            if (RemoveText)
                await Message.ModifyAsync(x => x.Content = "");

            await Message.ModifyAsync(x => x.Content = Text);
        }

        public static async void UpdateMessage(SocketGuild Guild, ulong MsgID, string Text, bool RemoveText = false)
        {
            Task<IMessage> msg = null;

            foreach(var Channel in Guild.TextChannels)
            {
                if (!(Channel.GetMessageAsync(MsgID) is null))
                    msg = Channel.GetMessageAsync(MsgID);
            }

            if (!(msg is null))
                return;

            var Message = ((IUserMessage)msg.Result);
            if (RemoveText)
                await Message.ModifyAsync(x => x.Content = "");

            await Message.ModifyAsync(x => x.Content = Text);
        }

        public static async Task<IUserMessage> UpdateMessage(SocketGuild Guild, ulong MsgID, Embed Embed, bool RemoveText = false)
        {
            IMessage msg = null;

            foreach (var Channel in Guild.TextChannels)
            {
                var TestCase = await Channel.GetMessageAsync(MsgID);

                if (!(TestCase is null))
                    msg = TestCase;
            }

            if (msg is null)
                return null;

            var Message = ((IUserMessage)msg);
            if (RemoveText)
                await Message.ModifyAsync(x => x.Content = "");

            await Message.ModifyAsync(x => x.Embed = Embed);

            return Message;
        }

        public static int GetClosest(List<int> Candidates, int Comparison)
        {
            int Winner = Candidates[0];
            foreach(var Candidate in Candidates)
                if (Math.Abs(Comparison - Candidate) < Math.Abs(Comparison - Winner))
                    Winner = Candidate;

            return Winner;
        }
    }
}
