namespace LogicLayer.Models
{
    public sealed record Phrase
    {
        public string Text { get; set; }
        public bool Used { get; set; }
    }
}
