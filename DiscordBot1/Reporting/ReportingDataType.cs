using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Reporting
{
    public class ReportingDataType
    {
        public ulong MessageID { get; set; }
        public ulong GuildID { get; set; }
        public ulong ErrorChannel { get; set; }

        public ISocketMessageChannel GlobalErrorChannel()
        {
            var Client = Program.Client;
            return (SocketGuildChannel)Client.GetChannel(ErrorChannel) as ISocketMessageChannel;
        }
    }
}
