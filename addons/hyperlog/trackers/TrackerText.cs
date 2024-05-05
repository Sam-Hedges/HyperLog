using Godot;
using System;

public partial class TrackerText : Tracker
{
    private Control nameContainer;
    private Control valueContainer;

    public override void _Ready()
    {
        nameContainer = GetNode<Control>("NameContainer");
        valueContainer = GetNode<Control>("ValueContainer");
    }

    public override void _Process(double delta)
    {
        if (!container.Tracking) return;
        for (int i = 0; i < monitors.Count; i++)
        {
            ValueMonitor monitor = monitors[i];
            Label labelName = (Label)nameContainer.GetChild(i);
            Label labelValue = (Label)valueContainer.GetChild(i);

            string propertyName = monitor.propertyName;
            object value = monitor.Format(monitor.GetValue());
            labelName.Text = propertyName;
            labelValue.Text = value.ToString();
            if (value is bool)
            {
                labelValue.Modulate = (bool)value ? Colors.GreenYellow : Colors.Tomato;
            }
            else
            {
                labelValue.Modulate = Colors.White;
            }
        }
    }

    protected override ValueMonitor AddTracker(string property, Node node = null)
    {
        _AddLabel(nameContainer);
        _AddLabel(valueContainer);
        return base.AddTracker(property, node);
    }

    private void _AddLabel(Control parent)
    {
        Label label = new Label();
        parent.AddChild(label);
    }

    protected override void RemoveTracker(ValueMonitor monitor)
    {
        nameContainer.RemoveChild(nameContainer.GetChild(0));
        valueContainer.RemoveChild(valueContainer.GetChild(0));
        base.RemoveTracker(monitor);
    }
}