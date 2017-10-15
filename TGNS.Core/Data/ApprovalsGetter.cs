using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGNS.Core.Domain;

namespace TGNS.Core.Data
{
    public interface IApprovalsGetter
    {
        IEnumerable<IApproval> Get(string realmName);
        IEnumerable<IApproval> GetByTargetPlayerId(string realmName, long targetPlayerId);
    }

    public class ApprovalsGetter : DataAccessor, IApprovalsGetter
    {
        public ApprovalsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IApproval> Get(string realmName)
        {
            var result = new List<IApproval>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT ServerName, StartTimeSeconds, SourcePlayerId, TargetPlayerId, Reason, RowCreated, RowUpdated FROM approvals WHERE Realm=@Realm;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realmName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sourcePlayerId = reader.GetInt64("SourcePlayerId");
                            var targetPlayerId = reader.GetInt64("TargetPlayerId");
                            var serverName = reader.GetString("ServerName");
                            var startTimeSeconds = reader.GetDouble("StartTimeSeconds");
                            var reasonOrdinal = reader.GetOrdinal("Reason");
                            var reason = reader.IsDBNull(reasonOrdinal) ? null : reader.GetString(reasonOrdinal);
                            var created = reader.GetDateTime("RowCreated");
                            var lastModified = reader.GetDateTime("RowUpdated");
                            result.Add(new Approval(sourcePlayerId, targetPlayerId, serverName, startTimeSeconds, reason, realmName, created, lastModified));
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<IApproval> GetByTargetPlayerId(string realmName, long targetPlayerId)
        {
            var result = new List<IApproval>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT ServerName, StartTimeSeconds, SourcePlayerId, TargetPlayerId, Reason, RowCreated, RowUpdated FROM approvals WHERE Realm=@Realm AND TargetPlayerId = @TargetPlayerId;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@Realm", realmName);
                    command.Parameters.AddWithValue("@TargetPlayerId", targetPlayerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sourcePlayerId = reader.GetInt64("SourcePlayerId");
                            var serverName = reader.GetString("ServerName");
                            var startTimeSeconds = reader.GetDouble("StartTimeSeconds");
                            var reasonOrdinal = reader.GetOrdinal("Reason");
                            var reason = reader.IsDBNull(reasonOrdinal) ? null : reader.GetString(reasonOrdinal);
                            var created = reader.GetDateTime("RowCreated");
                            var lastModified = reader.GetDateTime("RowUpdated");
                            result.Add(new Approval(sourcePlayerId, targetPlayerId, serverName, startTimeSeconds, reason, realmName, created, lastModified));
                        }
                    }
                }
            }
            return result;
        }
    }
}