using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using TGNS.Core.Data;

namespace TGNS.Portal.Classes
{
    public interface IPrimerSignersGetter
    {
        IEnumerable<IPlayer> Get();
    }

    public class PrimerSignersGetter : IPrimerSignersGetter
    {
        public static void Main()
        {
            var primerSignaturesGetter = new PrimerSignersGetter();
            var primerSignatures = primerSignaturesGetter.Get();
        }

        public IEnumerable<IPlayer> Get()
        {
            var result = new List<IPlayer>();
            var beforeName = "&lt;readandagree";
            var afterId = "&lt;/readandagree&gt;";
            var pageNumber = 0;

            var lastFetchedReadAndAgreeWithIdLines = new List<string>();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            while (pageNumber < int.MaxValue)
            {
                string downloadString;
                using (var webClient = new WebClient())
                {
                    pageNumber = pageNumber + 1;
                    string url = $"https://www.tacticalgamer.com/forum/action/natural-selection/natural-selection-general-discussion/122749-read-and-sign-the-tgns-primer/page{pageNumber}";
                    downloadString = webClient.DownloadString(url);
                }
                var lines = downloadString.Split(new[] { '\n' }, StringSplitOptions.None);
                var readAndAgreeLines = lines.Where(x => x.Contains(beforeName)).Where(x => x.Contains(afterId)).ToList();
                if (readAndAgreeLines.SequenceEqual(lastFetchedReadAndAgreeWithIdLines))
                {
                    break;
                }
                var readAndAgreeWithIdLines = readAndAgreeLines.Where(x => x.Contains("withid") && !x.Contains("99999999")).ToList();
                foreach (var readAndAgreeWithIdLine in readAndAgreeWithIdLines)
                {
                    var readAndAgreeWithIdLineParsed = Regex.Replace(HttpUtility.HtmlDecode(readAndAgreeWithIdLine), "<.*?>", "\t");
                    var parts = readAndAgreeWithIdLineParsed.Split(new[] { "\t" }, StringSplitOptions.None).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    if (parts.Count == 2)
                    {
                        var name = parts[0];
                        long playerId;
                        if (long.TryParse(parts[1], out playerId))
                        {
                            result.RemoveAll(x => playerId.Equals(x.PlayerId));
                            var player = new Player(name, playerId);
                            result.Add(player);
                        }
                    }
                }
                lastFetchedReadAndAgreeWithIdLines = readAndAgreeLines;
            }
            return result;
        }
    }
}