using Godot;
using System;

public partial class TrackerAngle : Tracker
{
    private Array values;

    private float scroll_speed = 1.0f;

    private int steps = 20;

    public TrackerAngle()
    {
        SetHeight(80);
    }

    protected override ValueMonitor AddTracker(string property = "rotation", Node node = null)
    {
        return base.AddTracker(property, node);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!container.Tracking) return;
        TrackersStoreValue();
    }

    public override void _Process(double delta)
    {
        if (!container.Tracking) return;
        QueueRedraw();
    }

    public override void _Draw()
    {
        float radius = CustomMinimumSize.Y / 2;
        Vector2 center = new Vector2(radius, radius);

        for (int j = 0; j < monitors.Count; j++)
        {
            ValueMonitor monitor = monitors[j];
            for (int i = 1; i < monitor.backlog.Count - steps - 1; i += steps)
            {
                float float_a = 0, float_b = 0;
                if (monitor.backlog[i].Obj is Vector2)
                {
                    float_a = ((Vector2)monitor.backlog[i]).Angle();
                    float_b = ((Vector2)monitor.backlog[i + steps]).Angle();
                }
                Color color = HyperLog.colors[j];
                color.A = (1 - i / (float)monitor.MaxLogLength) * 0.6f;

                DrawArch(new Vector2(float_a, radius / (1f + (i / (350f / scroll_speed)))), 
                         new Vector2(float_b, radius / (1f + ((i + steps) / (350f / scroll_speed)))), 
                         color, center);
            }
        }

        for (int j = 0; j < monitors.Count; j++)
        {
            ValueMonitor monitor = monitors[j];
            if (monitor.backlog.Count > 0)
            {
                float current_angle = 0f;
                if (monitor.backlog[0].Obj is Vector2)
                {
                    current_angle = ((Vector2)monitor.backlog[0]).Angle();
                }

                DrawLine(center, Vector2.Right.Rotated(current_angle) * radius + center, HyperLog.colors[j], 2, true);
            }
        }

        DrawCircle(center, radius, new Color(1, 1, 1, 0.1f));
    }

    void DrawArch(Vector2 from, Vector2 to, Color color, Vector2 center)
    {
        float step_size = 0.4f;
        float total_arch = AngleDifference(from.X, to.X);

        if (Math.Abs(total_arch) < step_size)
        {
            DrawLine(Vector2.Right.Rotated(from.X) * from.Y + center,
                     Vector2.Right.Rotated(to.X) * to.Y + center,
                     color, -1, true);
        }
        else
        {
            float current_angle = from.X;
            float previous_angle = from.X;
            float current_radius = from.Y;
            float previous_radius = from.Y;
            bool doLoop = true;

            while (doLoop)
            {
                if (Math.Abs(AngleDifference(current_angle, to.X)) < step_size)
                {
                    current_angle = to.X;
                    current_radius = to.Y;
                    doLoop = false;
                }
                else
                {
                    current_angle = AngleTowards(current_angle, to.X, step_size);
                    current_radius = from.Y;
                    DrawLine(Vector2.Right.Rotated(current_angle) * current_radius + center,
                             Vector2.Right.Rotated(previous_angle) * previous_radius + center,
                             color, -1, true);
                }

                previous_angle = current_angle;
                previous_radius = current_radius;
            }
        }
    }

    float AngleDifference(float a, float b)
    {
        return FPosMod(b - a + Mathf.Pi, Mathf.Pi * 2) - Mathf.Pi;
    }

    float AngleTowards(float from, float to, float delta)
    {
        return from + Mathf.Sign(AngleDifference(from, to)) * delta;
    }

    protected sealed override Tracker SetHeight(float value)
    {
        CustomMinimumSize = new Vector2
        {
            X = value,
            Y = CustomMinimumSize.Y
        };
        return base.SetHeight(value);
    }

    float FPosMod(float a, float b)
    {
        return (a % b + b) % b;
    }
}
