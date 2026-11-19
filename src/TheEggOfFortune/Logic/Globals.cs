using LogicLayer.Models;
using Microsoft.Maui.Controls;
using neXn.Lib.ConfigurationHandler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private static readonly SemaphoreSlim _saveGate = new(1, 1);

        internal static async Task<bool> SaveConfigurationAsync(bool skipIfBusy = false)
        {
            int timeout = skipIfBusy ? 0 : Timeout.Infinite;

            if (!await _saveGate.WaitAsync(timeout).ConfigureAwait(false))
            {
                return false;
            }

            try
            {
                await Configuration.SaveAsync().ConfigureAwait(false);
                return true;
            }
            finally
            {
                _saveGate.Release();
            }
        }

        public static ScoreService ScoreService { get; } = new();
    }
}
