using TGNS.Core.Extensions;

namespace TGNS.Core.Data
{
    public interface ITgJsonFixer
    {
        string Fix(string json);
    }

    public class TgJsonFixer : ITgJsonFixer
    {
        public string Fix(string json)
        {
            var result = json.ReplaceFirstOccurrence("{", "[");
            result = result.ReplaceLastOccurrence("}", "]");
            return result;
        }
    }
}