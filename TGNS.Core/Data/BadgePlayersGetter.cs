using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IBadgePlayersGetter
    {
        IEnumerable<IPlayer> Get(int badgeId);
    }

    public class BadgePlayersGetter : DataAccessor, IBadgePlayersGetter
    {
        private readonly IBkaDataParser _bkaDataParser;

        public BadgePlayersGetter(string connectionString, IBkaDataParser bkaDataParser) : base(connectionString)
        {
            _bkaDataParser = bkaDataParser;
        }

        public IEnumerable<IPlayer> Get(int badgeId)
        {
            var result = new List<IPlayer>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select x.playerid AS PlayerId, d.datavalue AS DataValue from (select distinct playerid from achievements_badges_players where badgeid = @BadgeId and achievementsrealm = 'ns2') x left outer join data d on x.playerid = d.datarecordid and d.datarealm = 'ns2' and d.datatypename = 'bka'";
                    command.Prepare();
                    command.Parameters.AddWithValue("@BadgeId", badgeId);
                    command.CommandTimeout = 120;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PlayerId");
                            var bkaJson = reader.GetString("DataValue");
                            var bkaData = _bkaDataParser.Parse(bkaJson);
                            result.Add(new Player(bkaData.Bka, playerId));
                        }
                    }
                }
            }
            return result;
        }
    }
}