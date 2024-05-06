using Godot;
using System.Collections.Generic;

public partial class LogPrint : Tracker
{
    private Label _label;

    private List<string> _backlog = new();

    private const int MaxLines = 1024;

    public override void _Ready()
    {
        _label = GetNode<Label>("%Label");
    }

    public override void _Process(double delta)
    {
        if (!_insideViewport) return;
        foreach (ValueMonitor monitor in monitors)
        {
            _backlog.Insert(0, $"{monitor.property}\t{monitor.Format(monitor.GetValue())}");
        }
        while (_backlog.Count > MaxLines)
        {
            _backlog.RemoveAt(_backlog.Count - 1);
        }

        _label.Text = "";
        foreach (string line in _backlog)
        {
            _label.Text += line + "\n";
        }
    }
}