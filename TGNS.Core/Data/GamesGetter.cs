using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IGamesGetter
    {
        IGame Get(string serverName, double startTimeSeconds);
        IEnumerable<IGame> Get(DateTime createdAfter);
    }

    public class GamesGetter : DataAccessor, IGamesGetter
    {
private static readonly string QueryWithoutPredicate = @"SELECT 
    g.ServerName,
    g.StartTimeSeconds,
    Realm,
    GameMode,
    DurationInSeconds,
    WinningTeamNumber,
    SkulksKilled,
    GorgesKilled,
    LerksKilled,
    FadesKilled,
    OnosKilled,
    RifleMarinesKilled,
    JetpackMarinesKilled,
    ClawMinigunMarinesKilled,
    ClawRailgunMarinesKilled,
    MinigunMinigunMarinesKilled,
    RailgunRailgunMarinesKilled,
    ShotgunMarinesKilled,
    FlamethrowerMarinesKilled,
    GrenadeLauncherMarinesKilled,
    MarineTeamResourcesTotal,
    AlienTeamResourcesTotal,
    HivesKilled,
    ChairsKilled,
    TotalPlayerCount,
    FullGameStrangerCount,
    FullGamePrimerWithGamesCount,
    FullGameSupportingMemberCount,
    CaptainsMode,
    IncludedBots,
    BuildNumber,
    MarineStartLocationName,
    AlienStartLocationName,
    MapName,
    StartingLocationsPathDistance,
    SurrenderTeamNumber,
    WinOrLoseEndGameCountdownValue,
    RowCreated,
    RowUpdated,
    COALESCE(RecordingsCount, 0) AS RecordingsCount,
    COALESCE(RatingsCount, 0) AS RatingsCount,
    COALESCE(RatingsAverage, 0) AS RatingsAverage,
    (SELECT 
            GROUP_CONCAT(DISTINCT CASE gp.StartTimeSeconds IS NULL
                        WHEN TRUE THEN 'S'
                        ELSE CONCAT(CASE gp.MarineSeconds > gp.AlienSeconds
                                    WHEN TRUE THEN 'M'
                                    ELSE 'A'
                                END,
                                CASE gp.CommanderSeconds > g.DurationInSeconds / 2
                                    WHEN TRUE THEN 'C'
                                    ELSE ''
                                END)
                    END
                    ORDER BY 1
                    SEPARATOR ', ')
        FROM
            games_recordings gr
                LEFT OUTER JOIN
            games_players gp ON gr.ServerName = gp.ServerName
                AND gr.StartTimeSeconds = gp.StartTimeSeconds
                AND gr.PlayerId = gp.PlayerId
                LEFT OUTER JOIN
            games g2 ON gr.ServerName = g2.ServerName
                AND gr.StartTimeSeconds = g2.StartTimeSeconds
        WHERE
            gr.ServerName = g.ServerName
                AND gr.StartTimeSeconds = g.StartTimeSeconds) AS RecordingCameraViewNames
FROM
    games g
        LEFT JOIN
    (SELECT DISTINCT
        ServerName, StartTimeSeconds, COUNT(1) AS RecordingsCount
    FROM
        games_recordings
    GROUP BY ServerName , StartTimeSeconds) r ON g.servername = r.servername
        AND g.starttimeseconds = r.starttimeseconds
        LEFT JOIN
    (SELECT DISTINCT
        servername,
            starttimeseconds,
            AVG(rating) AS RatingsAverage,
            COUNT(rating) AS RatingsCount
    FROM
        games_feedback
    WHERE
        rating >= 1
    GROUP BY servername , starttimeseconds) f ON g.servername = f.servername
        AND g.starttimeseconds = f.starttimeseconds";


        private readonly IPlayedGamesGetter _playedGamesGetter;
        private readonly IGameRecordingsGetter _gameRecordingsGetter;
        private readonly ICameraViewNamesGetter _cameraViewNamesGetter;
        public GamesGetter(string connectionString) : base(connectionString)
        {
            _playedGamesGetter = new PlayedGamesGetter(connectionString);
            _gameRecordingsGetter = new GameRecordingsGetter(connectionString);
            _cameraViewNamesGetter = new CameraViewNamesGetter(connectionString);
        }
        public IGame Get(string serverName, double startTimeSeconds)
        {
            IGame result;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = $@"{QueryWithoutPredicate} WHERE g.servername = @ServerName AND g.starttimeseconds = @StartTimeSeconds;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    using (var reader = command.ExecuteReader())
                    {
                        result = GetGamesFromReader(reader).SingleOrDefault();
                    }
                }
            }
            return result;
        }

        private IEnumerable<IGame> GetGamesFromReader(MySqlDataReader reader)
        {
            var result = new List<IGame>();
            while (reader.Read())
            {
                var serverName = reader.GetString("ServerName");
                var startTimeSeconds = reader.GetDouble("StartTimeSeconds");
                var realm = reader.GetString("Realm");
                var gameMode = reader.GetString("GameMode");
                var durationInSeconds = reader.GetDouble("DurationInSeconds");
                var winningTeamNumber = reader.GetInt32("WinningTeamNumber");
                var skulksKilled = reader.GetInt32("SkulksKilled");
                var gorgesKilled = reader.GetInt32("GorgesKilled");
                var lerksKilled = reader.GetInt32("LerksKilled");
                var fadesKilled = reader.GetInt32("FadesKilled");
                var onosKilled = reader.GetInt32("OnosKilled");
                var rifleMarinesKilled = reader.GetInt32("RifleMarinesKilled");
                var jetpackMarinesKilled = reader.GetInt32("JetpackMarinesKilled");
                var clawMinigunMarinesKilled = reader.GetInt32("ClawMinigunMarinesKilled");
                var clawRailgunMarinesKilled = reader.GetInt32("ClawRailgunMarinesKilled");
                var minigunMinigunMarinesKilled = reader.GetInt32("MinigunMinigunMarinesKilled");
                var railgunRailgunMarinesKilled = reader.GetInt32("RailgunRailgunMarinesKilled");
                var shotgunMarinesKilled = reader.GetInt32("ShotgunMarinesKilled");
                var flamethrowerMarinesKilled = reader.GetInt32("FlamethrowerMarinesKilled");
                var grenadeLauncherMarinesKilled = reader.GetInt32("GrenadeLauncherMarinesKilled");
                var marineTeamResourcesTotal = reader.GetDouble("MarineTeamResourcesTotal");
                var alienTeamResourcesTotal = reader.GetDouble("AlienTeamResourcesTotal");
                var hivesKilled = reader.GetInt32("HivesKilled");
                var chairsKilled = reader.GetInt32("ChairsKilled");
                var totalPlayerCount = reader.GetInt32("TotalPlayerCount");
                var fullGameStrangerCount = reader.GetInt32("FullGameStrangerCount");
                var fullGamePrimerWithGamesCount = reader.GetInt32("FullGamePrimerWithGamesCount");
                var fullGameSupportingMemberCount = reader.GetInt32("FullGameSupportingMemberCount");
                var captainsMode = reader.GetBoolean("CaptainsMode");
                var includedBots = reader.GetBoolean("IncludedBots");
                var buildNumber = reader.GetInt32("BuildNumber");
                var marineStartLocationNameOrdinal = reader.GetOrdinal("MarineStartLocationName");
                var marineStartLocationName = reader.IsDBNull(marineStartLocationNameOrdinal) ? null : reader.GetString(marineStartLocationNameOrdinal);
                var alienStartLocationNameOrdinal = reader.GetOrdinal("AlienStartLocationName");
                var alienStartLocationName = reader.IsDBNull(alienStartLocationNameOrdinal) ? null : reader.GetString(alienStartLocationNameOrdinal);
                var mapName = reader.GetString("MapName");
                var startingLocationsPathDistanceOrdinal = reader.GetOrdinal("StartingLocationsPathDistance");
                var startingLocationsPathDistance = reader.IsDBNull(startingLocationsPathDistanceOrdinal) ? default(double?) : reader.GetDouble(startingLocationsPathDistanceOrdinal);
                var surrenderTeamNumberOrdinal = reader.GetOrdinal("SurrenderTeamNumber");
                var surrenderTeamNumber = reader.IsDBNull(surrenderTeamNumberOrdinal) ? null : (int?) reader.GetInt32(surrenderTeamNumberOrdinal);
                var winOrLoseEndGameCountdownOrdinal = reader.GetOrdinal("winOrLoseEndGameCountdownValue");
                var winOrLoseEndGameCountdownValue = reader.IsDBNull(winOrLoseEndGameCountdownOrdinal) ? null : (int?) reader.GetInt32(winOrLoseEndGameCountdownOrdinal);
                var created = reader.GetDateTime("RowCreated");
                var lastModified = reader.GetDateTime("RowUpdated");
                var recordingsCount = reader.GetInt32("RecordingsCount");
                var ratingsCount = reader.GetInt32("RatingsCount");
                var ratingsAverage = reader.GetDecimal("RatingsAverage");
                var recordingsCameraViewNames = (recordingsCount > 0 ? reader.GetString("RecordingCameraViewNames") : string.Empty).Split(new[] { ", " }, StringSplitOptions.None);
                result.Add(new Game(serverName, startTimeSeconds, realm, gameMode, durationInSeconds, winningTeamNumber, skulksKilled, gorgesKilled, lerksKilled, fadesKilled, onosKilled, rifleMarinesKilled, jetpackMarinesKilled, clawMinigunMarinesKilled, clawRailgunMarinesKilled, minigunMinigunMarinesKilled, railgunRailgunMarinesKilled, shotgunMarinesKilled, flamethrowerMarinesKilled, grenadeLauncherMarinesKilled, marineTeamResourcesTotal, alienTeamResourcesTotal, hivesKilled, chairsKilled, totalPlayerCount, fullGameStrangerCount, fullGamePrimerWithGamesCount, fullGameSupportingMemberCount, captainsMode, includedBots, buildNumber, marineStartLocationName, alienStartLocationName, mapName, startingLocationsPathDistance, surrenderTeamNumber, winOrLoseEndGameCountdownValue, created, lastModified, recordingsCount, ratingsCount, ratingsAverage, recordingsCameraViewNames));
            }
            return result;
        }

        public IEnumerable<IGame> Get(DateTime createdAfter)
        {
            var result = new List<IGame>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = $@"{QueryWithoutPredicate} WHERE g.rowcreated >= @RowCreated;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@RowCreated", createdAfter);
                    using (var reader = command.ExecuteReader())
                    {
                        result.AddRange(GetGamesFromReader(reader));
                    }
                }
            }
            return result;
        }
    }

    public interface IGame
    {
        int? RecordingsCount { get; }
        int RatingsCount { get; }
        decimal RatingsAverage { get; }
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
        DateTime Created { get; }
        DateTime LastModified { get; }
        IEnumerable<string> RecordingsCameraViewNames { get; }
    }

    public class Game : IGame
    {
        public Game(string serverName, double startTimeSeconds, string realm, string gameMode, double durationInSeconds, int winningTeamNumber, int skulksKilled, int gorgesKilled, int lerksKilled, int fadesKilled, int onosKilled, int rifleMarinesKilled, int jetpackMarinesKilled, int clawMinigunMarinesKilled, int clawRailgunMarinesKilled, int minigunMinigunMarinesKilled, int railgunRailgunMarinesKilled, int shotgunMarinesKilled, int flamethrowerMarinesKilled, int grenadeLauncherMarinesKilled, double marineTeamResourcesTotal, double alienTeamResourcesTotal, int hivesKilled, int chairsKilled, int totalPlayerCount, int fullGameStrangerCount, int fullGamePrimerWithGamesCount, int fullGameSupportingMemberCount, bool captainsMode, bool includedBots, int buildNumber, string marineStartLocationName, string alienStartLocationName, string mapName, double? startingLocationsPathDistance, int? surrenderTeamNumber, int? winOrLoseEndGameCountdownValue, DateTime created, DateTime lastModified, int? recordingsCount, int ratingsCount, decimal ratingsAverage, IEnumerable<string> recordingsCameraViewNames)
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
            Created = created;
            LastModified = lastModified;
            RecordingsCount = recordingsCount;
            RatingsCount = ratingsCount;
            RatingsAverage = ratingsAverage;
            RecordingsCameraViewNames = recordingsCameraViewNames;
        }

        public IEnumerable<string> RecordingsCameraViewNames { get; }

        public decimal RatingsAverage { get; set; }

        public int RatingsCount { get; set; }

        public int? RecordingsCount { get; }
        public string ServerName { get; }
        public double StartTimeSeconds { get; }
        public string Realm { get; }
        public string GameMode { get; }
        public double DurationInSeconds { get; }
        public int WinningTeamNumber { get; }
        public int SkulksKilled { get; }
        public int GorgesKilled { get; }
        public int LerksKilled { get; }
        public int FadesKilled { get; }
        public int OnosKilled { get; }
        public int RifleMarinesKilled { get; }
        public int JetpackMarinesKilled { get; }
        public int ClawMinigunMarinesKilled { get; }
        public int ClawRailgunMarinesKilled { get; }
        public int MinigunMinigunMarinesKilled { get; }
        public int RailgunRailgunMarinesKilled { get; }
        public int ShotgunMarinesKilled { get; }
        public int FlamethrowerMarinesKilled { get; }
        public int GrenadeLauncherMarinesKilled { get; }
        public double MarineTeamResourcesTotal { get; }
        public double AlienTeamResourcesTotal { get; }
        public int HivesKilled { get; }
        public int ChairsKilled { get; }
        public int TotalPlayerCount { get; }
        public int FullGameStrangerCount { get; }
        public int FullGamePrimerWithGamesCount { get; }
        public int FullGameSupportingMemberCount { get; }
        public bool CaptainsMode { get; }
        public bool IncludedBots { get; }
        public int BuildNumber { get; }
        public string MarineStartLocationName { get; }
        public string AlienStartLocationName { get; }
        public string MapName { get; }
        public double? StartingLocationsPathDistance { get; }
        public int? SurrenderTeamNumber { get; }
        public int? WinOrLoseEndGameCountdownValue { get; }
        public DateTime Created { get; }
        public DateTime LastModified { get; }
    }
}