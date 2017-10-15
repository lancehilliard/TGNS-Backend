using System;

namespace TGNS.Core.Domain
{
    public interface IPlayedGame
    {
        string ServerName { get; }
        double StartTimeSeconds { get; }
        string Realm { get; }
        string GameMode { get; }
        double DurationInSeconds { get; }
        int WinningTeamNumber { get; }
        int SkulksKilled { get; }
        int GorgesKilled { get; }
        int LerksKilled { get; }
        int FadesKilled { get; }
        int OnosKilled { get; }
        int RifleMarinesKilled { get; }
        int JetpackMarinesKilled { get; }
        int ClawMinigunMarinesKilled { get; }
        int ClawRailgunMarinesKilled { get; }
        int MinigunMinigunMarinesKilled { get; }
        int RailgunRailgunMarinesKilled { get; }
        int ShotgunMarinesKilled { get; }
        int FlamethrowerMarinesKilled { get; }
        int GrenadeLauncherMarinesKilled { get; }
        double MarineTeamResourcesTotal { get; }
        double AlienTeamResourcesTotal { get; }
        int HivesKilled { get; }
        int ChairsKilled { get; }
        int TotalPlayerCount { get; }
        int FullGameStrangerCount { get; }
        int FullGamePrimerWithGamesCount { get; }
        int FullGameSupportingMemberCount { get; }
        bool CaptainsMode { get; }
        bool IncludedBots { get; }
        int BuildNumber { get; }
        string MarineStartLocationName { get; }
        string AlienStartLocationName { get; }
        string MapName { get; }
        double? StartingLocationsPathDistance { get; }
        int? SurrenderTeamNumber { get; }
        int? WinOrLoseEndGameCountdownValue { get; }
        long PlayerId { get; }
        double MarineSeconds { get; }
        double AlienSeconds { get; }
        double CommanderSeconds { get; }
        bool EndGameCommander { get; }
        bool Captain { get; }
        int Score { get; }
        int Kills { get; }
        int Assists { get; }
        int Deaths { get; }
        bool PlayerIsSupportingMember { get; }
        bool PlayerIsPrimerSignerWithGames { get; }
        bool PlayerIsStranger { get; }
        double WeldGave { get; }
        double HealSprayGave { get; }
        DateTime Created { get; }
        DateTime LastModified { get; }
    }

    public class PlayedGame : IPlayedGame
    {
        public PlayedGame(string serverName, double startTimeSeconds, string realm, string gameMode, double durationInSeconds, int winningTeamNumber, int skulksKilled, int gorgesKilled, int lerksKilled, int fadesKilled, int onosKilled, int rifleMarinesKilled, int jetpackMarinesKilled, int clawMinigunMarinesKilled, int clawRailgunMarinesKilled, int minigunMinigunMarinesKilled, int railgunRailgunMarinesKilled, int shotgunMarinesKilled, int flamethrowerMarinesKilled, int grenadeLauncherMarinesKilled, double marineTeamResourcesTotal, double alienTeamResourcesTotal, int hivesKilled, int chairsKilled, int totalPlayerCount, int fullGameStrangerCount, int fullGamePrimerWithGamesCount, int fullGameSupportingMemberCount, bool captainsMode, bool includedBots, int buildNumber, string marineStartLocationName, string alienStartLocationName, string mapName, double? startingLocationsPathDistance, int? surrenderTeamNumber, int? winOrLoseEndGameCountdownValue, long playerId, double marineSeconds, double alienSeconds, double commanderSeconds, bool endGameCommander, bool captain, int score, int kills, int assists, int deaths, bool playerIsSupportingMember, bool playerIsPrimerSignerWithGames, bool playerIsStranger, double weldGave, double healSprayGave, DateTime created, DateTime lastModified)
        {
            ServerName = serverName;
            StartTimeSeconds = startTimeSeconds;
            Realm = realm;
            GameMode = gameMode;
            DurationInSeconds = durationInSeconds;
            WinningTeamNumber = winningTeamNumber;
            SkulksKilled = skulksKilled;
            GorgesKilled = gorgesKilled;
            LerksKilled = lerksKilled;
            FadesKilled = fadesKilled;
            OnosKilled = onosKilled;
            RifleMarinesKilled = rifleMarinesKilled;
            JetpackMarinesKilled = jetpackMarinesKilled;
            ClawMinigunMarinesKilled = clawMinigunMarinesKilled;
            ClawRailgunMarinesKilled = clawRailgunMarinesKilled;
            MinigunMinigunMarinesKilled = minigunMinigunMarinesKilled;
            RailgunRailgunMarinesKilled = railgunRailgunMarinesKilled;
            ShotgunMarinesKilled = shotgunMarinesKilled;
            FlamethrowerMarinesKilled = flamethrowerMarinesKilled;
            GrenadeLauncherMarinesKilled = grenadeLauncherMarinesKilled;
            MarineTeamResourcesTotal = marineTeamResourcesTotal;
            AlienTeamResourcesTotal = alienTeamResourcesTotal;
            HivesKilled = hivesKilled;
            ChairsKilled = chairsKilled;
            TotalPlayerCount = totalPlayerCount;
            FullGameStrangerCount = fullGameStrangerCount;
            FullGamePrimerWithGamesCount = fullGamePrimerWithGamesCount;
            FullGameSupportingMemberCount = fullGameSupportingMemberCount;
            CaptainsMode = captainsMode;
            IncludedBots = includedBots;
            BuildNumber = buildNumber;
            MarineStartLocationName = marineStartLocationName;
            AlienStartLocationName = alienStartLocationName;
            MapName = mapName;
            StartingLocationsPathDistance = startingLocationsPathDistance;
            SurrenderTeamNumber = surrenderTeamNumber;
            WinOrLoseEndGameCountdownValue = winOrLoseEndGameCountdownValue;
            PlayerId = playerId;
            MarineSeconds = marineSeconds;
            AlienSeconds = alienSeconds;
            CommanderSeconds = commanderSeconds;
            EndGameCommander = endGameCommander;
            Captain = captain;
            Score = score;
            Kills = kills;
            Assists = assists;
            Deaths = deaths;
            PlayerIsSupportingMember = playerIsSupportingMember;
            PlayerIsPrimerSignerWithGames = playerIsPrimerSignerWithGames;
            PlayerIsStranger = playerIsStranger;
            WeldGave = weldGave;
            HealSprayGave = healSprayGave;
            Created = created;
            LastModified = lastModified;
        }

