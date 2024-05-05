using Godot;
using System.Collections.Generic;

public partial class HyperLog : Node
{
    private static HyperLog _instance;
    private static PackedScene _containerRef;
    private static LogContainer _mainLog;
    private static CanvasLayer _canvas;
    private static Sketchboard _sketchboard;
    public static List<Color> colors = new();
    internal static Camera3D Camera3D = null;

    private static List<LogContainer> _containers = new();

    private static Color _defaultColor = Colors.Red;
        
    public override void _Ready()
    {
        if (_instance != null) QueueFree();
        _instance = this;
        
        _containerRef = ResourceLoader.Load<PackedScene>("res://addons/hyperlog/log_container.tscn");
        _mainLog = GetNode<LogContainer>("%MainLog");
        _canvas = GetNode<CanvasLayer>("%Canvas");
        _sketchboard = GetNode<Sketchboard>("%Sketchboard");
        
        _mainLog.Hide();
        _mainLog._parentNode = GetTree().Root;

        for (int i = 0; i < 64; i++)
        {
            Color color = Colors.Red;
            color.H += i * (1 / 4.0f + 1 / 32.0f);
            color.V *= 1 - (i / 128.0f) * 0.5f;
            colors.Add(color);
        }
    }

    // TODO: Add a wrapper to Log or make the class static to include 'this' to Node node paramter
    // TODO: This will allow other node classes to call Log(this) without 'this': Log()
    public static LogContainer Log(Node node, params Variant[] prints)
    {
        LogContainer container = GetContainer(node);
        if (prints.Length > 0)
            container.Print(prints);

        container.Scale = Vector2.One * 0.75f;
        return container;
    }

    public static void RemoveLog(Node node)
    {
        for (int i = 0; i < _containers.Count; i++)
        {
            if (_containers[i].GetParent() == node)
            {
                _containers[i].QueueFree();
                _containers.RemoveAt(i);
                return;
            }
        }
    }

    private static void PauseLog(Node node)
    {
        foreach (LogContainer container in _containers)
            if (container.GetParent() == node)
            {
                container.Tracking = false;
                return;
            }
    }

    public static void ContinueLog(Node node)
    {
        foreach (LogContainer container in _containers)
            if (container.GetParent() == node)
            {
                container.Tracking = true;
                return;
            }
    }

    private static LogContainer GetContainer(Node node)
    {
        foreach (LogContainer container in _containers)
            if (container._parentNode == node)
                return container;
    
        LogContainer newContainer = _containerRef.Instantiate() as LogContainer; 
        _instance.AddChild(newContainer);
        node.Connect(Node.SignalName.TreeExiting, new Callable(newContainer, LogContainer.MethodName.RemoveAt));
        newContainer._parentNode = node;
        newContainer._SetName(node.Name);
        _containers.Add(newContainer);
        return newContainer;
    }

    public void Print(params Variant[] args)
    {
        _mainLog.Print(args);
    }
    
    // Tracker functionalities
    public static TrackerText AddText()
    {
        _mainLog.Show();
        return _mainLog.AddText();
    }

    public static TrackerText Text(string properties, Node node = null)
    {
        _mainLog.Show();
        return _mainLog.Text(properties, node);
    }

    public static TrackerAngle AddAngle()
    {
        _mainLog.Show();
        return _mainLog.AddAngle();
    }

    public static TrackerAngle Angle(string properties = "rotation", Node node = null)
    {
        _mainLog.Show();
        return _mainLog.Angle(properties, node);
    }

    public static TrackerGraph AddGraph()
    {
        _mainLog.Show();
        return _mainLog.AddGraph();
    }

    public static TrackerGraph Graph(string properties, Node node = null, float? rangeMin = null, float? rangeMax = null)
    {
        _mainLog.Show();
        return _mainLog.Graph(properties, node, rangeMin, rangeMax);
    }

    public static TrackerColor AddColor()
    {
        _mainLog.Show();
        return _mainLog.AddColor();
    }

    public static TrackerColor Color(string properties = "modulate", Node node = null)
    {
        _mainLog.Show();
        return _mainLog.Color(properties, node);
    }

    // Sketch functionalities
    // TODO: Add if to assign colour if it == default()
    public static void SketchLine(Vector2 from, Vector2 to, float duration = 0.0f, Color color = default)
    {
        if (color == default) color = _defaultColor;
        List<Variant> newLine = new List<Variant> { from, to, color, duration * 1000, Time.GetTicksMsec() };
        _sketchboard.lines.Add(newLine);
    }

    public static void SketchArrow(Vector2 position, Vector2 vector, float duration = 0.0f, Color color = default)
    {
        if (color == default) color = _defaultColor;
        List<Variant> newArrow = new List<Variant> { position, position + vector, color, duration * 1000, Time.GetTicksMsec() };
        _sketchboard.arrows.Add(newArrow);
    }

    public static void SketchCircle(Vector2 position, float radius, float duration = 0.0f, Color color = default)
    {
        if (color == default) color = _defaultColor;
        List<Variant> newCircle = new List<Variant> { position, radius, color, duration * 1000, Time.GetTicksMsec() };
        _sketchboard.circles.Add(newCircle);
    }

    public static void SketchBox(Vector2 position, Vector2 size, float duration = 0.0f, Color color = default)
    {
        if (color == default) color = _defaultColor;
        SketchRect(new Rect2(position - size / 2.0f, size), duration, color);
    }

    public static void SketchRect(Rect2 rect, float duration = 0.0f, Color color = default)
    {
        if (color == default) color = _defaultColor;
        List<Variant> newRect = new List<Variant> { rect, color, duration * 1000, Time.GetTicksMsec() };
        _sketchboard.rects.Add(newRect);
    }
}
