using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IFriendRequestsGetter
    {
        IEnumerable<long> GetFriendRequestTargetNs2Ids();
    }

    public class FriendRequestsGetter : DataAccessor, IFriendRequestsGetter
    {
        public FriendRequestsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<long> GetFriendRequestTargetNs2Ids()
        {
            var result = new List<long>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = @"
select PlayerId
from (select gp.playerid as PlayerId, count(1) as FifteenPlusMinuteRecentGames, max(fr.rowupdated) as MostRecentInviteSentDate
from games g
inner join games_players gp on g.servername = gp.servername and g.starttimeseconds = gp.starttimeseconds
left outer join chatbots_friendrequests fr on gp.playerid = fr.targetplayerid
where g.durationinseconds >= 15 * 60
and gp.RowUpdated > date_sub(current_timestamp(), interval 7 day)
group by gp.playerid) x
where FifteenPlusMinuteRecentGames >= 10
and MostRecentInviteSentDate is null or MostRecentInviteSentDate < date_sub(current_timestamp(), interval 14 day)
order by FifteenPlusMinuteRecentGames desc;
";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetInt64("PlayerId"));
                        }
                    }
                }
            }
            // result = new List<long> { 160301 };
            return result;
        } 
    }
}
