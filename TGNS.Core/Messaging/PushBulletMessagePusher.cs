using System;
using System.IO;
using System.Net;
using System.Text;

namespace TGNS.Core.Messaging
{
    public class PushbulletMessagePusher : IMessagePusher
    {
        readonly Func<HttpWebResponse, PushSummary, PushSummary> _resultPropertiesSetter = (response, result) =>
        {
            using (var dataStream = response.GetResponseStream())
            {
                if (dataStream != null)
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        result.Output = reader.ReadToEnd();
                    }
                }
            }
            result.ResultCode = (int)response.StatusCode;
            result.ResultDescription = response.StatusDescription;
            return result;
        };
        readonly Func<string, string> _valueEscaper = s => s.Replace(@"""", @"\""");

        public PushbulletMessagePusher(string encodedAuthKey)
        {
            _encodedAuthKey = encodedAuthKey;
        }

        private static readonly string PUSHES_URL = "https://api.pushbullet.com/v2/pushes";
        private readonly string _encodedAuthKey;

        public string PlatformName
        {
            get { return "Pushbullet"; }
        }

        public bool WasSuccessful(PushSummary summary)
        {
            var result = summary.ResultCode == 200;
            return result;
        }

        public PushSummary Push(string channelId, string title, string message)
        {
            var result = new PushSummary { Input = string.Format(@"{{""channel_tag"": ""{0}"", ""type"": ""note"", ""title"": ""{1}"", ""body"": ""{2}""}}", _valueEscaper(channelId), _valueEscaper(title), _valueEscaper(message)) };
            var postData = Encoding.ASCII.GetBytes(result.Input);
            var request = WebRequest.Create(PUSHES_URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", _encodedAuthKey);
            request.ContentLength = postData.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    result = _resultPropertiesSetter(response, result);
                }
            }
            catch (WebException e)
            {
                using (var response = (HttpWebResponse) e.Response)
                {
                    result = _resultPropertiesSetter(response, result);
                }
            }
            return result;
        }
    }
}
