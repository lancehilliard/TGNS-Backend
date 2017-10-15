using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TGNS.Core.Data
{
    public class DataAccessor
    {
        protected string ConnectionString { get; set; }

        public DataAccessor(string connectionString)
        {
            ConnectionString = connectionString;
        }

        //public void Add(string realmName, DateTime sentWhen, long? sourcePlayerId, long destinationPlayerId, int offeringName, string title, string message)
        //{
        //    using (var connection = new MySqlConnection(ConnectionString))
        //    {
        //        connection.Open();
        //        using (var command = new MySqlCommand())
        //        {
        //            command.Connection = connection;
        //            command.CommandText = @"INSERT INTO notifications_log (RealmName, SourcePlayerId, DestinationPlayerId, OfferingName, Title, Message, SentWhen) VALUES (@RealmName, @SourcePlayerId, @DestinationPlayerId, @OfferingName, @Title, @Message, @SentWhen);";
        //            command.Prepare();
        //            command.Parameters.AddWithValue("@RealmName", realmName);
        //            command.Parameters.AddWithValue("@SourcePlayerId", sourcePlayerId);
        //            command.Parameters.AddWithValue("@DestinationPlayerId", destinationPlayerId);
        //            command.Parameters.AddWithValue("@OfferingName", offeringName);
        //            command.Parameters.AddWithValue("@Title", title);
        //            command.Parameters.AddWithValue("@Message", message);
        //            command.Parameters.AddWithValue("@SentWhen", sentWhen);
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}