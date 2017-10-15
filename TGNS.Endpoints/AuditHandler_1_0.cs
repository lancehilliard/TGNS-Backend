using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGNS.Endpoints
{
    public class AuditHandler_1_0 : Handler
    {
        static readonly string EMPTY_VALUE = "NULL";
        readonly IDictionary<int, string> _statements;
        public AuditHandler_1_0()
        {
            _statements = new Dictionary<int, string>();
            _statements.Add(382, "INSERT INTO games (ServerName, StartTimeSeconds, Realm, GameMode, DurationInSeconds, WinningTeamNumber, SkulksKilled, GorgesKilled, LerksKilled, FadesKilled, OnosKilled, RifleMarinesKilled, JetpackMarinesKilled, ClawMinigunMarinesKilled, ClawRailgunMarinesKilled, MinigunMinigunMarinesKilled, RailgunRailgunMarinesKilled, ShotgunMarinesKilled, FlamethrowerMarinesKilled, GrenadeLauncherMarinesKilled, MarineTeamResourcesTotal, AlienTeamResourcesTotal, HivesKilled, ChairsKilled, TotalPlayerCount, FullGameSupportingMemberCount, FullGamePrimerWithGamesCount, FullGameStrangerCount, CaptainsMode, IncludedBots, BuildNumber, MarineStartLocationName, AlienStartLocationName, MapName, StartingLocationsPathDistance, SurrenderTeamNumber, WinOrLoseEndGameCountdownValue, MarineBonusResourcesAwarded, AlienBonusResourcesAwarded, HarvestersKilled, ExtractorsKilled) VALUES (@ServerName, @StartTimeSeconds, @Realm, @GameMode, @DurationInSeconds, @WinningTeamNumber, @SkulksKilled, @GorgesKilled, @LerksKilled, @FadesKilled, @OnosKilled, @RifleMarinesKilled, @JetpackMarinesKilled, @ClawMinigunMarinesKilled, @ClawRailgunMarinesKilled, @MinigunMinigunMarinesKilled, @RailgunRailgunMarinesKilled, @ShotgunMarinesKilled, @FlamethrowerMarinesKilled, @GrenadeLauncherMarinesKilled, @MarineTeamResourcesTotal, @AlienTeamResourcesTotal, @HivesKilled, @ChairsKilled, @TotalPlayerCount, @FullGameSupportingMemberCount, @FullGamePrimerWithGamesCount, @FullGameStrangerCount, @CaptainsMode, @IncludedBots, @BuildNumber, @MarineStartLocationName, @AlienStartLocationName, @MapName, @StartingLocationsPathDistance, @SurrenderTeamNumber, @WinOrLoseEndGameCountdownValue, @MarineBonusResourcesAwarded, @AlienBonusResourcesAwarded, @HarvestersKilled, @ExtractorsKilled);");
            _statements.Add(718, "INSERT INTO games_players (ServerName, StartTimeSeconds, PlayerId, MarineSeconds, AlienSeconds, CommanderSeconds, EndGameCommander, Captain, Score, Kills, Assists, Deaths, SupportingMember, PrimerSignerWithGames, Stranger, WeldGave, HealSprayGave, GorgeSeconds, LerkSeconds, FadeSeconds, OnosSeconds, HarvestersKilled, ExtractorsKilled, ParasitesInjested, ParasitesLanded, StructuresBuilt, HarvestersBuilt, ExtractorsBuilt, IPV4) VALUES (@ServerName, @StartTimeSeconds, @PlayerId, @MarineSeconds, @AlienSeconds, @CommanderSeconds, @EndGameCommander, @Captain, @Score, @Kills, @Assists, @Deaths, @SupportingMember, @PrimerSignerWithGames, @Stranger, @WeldGave, @HealSprayGave, @GorgeSeconds, @LerkSeconds, @FadeSeconds, @OnosSeconds, @HarvestersKilled, @ExtractorsKilled, @ParasitesInjested, @ParasitesLanded, @StructuresBuilt, @HarvestersBuilt, @ExtractorsBuilt, @IPV4);");
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            return false;
        }

        protected override void Write(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlCommand command)
        {
            var statementId = Convert.ToInt32(request["s"]);
            var gameMode = request["g"];
            var serverName = request["n"];
            var propertyData = JsonConvert.DeserializeObject<Dictionary<string, object>>(request["v"]);
            command.CommandText = _statements[statementId];
            command.Prepare();
            command.Parameters.AddWithValue("@Realm", realmName);
            command.Parameters.AddWithValue("@GameMode", gameMode);
            command.Parameters.AddWithValue("@ServerName", serverName);
            foreach (var key in propertyData.Keys)
            {
                var value = propertyData[key];
                command.Parameters.AddWithValue(string.Format("@{0}", key), value is string && (string)value == EMPTY_VALUE ? DBNull.Value : value);
            }
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}