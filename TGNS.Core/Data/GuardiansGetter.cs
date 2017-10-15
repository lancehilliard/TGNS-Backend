using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IGuardiansGetter
    {
        IEnumerable<IGuardianData> Get();
    }

    public class GuardiansGetter : DataAccessor, IGuardiansGetter
    {
        public GuardiansGetter(string connectionString) : base(connectionString)
        {
        }
        public IEnumerable<IGuardianData> Get()
        {
            var result = new List<IGuardianData>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select datarecordid, dataupdated, (select datavalue from data where datarealm = 'ns2' and datatypename = 'bka' and datarecordid = d1.DataRecordId) as bkaJson, (select count(*) from games_players where playerid = d1.datarecordid and rowcreated > d1.dataupdated and rowcreated > DATE_SUB(CURDATE(), INTERVAL 1 MONTH)) as optedInGamesInTheLastMonth from data d1 where datarealm = 'ns2' and datatypename = 'guardian' and datavalue like '%true%' order by optedInGamesInTheLastMonth desc;";
                    command.Prepare();
                    command.CommandTimeout = 120;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = Convert.ToInt64(reader.GetString("datarecordid"));
                            var bkaJson = reader.IsDBNull(reader.GetOrdinal("bkaJson")) ? null : reader.GetString("bkaJson");
                            var lastOptedIn = reader.GetDateTime("dataupdated");
                            string bka = null;
                            if (bkaJson != null)
                            {
                                var bkaDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bkaJson);
                                bka = bkaDictionary["BKA"] as string;
                            }
                            result.Add(new GuardianData(playerId, bka, lastOptedIn));
                        }
                    }
                }
            }
            return result;
        }
    }

    public interface IGuardianData
    {
        long PlayerId { get; }
        string BetterKnownAs { get; }
        DateTime LastOptedIn { get; }
    }

    public class GuardianData : IGuardianData
    {
        public GuardianData(long playerId, string betterKnownAs, DateTime lastOptedIn)
        {
            PlayerId = playerId;
            BetterKnownAs = betterKnownAs;
            LastOptedIn = lastOptedIn;
        }

        public long PlayerId { get; private set; }
        public string BetterKnownAs { get; private set; }
        public DateTime LastOptedIn { get; private set; }
    }
}