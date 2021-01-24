using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame
{
    public class Utils
    {
        public static async void UpdateMessage(ISocketMessageChannel Channel, ulong MsgID, Embed Embed, bool RemoveText = false)
        {
            var msg = Channel.GetMessageAsync(MsgID);
            var Message = ((IUserMessage)msg.Result);
            if (RemoveText)
                await Message.ModifyAsync(x => x.Content = "");

            await Message.ModifyAsync(x => x.Embed = Embed);
        }
    }
}
