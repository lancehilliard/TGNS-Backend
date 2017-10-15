using System.Collections.Generic;
using System.Linq;

namespace TGNS.Core.Data
{
    public interface ICameraViewNamesGetter
    {
        IEnumerable<string> GetCameraViewNames(string serverName, double startTimeSeconds);
    }

    public class CameraViewNamesGetter : DataAccessor, ICameraViewNamesGetter
    {
        private readonly IPlayedGamesGetter _playedGamesGetter;
        private readonly IGameRecordingsGetter _gameRecordingsGetter;


        public CameraViewNamesGetter(string connectionString) : base(connectionString)
        {
            _playedGamesGetter = new PlayedGamesGetter(connectionString);
            _gameRecordingsGetter = new GameRecordingsGetter(connectionString);
        }

        public IEnumerable<string> GetCameraViewNames(string serverName, double startTimeSeconds)
        {
            var result = new List<string>();
            var gameRecordings = _gameRecordingsGetter.Get(serverName, startTimeSeconds);
            foreach (var playedGame in gameRecordings.Select(gameRecording => _playedGamesGetter.Get("ns2", gameRecording.PlayerId).SingleOrDefault(x => x.ServerName.Equals(serverName) && x.StartTimeSeconds.Equals(startTimeSeconds))))
            {
                string cameraViewName;
                if (playedGame != null)
                {
                    var team = playedGame.MarineSeconds > playedGame.AlienSeconds ? "M" : "A";
                    var comm = playedGame.CommanderSeconds > playedGame.DurationInSeconds / 2 ? "C" : string.Empty;
                    cameraViewName = $"{team}{comm}";
                }
                else
                {
                    cameraViewName = "S";
                }
                result.Add(cameraViewName);
            }
            return result;
        }
    }
}