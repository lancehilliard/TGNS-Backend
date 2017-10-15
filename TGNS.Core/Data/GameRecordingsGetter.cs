using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TGNS.Core.Domain;
using TGNS.Core.Messaging;

namespace TGNS.Core.Data
{
    public interface IGameRecordingsGetter
    {
        IEnumerable<IGameRecording> Get(string serverName, double startTimeSeconds);
        IEnumerable<IRecordedGame> GetRecordedGames();
        IEnumerable<IRecordedGame> GetRecordedGamesByPlayer(long playerId);
        IEnumerable<IGameRecording> Get();
        long GetDistinctPlayerCount();
    }

    public class GameRecordingsGetter : DataAccessor, IGameRecordingsGetter
    {
        public GameRecordingsGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IGameRecording> Get(string serverName, double startTimeSeconds)
        {
            var result = new List<IGameRecording>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT PlayerId, VideoIdentifier FROM games_recordings WHERE ServerName=@ServerName AND StartTimeSeconds = @StartTimeSeconds;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@ServerName", serverName);
                    command.Parameters.AddWithValue("@StartTimeSeconds", startTimeSeconds);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PlayerId");
                            var videoIdentifier = reader.GetString("VideoIdentifier");
                            result.Add(new GameRecording(playerId, videoIdentifier));
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<IGameRecording> Get()
        {
            var result = new List<IGameRecording>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT PlayerId, VideoIdentifier FROM games_recordings;";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("PlayerId");
                            var videoIdentifier = reader.GetString("VideoIdentifier");
                            result.Add(new GameRecording(playerId, videoIdentifier));
                        }
                    }
                }
            }
            return result;
        }

        public long GetDistinctPlayerCount()
        {
            long result;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "select count(distinct gp.playerid) from games_players gp inner join games_recordings gr on gp.servername = gr.servername and gp.starttimeseconds = gr.starttimeseconds;";
                    command.Prepare();
                    result = (long)command.ExecuteScalar();
                }
            }
            return result;
        }

        public IEnumerable<IRecordedGame> GetRecordedGames()
        {
            var result = new List<IRecordedGame>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT DISTINCT ServerName, StartTimeSeconds, Count(1) As RecordingsCount FROM games_recordings GROUP BY ServerName, StartTimeSeconds;";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var serverName = reader.GetString("ServerName");
                            var startTimeSeconds = reader.GetDouble("StartTimeSeconds");
                            var recordingsCount = reader.GetInt32("RecordingsCount");
                            result.Add(new RecordedGame(serverName, startTimeSeconds, recordingsCount));
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<IRecordedGame> GetRecordedGamesByPlayer(long playerId)
        {
            var result = new List<IRecordedGame>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT DISTINCT ServerName, StartTimeSeconds, Count(1) As RecordingsCount FROM games_recordings WHERE PlayerId = @PlayerId GROUP BY ServerName, StartTimeSeconds;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlayerId", playerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var serverName = reader.GetString("ServerName");
                            var startTimeSeconds = reader.GetDouble("StartTimeSeconds");
                            var recordingsCount = reader.GetInt32("RecordingsCount");
                            result.Add(new RecordedGame(serverName, startTimeSeconds, recordingsCount));
                        }
                    }
                }
            }
            return result;
        }
    }

    public interface IRecordedGame
    {
        string ServerName { get; }
        double StartTimeSeconds { get; }
        int RecordingsCount { get; }
    }

    public class RecordedGame : IRecordedGame
    {
        public RecordedGame(string serverName, double startTimeSeconds, int recordingsCount)
        {
            ServerName = serverName;
            StartTimeSeconds = startTimeSeconds;
            RecordingsCount = recordingsCount;
        }

        public string ServerName { get; }
        public double StartTimeSeconds { get; }
        public int RecordingsCount { get; }
    }

    public interface IGameRecording
    {
        long PlayerId { get; }
        string VideoIdentifier { get; }
    }

    public class GameRecording : IGameRecording
    {
        public GameRecording(long playerId, string videoIdentifier)
        {
            PlayerId = playerId;
            VideoIdentifier = videoIdentifier;
        }

        public long PlayerId { get; }
        public string VideoIdentifier { get; }
    }
}