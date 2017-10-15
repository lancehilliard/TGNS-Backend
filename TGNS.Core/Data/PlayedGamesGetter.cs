using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IPlayedGamesGetter
    {
        IEnumerable<IPlayedGame> Get(string realmName, long playerId);
    }

    public class PlayedGamesGetter : DataAccessor, IPlayedGamesGetter
    {
        private static readonly string _baseSelectQuery = "SELECT g.ServerName, g.StartTimeSeconds, g.Realm, g.GameMode, g.DurationInSeconds, g.WinningTeamNumber, g.SkulksKilled, g.GorgesKilled, g.LerksKilled, g.FadesKilled, g.OnosKilled, g.RifleMarinesKilled, g.JetpackMarinesKilled, g.ClawMinigunMarinesKilled, g.ClawRailgunMarinesKilled, g.MinigunMinigunMarinesKilled, g.RailgunRailgunMarinesKilled, g.ShotgunMarinesKilled, g.FlamethrowerMarinesKilled, g.GrenadeLauncherMarinesKilled, g.MarineTeamResourcesTotal, g.AlienTeamResourcesTotal, g.HivesKilled, g.ChairsKilled, g.TotalPlayerCount, g.FullGameStrangerCount, g.FullGamePrimerWithGamesCount, g.FullGameSupportingMemberCount, g.CaptainsMode, g.IncludedBots, g.BuildNumber, g.MarineStartLocationName, g.AlienStartLocationName, g.MapName, g.StartingLocationsPathDistance, g.SurrenderTeamNumber, g.WinOrLoseEndGameCountdownValue, g.RowCreated, g.RowUpdated, gp.PlayerId, gp.MarineSeconds, gp.AlienSeconds, gp.CommanderSeconds, gp.EndGameCommander, gp.Captain, gp.Score, gp.Kills, gp.Assists, gp.Deaths, gp.SupportingMember, gp.PrimerSignerWithGames, gp.Stranger, gp.WeldGave, gp.HealSprayGave from games_players gp inner join games g on g.servername = gp.servername and g.starttimeseconds = gp.starttimeseconds";
        public PlayedGamesGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IPlayedGame> Get(string realmName, long playerId)
        {
            var result = new List<IPlayedGame>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = string.Format("{0} WHERE g.realm=@Realm AND gp.playerid = @PlayerId;", _baseSelectQuery);
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realmName);
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    result.AddRange(GetPlayedGames(command));
                }
            }
            return result;
        }

        private IEnumerable<IPlayedGame> GetPlayedGames(MySqlCommand command)
        {
            List<IPlayedGame> result = new List<IPlayedGame>();
            using (var reader = command.ExecuteReader())
            {
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

                    var marineSeconds = reader.GetDouble("MarineSeconds");
                    var alienSeconds = reader.GetDouble("AlienSeconds");
                    var commanderSeconds = reader.GetDouble("CommanderSeconds");
                    var endGameCommander = reader.GetBoolean("EndGameCommander");
                    var captain = reader.GetBoolean("Captain");
                    var score = reader.GetInt32("Score");
                    var kills = reader.GetInt32("Kills");
                    var assists = reader.GetInt32("Assists");
                    var deaths = reader.GetInt32("Deaths");
                    var supportingMember = reader.GetBoolean("SupportingMember");
                    var primerSignerWithGames = reader.GetBoolean("PrimerSignerWithGames");
                    var stranger = reader.GetBoolean("Stranger");
                    var weldGave = reader.GetDouble("WeldGave");
                    var healSprayGave = reader.GetDouble("HealSprayGave");
                    var created = reader.GetDateTime("RowCreated");
                    var lastModified = reader.GetDateTime("RowUpdated");
                    var playerId = reader.GetInt64("PlayerId");
                    result.Add(new PlayedGame(serverName, startTimeSeconds, realm, gameMode, durationInSeconds, winningTeamNumber, skulksKilled, gorgesKilled, lerksKilled, fadesKilled, onosKilled, rifleMarinesKilled, jetpackMarinesKilled, clawMinigunMarinesKilled, clawRailgunMarinesKilled, minigunMinigunMarinesKilled, railgunRailgunMarinesKilled, shotgunMarinesKilled, flamethrowerMarinesKilled, grenadeLauncherMarinesKilled, marineTeamResourcesTotal, alienTeamResourcesTotal, hivesKilled, chairsKilled, totalPlayerCount, fullGameStrangerCount, fullGamePrimerWithGamesCount, fullGameSupportingMemberCount, captainsMode, includedBots, buildNumber, marineStartLocationName, alienStartLocationName, mapName, startingLocationsPathDistance, surrenderTeamNumber, winOrLoseEndGameCountdownValue, playerId, marineSeconds, alienSeconds, commanderSeconds, endGameCommander, captain, score, kills, assists, deaths, supportingMember, primerSignerWithGames, stranger, weldGave, healSprayGave, created, lastModified));
                }
            }
            return result;
        }
    }
}
