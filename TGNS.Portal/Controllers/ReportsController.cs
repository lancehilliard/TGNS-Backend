using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace TGNS.Portal.Controllers
{
    public class ReportsController : AdminController
    {
        public ActionResult Index()
        {
            //var actionNames = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(type => typeof (Controller).IsAssignableFrom(type) && type.Name == "ReportsController") //filter controllers
            //    .SelectMany(type => type.GetMethods())
            //    .Where(method => method.IsPublic && !method.IsDefined(typeof (NonActionAttribute)) && method.ReturnType.Name == "ActionResult" && method.Name != "Index").Select(x=>x.Name);
            return View();
        }

        public ActionResult Revenue()
        {
            return GetViewer("Revenue Per Month", command =>
            {
                command.CommandText = "select DATE_FORMAT(RowCreated,'%Y-%m') as Month, count(distinct playerid) as SMs, count(distinct playerid) * 7 as MonthRevenue, -150 + (count(distinct playerid) * 7) as DiffFrom150 from games_players where 1 = 1 and SupportingMember = 1 group by Month order by Month desc;";
            });
        }

        public ActionResult Games()
        {
            return GetViewer("Games", command =>
            {
                command.CommandText = "select * from games where rowcreated > @RowCreatedMinimum and realm = 'ns2' and gamemode = 'ns2' order by rowcreated desc limit 100;";
                command.Parameters.AddWithValue("@RowCreatedMinimum", DateTime.Now.AddDays(-30));
            });
        }

        public ActionResult NotificationSubscriptions()
        {
            return GetViewer("Notification Subscriptions", command =>
            {
                command.CommandText = "select offeringname as Name, count(distinct playerid) as Count from notifications_subscriptions group by offeringname limit 100;";
            });
        }

        public ActionResult BadgesEarned()
        {
            return GetViewer("Badges Earned", command =>
            {
                command.CommandText = "select * from achievements_badges_players order by rowcreated desc limit 100;";
            });
        }
        public ActionResult AverageGameDurationInMinutes()
        {
            return GetViewer("Average Game Duration in Minutes", command =>
            {
                command.CommandText = "select count(durationinseconds) as Count, avg(durationinseconds)/60 as AverageDurationInMinutes from games where MapName in ('ns2_summit','ns2_eclipse','ns2_derelict','ns2_tram','ns2_veil','ns2_descent','ns2_refinery','ns2_biodome','ns2_mineshaft','ns2_kodiak','ns2_docking','ns2_caged') and includedbots = 0 and realm = 'ns2' and gamemode = 'ns2';";
            });
        }

        public ActionResult AverageGameDurationInMinutesPerBuild()
        {
            return GetViewer("Average Game Duration in Minutes, Per Build (With WinOrLose Rate)", command =>
            {
                command.CommandText = "select distinct BuildNumber, count(durationinseconds) as Count, avg(durationinseconds)/60 as AverageDurationInMinutes, SUM(CASE WHEN SurrenderTeamNumber IS NOT NULL THEN 1 ELSE 0 END) / COUNT(1) as WinOrLoseRate from games where MapName in ('ns2_summit','ns2_eclipse','ns2_derelict','ns2_tram','ns2_veil','ns2_descent','ns2_refinery','ns2_biodome','ns2_mineshaft','ns2_kodiak','ns2_docking', 'ns2_caged') and includedbots = 0 and realm = 'ns2' and gamemode = 'ns2' group by buildnumber order by buildnumber desc;";
            });
        }

        public ActionResult GameRecordings()
        {
            return GetViewer("Game Recordings", command =>
            {
                command.CommandText = "select * from games_recordings limit 100;";
            });
        }

        public ActionResult KarmaDecayByPlayer()
        {
            return GetViewer("Karma Decay by Player", command =>
            {
                command.CommandText = "select karmaplayerid AS Player, COALESCE(SUM(KarmaDelta),0) AS Total FROM karma where KarmaRealm = 'ns2' and KarmaDeltaName = 'Decay' group by karmaplayerid order by Total limit 100;";
            });
        }

        public ActionResult Karma()
        {
            return GetViewer("Karma", command =>
            {
                command.CommandText = "select * from karma where karmadeltaname <> 'Decay' order by rowcreated desc limit 100;";
            });
        }

        public ActionResult GameFeedback()
        {
            return GetViewer("Game Feedback", command =>
            {
                command.CommandText = "SELECT * FROM tgns.games_feedback order by rowcreated desc limit 100;";
            });
        }

        public ActionResult KarmaTotals()
        {
            return GetViewer("Karma Totals", command =>
            {
                command.CommandText = "SELECT karmaplayerid, sum(karmadelta) as total FROM karma group by karmaplayerid order by total desc limit 100;";
            });
        }

        public ActionResult PrimerSignersCountOverLastMonth()
        {
            return GetViewer("Primer Signers Count Over Last Month", command =>
            {
                command.CommandText = "select count(distinct playerid) as Count from games_players where primersignerwithgames = 1 and rowcreated > @RowCreatedMinimum;";
                command.Parameters.AddWithValue("@RowCreatedMinimum", DateTime.Now.AddDays(-30));
            });
        }

        public ActionResult EarnedCountPerBadge()
        {
            return GetViewer("Earned Count Per Badge", command =>
            {
                command.CommandText = "SELECT `b`.`BadgeDisplayName` AS `Badge`, `b`.`BadgeDescription` AS `Description`, COUNT(1) AS `Total Earned` FROM (`achievements_badges_players` `bp` JOIN `achievements_badges` `b` ON ((`b`.`BadgeID` = `bp`.`BadgeID`))) WHERE (`b`.`AchievementsRealm` = 'ns2') GROUP BY `b`.`BadgeID` ORDER BY 3 DESC;";
            });
        }

        public ActionResult ApproversTopTwentyLastTwoWeeks()
        {
            return GetViewer("Approvals: Top 20 Approvers Last Two Weeks", command =>
            {
                command.CommandText = "SELECT `approvals`.`SourcePlayerId` AS `sourceplayerid`, COUNT(0) AS `Count` FROM `approvals` WHERE (`approvals`.`RowCreated` > (NOW() - INTERVAL 2 WEEK)) GROUP BY `approvals`.`SourcePlayerId` ORDER BY `Count` DESC LIMIT 20;";
            });
        }

        public ActionResult ApprovedTopTwentyLastTwoWeeks()
        {
            return GetViewer("Approvals: Top 20 Approved Last Two Weeks", command =>
            {
                command.CommandText = "SELECT `approvals`.`TargetPlayerId` AS `targetplayerid`, COUNT(0) AS `Count` FROM `approvals` WHERE (`approvals`.`RowCreated` > (NOW() - INTERVAL 2 WEEK)) GROUP BY `approvals`.`TargetPlayerId` ORDER BY `Count` DESC LIMIT 20;";
            });
        }

        public ActionResult ApprovalsPerDayLastTwoWeeks()
        {
            return GetViewer("Approvals: Count Per Day Last Two Weeks", command =>
            {
                command.CommandText = "SELECT DATE_FORMAT(`approvals`.`RowUpdated`, '%Y%m%d') AS `Date`, COUNT(1) AS `Approvals` FROM `approvals` GROUP BY DATE_FORMAT(`approvals`.`RowUpdated`, '%Y%m%d') ORDER BY 1 DESC LIMIT 14;";
            });
        }

        public ActionResult BadgesList()
        {
            return GetViewer("Badges List", command =>
            {
                command.CommandText = "SELECT CONCAT(`b`.`BadgeID`, '. ', `b`.`BadgeDisplayName`, ': ', `b`.`BadgeDescription`) AS `Badge` FROM (`achievements_badges` `b` JOIN `achievements_badges_levels` `bl` ON (((`b`.`AchievementsRealm` = 'ns2') AND (`b`.`BadgeLevelID` = `bl`.`LevelID`)))) ORDER BY `b`.`BadgeID`;";
            });
        }

        public ActionResult RecentSmPrimerSigners()
        {
            return GetViewer("Recent SM Primer Signers", command =>
            {
                command.CommandText = "select x.playerid, x.gamesCount, d.datavalue from data d inner join (select playerid, count(*) as gamesCount from games_players where 1=1 and supportingmember = 1 and PrimerSignerWithGames = 1 and rowcreated > date_sub(current_date(), interval 30 day) group by playerid) x on d.DataRecordId = x.playerid and d.DataTypeName = 'bka' and datarealm = 'ns2' order by x.gamesCount desc limit 50;";
            });
        }

        public ActionResult RecentNonSmPrimerSigners()
        {
            return GetViewer("Recent Non-SM Primer Signers", command =>
            {
                command.CommandText = "select x.playerid, x.gamesCount, d.datavalue from data d inner join (select playerid, count(*) as gamesCount from games_players where 1=1 and supportingmember = 0 and PrimerSignerWithGames = 1 and playerid not in (select distinct playerid from games_players where supportingmember = 1 and rowcreated > date_sub(current_date(), interval 30 day)) and rowcreated > date_sub(current_date(), interval 30 day) group by playerid) x on d.DataRecordId = x.playerid and d.DataTypeName = 'bka' and datarealm = 'ns2' order by x.gamesCount desc limit 50;";
            });
        }

        public ActionResult CaptainsNightGamesCountPerPlayer()
        {
            return GetViewer("Captains Night Games Count Per Player", command =>
            {
                command.CommandText = "select x.playerid, x.CaptainsNightGamesCount, d.DataValue from (select playerid, count(*) as CaptainsNightGamesCount from games_players gp inner join games g on g.servername = gp.servername and g.StartTimeSeconds = gp.starttimeseconds where g.CaptainsMode = 1 and gp.rowcreated > @RowCreatedMinimum and ((dayofweek(gp.rowcreated) = 6 and hour(gp.rowcreated) >= 19) or (dayofweek(gp.rowcreated) = 7 and hour(gp.rowcreated) < 6)) group by PlayerId) x inner join data d on d.datarecordid = x.playerid and d.datatypename = 'bka' and d.datarealm = 'ns2' order by x.CaptainsNightGamesCount desc;";
                command.Parameters.AddWithValue("@RowCreatedMinimum", new DateTime(2000, 1, 1));
            });
        }

        public ActionResult CumulativeScoreByPlayer()
        {
            return GetViewer("Cumulative Score By Player", command =>
            {
                command.CommandText = @"select *
from (select sum(score) as Sum, count(1) as Count, playerid
from games_players gp
inner join games g on g.servername = gp.servername and gp.starttimeseconds = g.starttimeseconds
where g.includedbots = 0
and g.realm = 'ns2' 
and g.gamemode = 'ns2'
group by playerid) x
where Count > 50
order by Sum/Count desc;";
            });
        }

        public ActionResult AverageScoreByPlayer()
        {
            return GetViewer("Average Score By Player", command =>
            {
                command.CommandText = @"select *
from (select sum(score) / count(1) as Average, count(1) as Count, playerid
from games_players gp
inner join games g on g.servername = gp.servername and gp.starttimeseconds = g.starttimeseconds
where g.includedbots = 0
and g.realm = 'ns2' 
and g.gamemode = 'ns2'
group by playerid) x
where Count > 50
order by Average desc;";
            });
        }

        public ActionResult MarineWinRates()
        {
            return GetViewer("Marine Win Rates", command =>
            {
                command.CommandText = @"SELECT 
    BuildNumber,
    MinMin,
    Total,
    MarinesWon,
    MarinesWon / Total AS MarineWinRatio
FROM
    (SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 5
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 5
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            5 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 5
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 5
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            5 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 10
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 10
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            10 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 10
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 10
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            10 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 15
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 15
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            15 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 15
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 15
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            15 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 20
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 20
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            20 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 20
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 20
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            20 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 30
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 30
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            30 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 30
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 30
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            30 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 45
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 45
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber)
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber)
                FROM
                    games) AS BuildNumber,
            45 AS MinMin
UNION
SELECT 
        (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 45
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS Total,
            (SELECT 
                    COUNT(1)
                FROM
                    games
                WHERE
                    WinningTeamNumber = 1
                        AND IncludedBots = 0
                        AND MapName NOT LIKE '%arclight%'
                        AND DurationInSeconds / 60 >= 45
                        AND BuildNumber = (SELECT 
                            MAX(buildnumber) - 1
                        FROM
                            games)) AS MarinesWon,
            (SELECT 
                    MAX(buildnumber) - 1
                FROM
                    games) AS BuildNumber,
            45 AS MinMin
    ) x;";
            });
        }

        public ActionResult CaptainsNightTotalCaptainsGamesCount()
        {
            return GetViewer("Captains Night Total Captains Games Count", command =>
            {
                command.CommandText = @"select count(1) as Count from games where CaptainsMode = 1 and ((DAYOFWEEK(rowcreated) = 6 and HOUR(rowcreated) >= 19) or (DAYOFWEEK(rowcreated) = 7 and HOUR(rowcreated) <= 6)) and DurationInSeconds >= 60;";
            });
        }

        /**/

        public string ConnectionString => ConfigurationManager.ConnectionStrings["Data"].ConnectionString;

        ActionResult GetViewer(string title, Action<MySqlCommand> commandAction)
        {
            var dataTable = new DataTable();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    commandAction(command);
                    command.Prepare();
                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            
            return View("Viewer", new ReportViewModel { Title = title, Results = dataTable });
        }

    }

    public class ReportViewModel
    {
        public DataTable Results { get; set; }
        public string Title { get; set; }
    }
}