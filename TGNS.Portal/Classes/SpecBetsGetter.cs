using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface ISpecBetsGetter
    {
        IEnumerable<ISpecBetsData> Get();
    }

    public class SpecBetsGetter : ISpecBetsGetter
    {
        private readonly IBkaDataParser _bkaDataParser;

        public SpecBetsGetter(IBkaDataParser bkaDataParser)
        {
            _bkaDataParser = bkaDataParser;
        }

        public IEnumerable<ISpecBetsData> Get()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select t.makerplayerid, t.Total, t.PayoutRate, t.BetCount, t.BetMax, t.BetAverage, t.PayoutCount, t.PayoutMax, t.PayoutAverage, d.datavalue as BkaJson
from (select makerplayerid, sum(amount) as Total, sum(case type when 'payout' then 1 else 0 end) / sum(case type when 'bet' then 1 else 0 end) As PayoutRate, sum(case type when 'bet' then 1 else 0 end) as BetCount, min(case type when 'bet' then amount else 0 end) as BetMax, sum(case type when 'bet' then amount else 0 end) / sum(case type when 'bet' then 1 else 0 end) as BetAverage, sum(case type when 'payout' then 1 else 0 end) as PayoutCount, max(case type when 'payout' then amount else 0 end) as PayoutMax, sum(case type when 'payout' then amount else 0 end) / sum(case type when 'payout' then 1 else 0 end) as PayoutAverage
from bet_transactions
where realm = 'ns2'
and type <> 'playcredit'
group by makerplayerid
order by total desc
limit 50) t
inner join data d on t.makerplayerid = d.datarecordid
where d.datatypename = 'bka' and datavalue not like '%""BKA"":""""%' and datavalue not like '%""BKA"": """"%' and d.datarealm = 'ns2'
order by total desc
limit 25";
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bkaJson = reader.GetString("BkaJson");
                            var total = reader.GetDouble("Total");
                            var payoutRate = reader.GetDouble("PayoutRate");
                            var betCount = reader.GetInt64("BetCount");
                            var betMax = reader.GetDouble("BetMax");
                            var betAverage = reader.GetDouble("BetAverage");
                            var payoutCount = reader.GetInt64("PayoutCount");
                            var payoutMax = reader.GetDouble("PayoutMax");
                            var payoutAverage = reader.GetDouble("PayoutAverage");
                            var bkaData = _bkaDataParser.Parse(bkaJson);
                            var result = new SpecBetsData(new Player(bkaData.Bka, bkaData.PlayerId), total, payoutRate, betCount, betMax, betAverage, payoutCount, payoutMax, payoutAverage);
                            yield return result;
                        }
                    }
                }
            }

        }
    }

    public interface ISpecBetsData
    {
        IPlayer Player { get; }
        double Total { get; }
        double PayoutRate { get; }
        long BetCount { get; }
        double BetMax { get; }
        double BetAverage { get; }
        long PayoutCount { get; }
        double PayoutMax { get; }
        double PayoutAverage { get; }
    }

    public class SpecBetsData : ISpecBetsData
    {
        public SpecBetsData(IPlayer player, double total, double payoutRate, long betCount, double betMax, double betAverage, long payoutCount, double payoutMax, double payoutAverage)
        {
            Player = player;
            Total = total;
            PayoutRate = payoutRate;
            BetCount = betCount;
            BetAverage = betAverage;
            BetMax = betMax;
            PayoutCount = payoutCount;
            PayoutAverage = payoutAverage;
            PayoutMax = payoutMax;
        }

        public double PayoutRate { get; }

        public IPlayer Player { get; }
        public double Total { get; }
        public long BetCount { get; }
        public double BetMax { get; }
        public double BetAverage { get; }
        public long PayoutCount { get; }
        public double PayoutMax { get; }
        public double PayoutAverage { get; }
    }
}