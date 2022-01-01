using Discord;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Comp
{
    public class Player
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string CurrentTeam { get; set; }
        public int TotalGames { get; set; }
        public int Total1V1 { get; set; }
        public int VersusWins { get; set; }
        public bool Active { get; set; }
        public Wins Wins { get; set; }
        public List<Match> Matches { get; private set; }
        public List<string> PreviousTeams { get; set; }

        public Player(GuildUser User, string TeamName)
        {
            Name = User.Name;
            Id = User.DiscordID;
            CurrentTeam = TeamName;
            Active = true;
            Wins = new Wins();
            Matches = new List<Match>();
            PreviousTeams = new List<string>();
        }

        public void AddMatch(Match match)
        {
            Matches.Add(match);

            if (match.Versus)
            {
                Award1v1(match);
                return;
            }

            GiveWins(Wins, match);
        }

        public void ChangeTeam(string NewTeam)
        {
            if (!PreviousTeams.Contains(CurrentTeam))
                PreviousTeams.Add(CurrentTeam);
            CurrentTeam = NewTeam;
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

        private void GiveWins(Wins wins, Match match)
        {
            TotalGames++;
            if (match.Placement == 1)
                wins.Gold++;
            if (match.Placement == 2)
                wins.Silver++;
            if (match.Placement == 3)
                wins.Bronze++;
            return;
        }

        private void Award1v1(Match match)
        {
            Total1V1++;
            VersusWins += match.Placement == 1 ? 1 : 0;
            return;
        }

    }
}
