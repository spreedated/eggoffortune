using LogicLayer.Models;
using Microsoft.Maui.Controls;
using neXn.Lib.ConfigurationHandler;
using System.Collections.Generic;
using TheEggOfFortune.Models;

namespace TheEggOfFortune.Logic
{
    internal static class Globals
    {
        public enum EggState
        {
            Intact,
            Broken,
            Halfed
        }
        public static ConfigurationHandler<AppConfig> Configuration { get; set; }
        public static Dictionary<EggState, ImageSource> EggstateImage { get; } = new()
        {
            { EggState.Intact, "egg_transparent_1024.png" },
            { EggState.Broken, "egg_transparent_broken_1024.png" },
            { EggState.Halfed, "egg_transparent_bottom_1024.png" }
        };

        public static List<Phrase> Phrases { get; } = [];
    }
}
