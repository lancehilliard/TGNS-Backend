using System;
using System.Web;

namespace TGNS.Core.Data
{
    public interface ICurrentUserPlayerIdGetter
    {
        long Get();
    }

    public class CurrentUserPlayerIdGetter : DataAccessor, ICurrentUserPlayerIdGetter
    {
        private readonly IPlayerIdGetter _playerIdGetter;

        public CurrentUserPlayerIdGetter(string connectionString) : base(connectionString)
        {
            _playerIdGetter = new PlayerIdGetter(connectionString);
        }

        public long Get()
        {
            var username = HttpContext.Current.User.Identity.Name;
            var sessionKey = username + "PlayerId";
            var playerIdSession = HttpContext.Current.Session[sessionKey];
            var result = playerIdSession == null ? _playerIdGetter.Get(username) : Convert.ToInt64(playerIdSession);
            HttpContext.Current.Session[sessionKey] = result;
            return result;
        }
    }
}