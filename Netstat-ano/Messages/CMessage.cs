namespace Netstat_ano.Messages
{
    public class CMessage
    {
        public object? Sender { get; set; }

        public string? Tag { get; set; }

        public object? Content { get; set; }

        public CMessage(object? sender)
        {
            Sender = sender;
        }

        public CMessage(object? sender, string? tag)
        {
            Sender = sender;
            Tag = tag;
        }
    }
}
