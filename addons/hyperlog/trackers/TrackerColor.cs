using Godot;
using System.Collections.Generic;

public partial class TrackerColor : Tracker
{
    private Label label;

    List<string> backlog = new List<string>();

    private readonly int max_lines = 2048;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    public override void _Process(double delta)
    {
        if (!container.Tracking) return;
        foreach (ValueMonitor tracker in monitors)
        {
            backlog.Insert(0, $"{tracker.property}\t{tracker.Format(tracker.GetValue())}");
        }

        while (backlog.Count > max_lines)
        {
            backlog.RemoveAt(backlog.Count - 1);
        }

        label.Text = string.Join("\n", backlog);
    }
}