        public string ServerName { get; private set; }
        public double StartTimeSeconds { get; private set; }
        public string Realm { get; private set; }
        public string GameMode { get; private set; }
        public double DurationInSeconds { get; private set; }
        public int WinningTeamNumber { get; private set; }
        public int SkulksKilled { get; private set; }
        public int GorgesKilled { get; private set; }
        public int LerksKilled { get; private set; }
        public int FadesKilled { get; private set; }
        public int OnosKilled { get; private set; }
        public int RifleMarinesKilled { get; private set; }
        public int JetpackMarinesKilled { get; private set; }
        public int ClawMinigunMarinesKilled { get; private set; }
        public int ClawRailgunMarinesKilled { get; private set; }
        public int MinigunMinigunMarinesKilled { get; private set; }
        public int RailgunRailgunMarinesKilled { get; private set; }
        public int ShotgunMarinesKilled { get; private set; }
        public int FlamethrowerMarinesKilled { get; private set; }
        public int GrenadeLauncherMarinesKilled { get; private set; }
        public double MarineTeamResourcesTotal { get; private set; }
        public double AlienTeamResourcesTotal { get; private set; }
        public int HivesKilled { get; private set; }
        public int ChairsKilled { get; private set; }
        public int TotalPlayerCount { get; private set; }
        public int FullGameStrangerCount { get; private set; }
        public int FullGamePrimerWithGamesCount { get; private set; }
        public int FullGameSupportingMemberCount { get; private set; }
        public bool CaptainsMode { get; private set; }
        public bool IncludedBots { get; private set; }
        public int BuildNumber { get; private set; }
        public string MarineStartLocationName { get; private set; }
        public string AlienStartLocationName { get; private set; }
        public string MapName { get; private set; }
        public double? StartingLocationsPathDistance { get; private set; }
        public int? SurrenderTeamNumber { get; private set; }
        public int? WinOrLoseEndGameCountdownValue { get; private set; }        
        public long PlayerId { get; private set; }
        public double MarineSeconds { get; private set; }
        public double AlienSeconds { get; private set; }
        public double CommanderSeconds { get; private set; }
        public bool EndGameCommander { get; private set; }
        public bool Captain { get; private set; }
        public int Score { get; private set; }
        public int Kills { get; private set; }
        public int Assists { get; private set; }
        public int Deaths { get; private set; }
        public bool PlayerIsSupportingMember { get; private set; }
        public bool PlayerIsPrimerSignerWithGames { get; private set; }
        public bool PlayerIsStranger { get; private set; }
        public double WeldGave { get; private set; }
        public double HealSprayGave { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
    }
}