namespace TGNS.Core.Messaging
{
    public struct PushSummary
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int ResultCode { get; set; }
        public string ResultDescription { get; set; }
    }
}