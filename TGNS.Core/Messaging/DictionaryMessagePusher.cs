using System.Collections.Generic;
using Newtonsoft.Json;

namespace TGNS.Core.Messaging
{
    public interface IDictionaryMessagePusher
    {
        IDictionary<string, object> Push(string channelId, string title, string message);
    }

    public class DictionaryMessagePusher : IDictionaryMessagePusher
    {
        private readonly IMessagePusher _messagePusher;


        public DictionaryMessagePusher(IMessagePusher messagePusher)
        {
            _messagePusher = messagePusher;
        }

        public IDictionary<string, dynamic> Push(string channelId, string title, string message)
        {
            var pushResponse = _messagePusher.Push(channelId, title, message);
            var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(pushResponse.Output);
            return result;
        }
    }
}