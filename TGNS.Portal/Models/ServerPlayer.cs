using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGNS.Portal.Classes;

namespace TGNS.Portal.Models
{
    public interface IServerPlayer
    {
        ITeam Team { get; }
        string PlayerName { get; }
        long PlayerId { get; }
        bool IsMarine { get; }
        bool IsAlien { get; }
        bool IsSpectator { get; }
        bool IsReadyRoom { get; }
        bool IsBot { get; }
        int Score { get; }
        decimal Resources { get; }
        string IpAddress { get; }
        bool IsCommander { get; }
    }

    public class ServerPlayer : IServerPlayer
    {
        public ITeam Team { get; private set; }
        public string PlayerName { get; private set; }
        public bool IsBot { get; private set; }
        public int Score { get; private set; }
        public decimal Resources { get; private set; }
        public string IpAddress { get; private set; }
        public long PlayerId { get; private set; }

        public bool IsReadyRoom
        {
            get { return Team.Number == 0; }
        }

        public bool IsMarine
        {
            get { return Team.Number == 1; }
        }

        public bool IsAlien
        {
            get { return Team.Number == 2; }
        }

        public bool IsSpectator
        {
            get { return Team.Number == 3; }
        }

        public ServerPlayer(long playerId, string playerName, int teamNumber, bool isBot, int score, decimal resources, string ipAddress, bool isCommander)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            IsBot = isBot;
            Score = score;
            Resources = resources;
            IpAddress = ipAddress;
            Team = new Team(teamNumber);
            IsCommander = isCommander;
        }

        public bool IsCommander { get; private set; }
    }
}