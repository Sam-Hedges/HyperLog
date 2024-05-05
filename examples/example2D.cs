using Godot;
using System;

public partial class example2D : Sprite2D
{
    // Define a maximum duration for the arrow
    private Vector2 dir = Vector2.Zero;
    private float speed;
    private Vector2 last_pos;

    // Define the distance threshold to draw an arrow
    [Export] private float ArrowDrawDistance = 20f;
    private float accumulatedDistance;

    public override void _Ready()
    {
        last_pos = Position;
        HyperLog.Log(this).Offset(new Vector2(20f, 20f));
        HyperLog.Log(this).Text("speed>%0.1f");
        HyperLog.Log(this).Text("global_position>%0.1v");
        HyperLog.Log(this).Graph("speed");
        HyperLog.Log(this).Graph("last_pos");
        HyperLog.Log(this).Angle("dir");
        HyperLog.Graph("speed", this, 0f, 40f).SetSteps(3);
    }

    public override void _PhysicsProcess(double delta)
    {
        _update_pos(GetGlobalMousePosition(), delta);
    }

    private void _update_pos(Vector2 _new_pos, double delta)
    {
        // Check if accumulated distance exceeds the threshold
        if (accumulatedDistance >= ArrowDrawDistance)
        {
            // Draw the arrow
            HyperLog.SketchArrow(GlobalPosition, dir * 20f, 0.5f, Colors.Tomato); // Adjust duration as needed
            accumulatedDistance = 0f; // Reset accumulated distance
        }
        HyperLog.SketchLine(GlobalPosition, GlobalPosition + dir * (50f + speed * 10f), (float)delta, Colors.CornflowerBlue);
        dir = last_pos.DirectionTo(_new_pos);
        Vector2 old_pos = GlobalPosition;
        GlobalPosition = GlobalPosition.Lerp(_new_pos, (float)delta * 4f);
        speed = (old_pos - GlobalPosition).Length();
        Rotation = dir.Angle() + Mathf.Pi / 2;
        last_pos = GlobalPosition;
        accumulatedDistance += speed;
    }
}
