using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Comp
{
    public class CompData
    {
        public List<CompTeam> Teams { get; set; }

        public CompData()
        {
            Teams = new List<CompTeam>();
        }

        public bool AddTeam(string TeamName, string Game)//TODO: Add Team
        {
            Teams.Add(new CompTeam(TeamName, Game));
            return false;
        }

        public CompTeam GetTeam(string TeamName)
        {
            return Teams.Find(Team => Team.Name.Equals(TeamName, StringComparison.OrdinalIgnoreCase));
        }

        public Embed ListTeams()
        {
            var Embed = new EmbedBuilder();
            

            foreach(var Team in Teams)
            {
                Embed.AddField($"{Team.Name}", $"Active Players: {Team.ActivePlayers()}\n" +
                    $"Inactive Players: {Team.InactivePlayers()}\n" +
                    $"Games Played: {Team.TotalGames}" +
                    $"1st Place Wins: {Team.Wins.Gold}\n" +
                    $"2nd Place Wins: {Team.Wins.Silver}\n" +
                    $"3rd Place Wins: {Team.Wins.Bronze}");
            }

            return Embed.Build();
        }
    }
}
