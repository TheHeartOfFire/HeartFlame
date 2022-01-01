using Discord;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Comp
{
    public class CompTeam
    {
        public string Name { get; set; }
        public string Game { get; set; }
        public List<Player> Players { get; set; }
        public Wins Wins { get; set; }
        public int TotalGames { get; set; }
        public List<Match> Matches { get; set; }

        public CompTeam(string Name, string Game)
        {
            this.Name = Name;
            this.Game = Game;
            Players = new List<Player>();
            Wins = new Wins();
            Matches = new List<Match>();
        }

        public bool AddPlayer(GuildUser User)//TODO: Add Player
        {
            return false;
        }

        public Player GetPlayer(GuildUser User)
        {
            return Players.Find(Player => Player.Id == User.DiscordID);
        }

        public int ActivePlayers()
        {
            int Counter = 0;
            foreach(var Player in Players)
            {
                if(Player.Active) Counter++;
            }
            return Counter;
        }

        public int InactivePlayers()
        {
            int Counter = 0;
            foreach (var Player in Players)
            {
                if (!Player.Active) Counter++;
            }
            return Counter;
        }

        public void AddMatch(Match match)
        {
            Matches.Add(match);
            TotalGames++;
            GiveWins(Wins, match.Placement);
        }

        private int GiveWins(Wins wins, int Placement)
        {
            if (Placement == 1)
                wins.Gold++;
            if (Placement == 2)
                wins.Silver++;
            if (Placement == 3)
                wins.Bronze++;
            return -1;
        }

        public Embed ListPlayers()
        {
            var Embed = new EmbedBuilder();


            foreach (var player in Players)
            {
                Embed.AddField($"{player.Name}", 
                    $"Games Played: {player.TotalGames}" +
                    $"1st Place Wins: {player.Wins.Gold}\n" +
                    $"2nd Place Wins: {player.Wins.Silver}\n" +
                    $"3rd Place Wins: {player.Wins.Bronze}");
            }

            return Embed.Build();
        }

        public Embed ListWins()
        {
            var Embed = new EmbedBuilder();

            Embed.AddField($"Games Played: {TotalGames}",
                $"1st Place Wins: {Wins.Gold}\n" +
                $"2nd Place Wins: {Wins.Silver}\n" +
                $"3rd Place Wins: {Wins.Bronze}");
            
            return Embed.Build();
        }
    }
}
