using Godot;
using System;
using System.Collections.Generic;

public partial class TrackerBar : Tracker
{
    int range_min = 0;
    int range_max = 10;

    private List<ProgressBar> bars;
    private List<Label> labels;

    public override void _Process(double delta)
    {
        if (!container.Tracking) return;
        for (int i = 0; i < monitors.Count; i++)
        {
            ValueMonitor monitor = monitors[i];
            double value = (double)monitor.GetValue();
            ProgressBar bar = bars[i];
            bar.Value = value;
        }
    }

    protected override ValueMonitor AddTracker(string property, Node node)
    {
        ValueMonitor monitor = base.AddTracker(property, node);
        Label label = new Label();
        AddChild(label);
        label.Text = monitor.propertyName;
        labels.Add(label);

        ProgressBar bar = new ProgressBar();
        bar.CustomMinimumSize = new Vector2(0, 20);
        bar.MinValue = range_min;
        bar.MaxValue = range_max;
        bar.ShowPercentage = false;
        AddChild(bar);
        bars.Add(bar);
        return monitor;
    }

    public TrackerBar SetRange(int value_min, int value_max)
    {
        range_min = value_min;
        range_max = value_max;

        foreach (ProgressBar bar in bars)
        {
            bar.MinValue = range_min;
            bar.MaxValue = range_max;
        }
        return this;
    }

    public TrackerBar HideLabels()
    {
        foreach (Label label in labels)
        {
            label.Visible = false;
        }
        return this;
    }

    public TrackerBar ShowLabels()
    {
        foreach (Label label in labels)
        {
            label.Visible = true;
        }
        return this;
    }
}