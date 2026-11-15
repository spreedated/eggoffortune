namespace TheEggOfFortune.Models
{
    internal sealed record AppConfig
    {
        public int TapsLeft { get; set; } = 20000;
        public bool Theme { get; set; }
    }
}
