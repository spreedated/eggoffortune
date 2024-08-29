namespace TheEggOfFortune.Models
{
    internal record AppConfig
    {
        public int TapsLeft { get; set; } = 20000;
        public bool Theme { get; set; }
    }
}
