using System.Configuration;
using System.Globalization;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using TGNS.Core.Data;

namespace TGNS.Endpoints
{
    public class BanHandler_1_0 : Handler
    {
        private IBansGetter _bansGetter;

        public BanHandler_1_0()
        {
            _bansGetter = new BansGetter(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        }

        protected override bool IsReadRequest(HttpRequest request)
        {
            var result = string.IsNullOrWhiteSpace(request["unban"]);
            return result;
        }
        protected override void Write(string realmName, HttpRequest request, MySqlCommand command)
        {
            var banDataInput = request["bandata"] ?? request["unbandata"];
            var banData = JsonConvert.DeserializeObject<dynamic>(banDataInput);
            var isAddingBan = string.Equals(request["unban"], "0");
            if (isAddingBan) {
                command.CommandText = "INSERT INTO bans (BanRealm, BanPlayerId, BanPlayerName, BanDurationInSeconds, BanUnbanTimeInSeconds, BanCreatorName, BanCreatorId, BanReason, BanIssuedTimeInSeconds) VALUES (@BanRealm, @BanPlayerId, @BanPlayerName, @BanDurationInSeconds, @BanUnbanTimeInSeconds, @BanCreatorName, @BanCreatorId, @BanReason, @BanIssuedTimeInSeconds);";
                command.Prepare();
                command.Parameters.AddWithValue("@BanRealm", realmName);
                command.Parameters.AddWithValue("@BanPlayerId", banData.ID);
                command.Parameters.AddWithValue("@BanPlayerName", banData.Name);
                command.Parameters.AddWithValue("@BanDurationInSeconds", banData.Duration);
                command.Parameters.AddWithValue("@BanUnbanTimeInSeconds", banData.UnbanTime);
                command.Parameters.AddWithValue("@BanCreatorName", banData.BannedBy);
                command.Parameters.AddWithValue("@BanCreatorId", banData.BannerID);
                command.Parameters.AddWithValue("@BanReason", banData.Reason);
                command.Parameters.AddWithValue("@BanIssuedTimeInSeconds", banData.Issued);
                command.ExecuteNonQuery();
            }
            else {
                command.CommandText = "DELETE FROM bans WHERE BanRealm = @BanRealm AND BanPlayerId = @BanPlayerId;";
                command.Prepare();
                command.Parameters.AddWithValue("@BanRealm", realmName);
                command.Parameters.AddWithValue("@BanPlayerId", banData.ID);
                command.ExecuteNonQuery();
            }
        }
        protected override string GetResponseJson(string realmName, HttpRequest request, MySqlConnection connection)
        {
            var bans = _bansGetter.Get(realmName);
            var banDatas = new Dictionary<string, Dictionary<string, object>>();
            foreach (var ban in bans)
            {
                var banData = new Dictionary<string, object>();
                banData["UnbanTime"] = ban.UnbanTime;
                banData["Name"] = ban.PlayerName;
                banData["BannedBy"] = ban.CreatorName;
                banData["Duration"] = ban.DurationInSeconds;
                banData["Reason"] = ban.Reason;
                var banPlayerId = ban.PlayerId;
                banDatas.Add(banPlayerId.ToString(CultureInfo.InvariantCulture), banData);
            }
            var banned = new Dictionary<string, Dictionary<string, Dictionary<string, object>>> {{"Banned", banDatas}};
            var result = JsonConvert.SerializeObject(banned);
            return result;
        }
    }
}