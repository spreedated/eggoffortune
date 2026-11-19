using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using TheEggOfFortune.Views;

namespace TheEggOfFortune.Logic
{
    public static class DeveloperService
    {
        private static readonly TapCode[] _cheatCode =
            [
                TapCode.Theme,
                TapCode.Theme,
                TapCode.Theme,
                TapCode.Theme,
                TapCode.Theme,
                TapCode.Egg,
                TapCode.Egg,
                TapCode.Counter,
                TapCode.Counter,
                TapCode.Counter,
                TapCode.Counter,
                TapCode.Counter,
                TapCode.Theme,
                TapCode.Theme
            ];

        private static readonly List<TapCode> _tapHistory = [];

        public enum TapCode
        {
            Egg,
            Theme,
            Counter
        }

        private static void CheckForCheatCode()
        {
            if (!_tapHistory.SequenceEqual(_cheatCode.Take(_tapHistory.Count)))
            {
                _tapHistory.RemoveAt(0);
                return;
            }

            if (_tapHistory.Count >= _cheatCode.Length)
            {
                if (_cheatCode.SequenceEqual(_tapHistory))
                {
                    Application.Current.Windows[0].Page.ShowPopup(new DevScreen());
                }

                _tapHistory.Clear();
            }
        }

        public static void AddTap(TapCode tapCode)
        {
            _tapHistory.Add(tapCode);
            CheckForCheatCode();
        }
    }
}
