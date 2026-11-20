using Microsoft.Maui.Graphics;
using System;

namespace TheEggOfFortune.ViewEffects
{
    public sealed class StrikeDrawable : IDrawable
    {
        private static readonly Color _outlineColor = Color.FromArgb("#634124");
        private static readonly Color _shellColor = Color.FromArgb("#E8BE77");
        private static readonly Color _flashColor = Color.FromArgb("#FFE3A3");
        private readonly float _rotation = Random.Shared.Next(-25, 26);
        private readonly PathF _flash = CreateFlash();

        public float Progress { get; set; }

        private static PathF CreateFlash()
        {
            PathF path = new();
            path.MoveTo(0, -17);
            path.LineTo(3, -5);
            path.LineTo(15, -9);
            path.LineTo(6, 1);
            path.LineTo(13, 13);
            path.LineTo(1, 6);
            path.LineTo(-10, 16);
            path.LineTo(-5, 3);
            path.LineTo(-17, -2);
            path.LineTo(-5, -5);
            path.LineTo(-8, -15);
            path.LineTo(-1, -7);
            path.Close();
            return path;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.Antialias = true;
            canvas.Translate(dirtyRect.Center.X, dirtyRect.Center.Y);
            canvas.Rotate(_rotation);
            canvas.StrokeLineCap = LineCap.Round;

            // A short sweep reaches the contact point before the burst.
            if (this.Progress < 0.45f)
            {
                float sweep = Math.Clamp(this.Progress / 0.3f, 0, 1);
                float head = -24 + 42 * sweep;
                canvas.Alpha = 1 - Math.Clamp((this.Progress - 0.3f) / 0.15f, 0, 1);
                canvas.StrokeColor = _outlineColor;
                canvas.StrokeSize = 7;
                canvas.DrawLine(head - 13, head - 13, head, head);
                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize = 3;
                canvas.DrawLine(head - 13, head - 13, head, head);
            }

            if (this.Progress >= 0.17f)
            {
                float impact = Math.Clamp((this.Progress - 0.17f) / 0.83f, 0, 1);
                float flight = 1 - MathF.Pow(1 - impact, 3);
                float radius = 6 + 28 * flight;
                float length = 2 + 6 * (1 - impact);
                canvas.Alpha = (1 - impact) * (1 - impact);

                // Small fragments shoot outward and fall as they fade.
                for (int i = 0; i < 6; i++)
                {
                    float angle = i * MathF.Tau / 6 + 0.25f;
                    float dx = MathF.Cos(angle);
                    float dy = MathF.Sin(angle);
                    float x = dx * radius;
                    float y = dy * radius + 10 * impact * impact;
                    canvas.StrokeColor = _outlineColor;
                    canvas.StrokeSize = 4;
                    canvas.DrawLine(x, y, x + dx * length, y + dy * length);
                    canvas.StrokeColor = i % 2 == 0 ? _shellColor : Colors.White;
                    canvas.StrokeSize = 2;
                    canvas.DrawLine(x, y, x + dx * length, y + dy * length);
                }

                // The flash pops open, then contracts instead of rippling.
                float flash = Math.Clamp(impact / 0.6f, 0, 1);
                float scale = 0.55f + 0.65f * MathF.Sin(flash * MathF.PI);
                canvas.Alpha = 1 - flash;
                canvas.Scale(scale, scale);
                canvas.FillColor = _flashColor;
                canvas.FillPath(_flash);
                canvas.StrokeColor = _outlineColor;
                canvas.StrokeSize = 1.5f;
                canvas.DrawPath(_flash);
            }

            canvas.RestoreState();
        }
    }
}
