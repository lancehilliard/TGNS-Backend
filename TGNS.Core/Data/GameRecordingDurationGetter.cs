using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IGameRecordingDurationGetter
    {
        double GetTotalGamesDurationInSeconds();
    }

    public class GameRecordingDurationGetter : DataAccessor, IGameRecordingDurationGetter
    {
        public double GetTotalGamesDurationInSeconds()
        {
            double result;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select sum(durationinseconds) as TotalDurationInSeconds from games g inner join (select distinct servername, starttimeseconds from games_recordings) x on x.servername = g.servername and x.starttimeseconds = g.starttimeseconds;";
                    command.Prepare();
                    result = (double)command.ExecuteScalar();
                }
            }
            return result;
        }

        public GameRecordingDurationGetter(string connectionString) : base(connectionString)
        {
        }
    }
}