using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IReplayGamesDatatablesDataGetter
    {
        IEnumerable<IEnumerable<string>> Get(string gameMode, long globalRatingsAverage);
        long GetGlobalRatingsAverage(string gameMode);
    }

    public class ReplayGamesDatatablesDataGetter : DataAccessor, IReplayGamesDatatablesDataGetter
    {
        public ReplayGamesDatatablesDataGetter(string connectionString) : base(connectionString)
        {
        }
        public IEnumerable<IEnumerable<string>> Get(string gameMode, long globalRatingsAverage)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"

select y.ServerName as ServerName
	, y.StartTimeSeconds as StartTimeSeconds
    , MapName
    , COALESCE(CASE WHEN PlayerRecordingsCount < RecordingsCount THEN CONCAT(PlayerCameraViewNames,', S') ELSE PlayerCameraViewNames END, 'S') AS CameraViewNames
    , DurationInSeconds
    , RecordingPlayerIds
    , COALESCE(PlayingPlayerIds, '') as PlayingPlayerIds
    , RatingsCount
    , RatingsAverage
            -- http://stackoverflow.com/questions/1411199/what-is-a-better-way-to-sort-by-a-5-star-rating
            -- (v ÷ (v+m)) × R + (m ÷ (v+m)) × C
            -- * R = average for the movie (mean) = (Rating)
            -- * v = number of votes for the movie = (votes)
            -- * m = minimum votes required to be listed in the Top 250 (currently 1300)
            -- * C = the mean vote across the whole report (currently 6.8)
	, CASE WHEN RatingsCount > 0 THEN (RatingsCount / (RatingsCount+1)) * RatingsAverage + (1 / (RatingsCount+1)) * @GlobalRatingsAverage ELSE 0 END AS WeightedRating
    , GameMode
    , RowCreated
from (select x.ServerName as ServerName
	, x.StartTimeSeconds as StartTimeSeconds
    , x.MapName as MapName
    , x.DurationInSeconds as DurationInSeconds
    , x.GameMode as GameMode
    , x.PlayingPlayerIds as PlayingPlayerIds
    , x.PlayerRecordingsCount as PlayerRecordingsCount
    , grc.RecordingsCount as RecordingsCount
    , x.PlayerCameraViewNames as PlayerCameraViewNames
    , x.RatingsCount as RatingsCount
    , x.RatingsAverage as RatingsAverage
    , x.RowCreated as RowCreated
from (select g.servername as ServerName
	, g.starttimeseconds as StartTimeSeconds
    , g.DurationInSeconds as DurationInSeconds
    , g.GameMode as GameMode
    , prc.PlayerRecordingsCount as PlayerRecordingsCount
    , g.MapName as MapName
	, GROUP_CONCAT(DISTINCT gp.PlayerId ORDER BY 1 SEPARATOR ',') AS PlayingPlayerIds
	, GROUP_CONCAT(DISTINCT CASE
		WHEN
			gr.PlayerId = gp.PlayerId
		THEN
			CASE gp.PlayerId IS NULL
				WHEN TRUE THEN 'S'
				ELSE CONCAT(CASE gp.MarineSeconds > gp.AlienSeconds
					WHEN TRUE THEN 'M'
					ELSE 'A'
				END, CASE gp.CommanderSeconds > g.DurationInSeconds / 2
					WHEN TRUE THEN 'C'
					ELSE ''
				END)
			END
		END ORDER BY 1 SEPARATOR ', ') AS PlayerCameraViewNames
    , g.RowCreated as RowCreated
    , COALESCE(COUNT(DISTINCT CASE WHEN gf.Rating >= 1 THEN gf.PlayerId END), 0) AS RatingsCount
    , COALESCE(AVG(CASE WHEN gf.Rating >= 1 THEN gf.Rating END), 0) AS RatingsAverage    
from games_recordings gr
left join games_feedback gf on gf.servername = gr.servername and gf.starttimeseconds = gr.starttimeseconds
left join (select prc_gr.servername, prc_gr.starttimeseconds, count(prc_gr.playerid) as PlayerRecordingsCount from games_recordings prc_gr inner join games_players prc_gp where prc_gp.servername = prc_gr.servername and prc_gp.starttimeseconds = prc_gr.starttimeseconds and prc_gp.playerid = prc_gr.playerid group by prc_gr.servername, prc_gr.starttimeseconds) prc on prc.servername = gr.servername and prc.starttimeseconds = gr.starttimeseconds
inner join games g on g.servername = gr.servername and g.starttimeseconds = gr.starttimeseconds
left join games_players gp on gp.servername = gr.servername and gp.starttimeseconds = gr.starttimeseconds
group by gr.servername, gr.starttimeseconds) x
inner join (select servername, starttimeseconds, count(distinct playerid) as RecordingsCount from games_recordings group by servername, starttimeseconds) grc on grc.servername = x.servername and grc.starttimeseconds = x.starttimeseconds) y
inner join (select servername, starttimeseconds, GROUP_CONCAT(DISTINCT PlayerId ORDER BY 1 SEPARATOR ',') AS RecordingPlayerIds from games_recordings group by servername, starttimeseconds) z on z.servername = y.servername and z.starttimeseconds = y.starttimeseconds
where RecordingsCount > 0 and DurationInSeconds > 60 and GameMode = @GameMode
order by RowCreated DESC;

";
                    command.Parameters.AddWithValue("@GameMode", gameMode);
                    command.Parameters.AddWithValue("@GlobalRatingsAverage", globalRatingsAverage);
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var trimmedMapName = reader.GetString("MapName").Replace("ns2_", "");
                            var result = new List<string>();
                            result.Add(reader.GetString("ServerName"));
                            result.Add(reader.GetString("StartTimeSeconds"));
                            result.Add($"{trimmedMapName}");
                            result.Add(trimmedMapName);
                            result.Add(reader.GetDateTime("RowCreated").ToShortDateString());
                            result.Add(reader.GetString("RowCreated"));
                            result.Add($"{Convert.ToInt32(reader.GetDouble("DurationInSeconds")/60)}m");
                            result.Add($"{reader.GetDouble("DurationInSeconds")}");
                            result.Add(reader.GetString("RecordingPlayerIds"));
                            result.Add(reader.GetString("PlayingPlayerIds"));
                            result.Add($"{reader.GetInt32("RatingsCount")}");
                            result.Add($"{reader.GetDouble("RatingsAverage")}");
                            result.Add($"{reader.GetDouble("WeightedRating")}");
                            result.Add($"{reader.GetString("CameraViewNames")}");
                            yield return result;
                        }
                    }
                }
            }
        }

        public long GetGlobalRatingsAverage(string gameMode)
        {
            long result;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select COALESCE(avg(gf.Rating), 0) RatingAverage from games g inner join games_feedback gf on g.servername = gf.servername and g.starttimeseconds = gf.starttimeseconds where Rating >= 1 and g.gamemode = @GameMode;";
                    command.Parameters.AddWithValue("@GameMode", gameMode);
                    command.Prepare();
                    result = Convert.ToInt64(command.ExecuteScalar());
                }
            }
            return result;
        }

    }
}