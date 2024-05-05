using Godot;
using System;
using System.Collections.Generic;

public partial class LogContainer : PanelContainer
{
    public Node _parentNode;

    private VBoxContainer _container;
    private Label _nameLabel;

    internal bool Tracking = true;
    private Vector2 _offset;
    private int _alignHorizontal;
    private int _alignVertical;
    
    // Print
    private Label _printsLabel;
    private bool _printDirty;

    private List<string> _printLines = new();
    private int _printScroll;
    private int _printLinesVisibleMax = 8;

    private PackedScene _refText = ResourceLoader.Load<PackedScene>("res://addons/hyperlog/trackers/tracker_text.tscn");

    private PackedScene _refAngle =
        ResourceLoader.Load<PackedScene>("res://addons/hyperlog/trackers/tracker_angle.tscn");

    private PackedScene _refGraph =
        ResourceLoader.Load<PackedScene>("res://addons/hyperlog/trackers/tracker_graph.tscn");

    private PackedScene _refBar = ResourceLoader.Load<PackedScene>("res://addons/hyperlog/trackers/tracker_bar.tscn");

    private PackedScene _refColor =
        ResourceLoader.Load<PackedScene>("res://addons/hyperlog/trackers/tracker_color.tscn");

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("%Container");
        _nameLabel = GetNode<Label>("%Name");
    }

    public override void _Process(double delta)
    {
        if (_parentNode != null && _parentNode != GetTree().Root)
        {
            switch (_parentNode)
            {
                case Node3D parent3D when HyperLog.Camera3D != null:
                    Position = HyperLog.Camera3D.UnprojectPosition(parent3D.GlobalPosition) + _offset;
                    break;
                case Node2D parent2D:
                    Position = parent2D.GetGlobalTransformWithCanvas().Origin + _offset;
                    break;
            }

            switch (_alignHorizontal)
            {
                case (int)HorizontalAlignment.Center:
                    Position = new Vector2 { X = Position.X - (Size.X / 2 * Scale.X), Y = Position.Y };
                    break;
                case (int)HorizontalAlignment.Right:
                    Position = new Vector2 { X = Position.X - (Size.X * Scale.X), Y = Position.Y };
                    break;
                default:
                    Position = Position;
                    break;
            }

            switch (_alignVertical)
            {
                case (int)VerticalAlignment.Center:
                    Position = new Vector2 { X = Position.X, Y = Position.Y - (Size.Y / 2 * Scale.Y) };
                    break;
                case (int)VerticalAlignment.Bottom:
                    Position = new Vector2 { X = Position.X, Y = Position.Y - (Size.Y * Scale.Y) };
                    break;
                default:
                    Position = Position;
                    break;
            }
        }
        else
        {
            RemoveAt();
        }

        if (_printDirty)
        {
            ProcessPrint();
        }
    }

    public void _SetName(string value)
    {
        _nameLabel.Visible = true;
        _nameLabel.Text = value;
    }


    public void Print(object arg1, object arg2 = null, object arg3 = null, object arg4 = null)
    {
        _printsLabel.Visible = true;
        string line = Format(arg1);
        if (arg2 != null) line += ", " + Format(arg2);
        if (arg3 != null) line += ", " + Format(arg3);
        if (arg4 != null) line += ", " + Format(arg4);
        _printLines.Add(line);
        _printDirty = true;
    }

    string Format(object value)
    {
        if (value.Equals(typeof(Color)))
        {
            string hex = ((Color)value).ToHtml();
            return "[color=#" + hex + "]" + hex + "[/color]";
        }

        if (value.Equals(typeof(bool)))
        {
            return "[color=" + (((bool)value) ? "green" : "red") + "]" + value + "[/color]";
        }

        return value.ToString();
    }

    void ProcessPrint()
    {
        _printsLabel.Text = "";
        for (int i = Math.Min(_printLines.Count, _printLinesVisibleMax) - 1; i >= 0; i--)
        {
            _printsLabel.Text += _printLines[_printLines.Count - i - 1] + "\n";
        }
    }

    // Text
    public TrackerText AddText()
    {
        return CreateTracker<TrackerText>(_refText);
    }
    
    public TrackerText Text(string properties, object node = null)
    {
        TrackerText tracker = CreateTracker<TrackerText>(_refText);
        tracker.Track(properties, node);
        return tracker;
    }

    // Angle
    public TrackerAngle AddAngle()
    {
        return CreateTracker<TrackerAngle>(_refText);
    }
    
    public TrackerAngle Angle(string properties = "rotation", object node = null)
    {
        TrackerAngle tracker = CreateTracker<TrackerAngle>(_refAngle);
        tracker.Track(properties, node);
        return tracker;
    }

    // Graph
    public TrackerGraph AddGraph()
    {
        return CreateTracker<TrackerGraph>(_refText);
    }
    
    public TrackerGraph Graph(object properties, object node = null, object rangeMin = null, object rangeMax = null)
    {
        TrackerGraph tracker = CreateTracker<TrackerGraph>(_refGraph);
        tracker._Ready();
        if (rangeMin != null)
        {
            tracker.SetRangeMin((float)rangeMin);
        }

        if (rangeMax != null)
        {
            tracker.SetRangeMax((float)rangeMax);
        }

        tracker.Track(properties, node);
        return tracker;
    }

    // Bar
    public TrackerBar AddBar()
    {
        return CreateTracker<TrackerBar>(_refText);
    }
    
    public TrackerBar Bar(object properties, int rangeMin = 0, int rangeMax = 10, object node = null)
    {
        TrackerBar tracker = CreateTracker<TrackerBar>(_refBar);
        tracker.SetRange(rangeMin, rangeMax);
        tracker.Track(properties, node);
        return tracker;
    }

    // Color
    public TrackerColor AddColor()
    {
        return CreateTracker<TrackerColor>(_refText);
    }
    
    public TrackerColor Color(string properties = "modulate", object node = null)
    {
        TrackerColor tracker = CreateTracker<TrackerColor>(_refColor);
        tracker.Track(properties, node);
        return tracker;
    }

    public TTracker CreateTracker<TTracker>(PackedScene reference) where TTracker : class
    {
        Tracker tracker = reference.Instantiate<Tracker>();
        _container.AddChild(tracker);
        tracker.container = this;
        return tracker as TTracker;
    }

    public LogContainer SetWidth(float value)
    {
        Size = new Vector2
        {
            X = value,
            Y = Size.Y
        };
        return this;
    }

    public LogContainer Offset(Vector2 value)
    {
        _offset = value;
        return this;
    }

    // Cant use VerticalAlignment or HorizontalAlignment constants as defaults
    private LogContainer Align(int horizontal, int vertical)
    {
        // _alignHorizontal = horizontal == null ? (int)HorizontalAlignment.Left : horizontal;
        _alignHorizontal = horizontal;
        _alignVertical = vertical;
        return this;
    }

    private void RemoveAt()
    {
        HyperLog.RemoveLog(_parentNode);
    }

    private LogContainer HideName()
    {
        _nameLabel.Visible = false;
        return this;
    }

    private LogContainer ShowName()
    {
        _nameLabel.Visible = true;
        return this;
    }
}