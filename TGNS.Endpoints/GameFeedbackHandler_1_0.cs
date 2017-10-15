using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace TGNS.Endpoints
{
    public class GameFeedbackHandler_1_0 : Handler
    {
        protected override bool IsReadRequest(HttpRequest request)
        {
            return false;
        }

        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var serverName = request["n"];
            var startTimeSeconds = request["t"];
            var playerId = Convert.ToInt64(request["i"]);
            var rating = Convert.ToInt32(request["rating"]);
            var reasons = request["reasons"] ?? string.Empty;
            reasons = reasons.Substring(0, reasons.Length < 500 ? reasons.Length : 500);
            command.CommandText = "INSERT INTO games_feedback (ServerName, StartTimeSeconds, PlayerId, Rating, Reasons) VALUES (@ServerName, @StartTimeSeconds, @PlayerId, @Rating, @Reasons) ON DUPLICATE KEY UPDATE Rating=@Rating, Reasons=@Reasons;";
            command.Prepare();
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ServerName", serverName);
            command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
            command.Parameters.AddWithValue("@PlayerId", playerId);
            command.Parameters.AddWithValue("@Rating", rating);
            command.Parameters.AddWithValue("@Reasons", string.IsNullOrWhiteSpace(reasons) ? DBNull.Value : (object)reasons);
            command.ExecuteNonQuery();
        }

        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}