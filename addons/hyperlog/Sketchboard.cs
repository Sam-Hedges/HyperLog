using Godot;
using System.Collections.Generic;

public partial class Sketchboard : Node2D
{
    public List<List<Variant>> lines = new();
    public List<List<Variant>> circles = new();
    public List<List<Variant>> arrows = new();
    public List<List<Variant>> rects = new();

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        ulong time = Time.GetTicksMsec();

        // LINES
        for (int i = 0; i < lines.Count;)
        {
            var data = lines[i];
            var duration = (float)data[3];
            var color = (Color)data[2];
            var from = (Vector2)data[0];
            var to = (Vector2)data[1];
            if (duration == 0)
            {
                lines.RemoveAt(i);
            }
            else
            {
                var start = (float)data[4];
                color.A *= GetAlpha(start, duration, time);
                if (start + duration < time)
                {
                    lines.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            DrawLine(from, to, color, 1.0f, true);
        }

        // ARROWS
        for (int i = 0; i < arrows.Count;)
        {
            var data = arrows[i];
            var duration = (float)data[3];
            var color = (Color)data[2];
            var to = (Vector2)data[1];
            var from = (Vector2)data[0];

            if (duration == 0)
            {
                arrows.RemoveAt(i);
            }
            else
            {
                var start = (float)data[4];
                color.A *= GetAlpha(start, duration, time);
                if (start + duration < time)
                {
                    arrows.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (from == to)
            {
                DrawCircle(from, 2, color);
            }
            else
            {
                DrawLine(from, to, color, 1.0f, true);
                var angle = from.AngleToPoint(to) + Mathf.Pi;
                DrawLine(to, to + new Vector2(1, 0).Rotated(angle - 0.4f) * 16, color, 1.0f, true);
                DrawLine(to, to + new Vector2(1, 0).Rotated(angle + 0.4f) * 16, color, 1.0f, true);
            }
        }

        // CIRCLES
        for (int i = 0; i < circles.Count;)
        {
            var data = circles[i];
            var duration = (float)data[3];
            var color = (Color)data[2];
            var position = (Vector2)data[0];
            var radius = (float)data[1];
            if (duration == 0)
            {
                circles.RemoveAt(i);
            }
            else
            {
                var start = (float)data[4];
                color.A *= GetAlpha(start, duration, time);
                if (start + duration < time)
                {
                    circles.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            Vector2 prevPoint = new Vector2();
            for (int j = 0; j < 13; j++)
            {
                var point = new Vector2(1, 0).Rotated(j / 12.0f * Mathf.Tau) * radius + position;
                if (prevPoint != Vector2.Zero)
                {
                    DrawLine(prevPoint, point, color, 1.0f, true);
                }
                prevPoint = point;
            }
        }

        // RECTS
        for (int i = 0; i < rects.Count;)
        {
            var data = rects[i];
            var duration = (float)data[2];
            var color = (Color)data[1];
            var rect = (Rect2)data[0];
            if (duration == 0)
            {
                rects.RemoveAt(i);
            }
            else
            {
                var start = (float)data[3];
                color.A *= GetAlpha(start, duration, time);
                if (start + duration < time)
                {
                    rects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            DrawRect(rect, color, false);
        }
    }

    private float GetAlpha(float start, float duration, float current)
    {
        return 1.0f - ((current - start) / duration) * 0.9f;
    }
}
