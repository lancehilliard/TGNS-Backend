using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TGNS.Core.Data
{
    public interface IFeedbacksGetter
    {
        IEnumerable<IFeedback> Get();
    }

    public class FeedbacksGetter : DataAccessor, IFeedbacksGetter
    {
        public FeedbacksGetter(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<IFeedback> Get()
        {
            var result = new List<IFeedback>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand { Connection = connection })
                {
                    command.CommandText = "SELECT f.FeedbackPlayerId, f.FeedbackSubject, f.FeedbackBody, f.RowCreated, f.RowUpdated, fr.FeedbackReaderPlayerId FROM feedback f LEFT OUTER JOIN feedback_read fr ON f.FeedbackPlayerId = fr.FeedbackPlayerId AND f.RowCreated = fr.FeedbackCreated;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerId = reader.GetInt64("FeedbackPlayerId");
                            var subject = reader.GetString("FeedbackSubject");
                            var body = reader.GetString("FeedbackBody");
                            var created = reader.GetDateTime("RowCreated");
                            var lastModified = reader.GetDateTime("RowUpdated");
                            var readPlayerId = reader.IsDBNull(reader.GetOrdinal("FeedbackReaderPlayerId")) ? default(long?) : reader.GetInt64("FeedbackReaderPlayerId");

                            var readPlayerIds = new List<long>();
                            if (readPlayerId.HasValue)
                            {
                                readPlayerIds.Add(readPlayerId.Value);
                            }
                            var existingResult = result.SingleOrDefault(x => x.PlayerId == playerId && x.Created == created);
                            if (existingResult != null)
                            {
                                readPlayerIds.AddRange(existingResult.ReadPlayerIds);
                                result.Remove(existingResult);
                            }
                            result.Add(new Feedback(playerId, subject, body, created, lastModified, readPlayerIds));
                        }
                    }
                }
            }
            return result;
        }
    }

    public interface IFeedback
    {
        [DisplayName("From")]
        long PlayerId { get; }
        string Subject { get; }
        string Body { get; }
        [DisplayName("When")]
        DateTime Created { get; }
        DateTime LastModified { get; }
        IEnumerable<long> ReadPlayerIds { get; }
    }

    public class Feedback : IFeedback
    {
        public long PlayerId { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
        public IEnumerable<long> ReadPlayerIds { get; private set; }

        public Feedback(long playerId, string subject, string body, DateTime created, DateTime lastModified, IEnumerable<long> readPlayerIds)
        {
            PlayerId = playerId;
            Subject = subject;
            Body = body;
            Created = created;
            LastModified = lastModified;
            ReadPlayerIds = readPlayerIds;
        }
    }
}
