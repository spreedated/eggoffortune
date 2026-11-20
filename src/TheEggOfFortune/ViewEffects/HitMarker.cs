using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;

namespace TheEggOfFortune.ViewEffects
{
    public static class HitMarker
    {
        public static async Task ShowHitMarkerAsync(AbsoluteLayout hitMarkerLayer, Point position)
        {
            const double size = 112;
            const uint duration = 220;
            const string animationName = "HitStrike";

            StrikeDrawable strike = new();
            GraphicsView marker = new()
            {
                WidthRequest = size,
                HeightRequest = size,
                Drawable = strike,
                InputTransparent = true,
                BackgroundColor = Colors.Transparent
            };

            AbsoluteLayout.SetLayoutBounds(marker, new Rect(position.X - size / 2, position.Y - size / 2, size, size));

            TaskCompletionSource<bool> finished = new(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnMarkerUnloaded(object sender, EventArgs e)
            {
                marker.AbortAnimation(animationName);
                finished.TrySetResult(false);
            }

            marker.Unloaded += OnMarkerUnloaded;
            hitMarkerLayer.Children.Add(marker);

            try
            {
                new Animation(progress =>
                {
                    strike.Progress = (float)progress;
                    marker.Invalidate();
                }).Commit(marker, animationName, rate: 16, length: duration, easing: Easing.Linear, finished: (_, cancelled) => finished.TrySetResult(!cancelled));

                await finished.Task;
            }
            finally
            {
                marker.Unloaded -= OnMarkerUnloaded;
                marker.AbortAnimation(animationName);
                hitMarkerLayer.Children.Remove(marker);
            }
        }
    }
}
