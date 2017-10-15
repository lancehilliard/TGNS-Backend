using MySql.Data.MySqlClient;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IApprovalsRemover
    {
        void Remove(string realm, string serverName, double startTimeSeconds, long sourcePlayerId, long targetPlayerId);
    }

    public class ApprovalsRemover : DataAccessor, IApprovalsRemover
    {
        public ApprovalsRemover(string connectionString) : base(connectionString)
        {
        }

        public void Remove(string realm, string serverName, double startTimeSeconds, long sourcePlayerId, long targetPlayerId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "DELETE FROM approvals WHERE Realm = @Realm AND ServerName = @ServerName AND StartTimeSeconds = @StartTimeSeconds AND SourcePlayerId = @SourcePlayerId AND TargetPlayerId = @TargetPlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realm);
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
                    command.Parameters.AddWithValue("@TargetPlayerId", targetPlayerId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}