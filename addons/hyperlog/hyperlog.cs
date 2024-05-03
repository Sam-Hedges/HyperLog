using Godot;
using System;

public class HyperLog : Node
{
    private readonly GDScript _logContainerScript = (GDScript)GD.Load("res://addons/hyperlog/log_container.gd");
    private GodotObject _mainLog;
    private Godot.Collections.Array colors = new Godot.Collections.Array();

    public override void _Ready()
    {
        _mainLog = (GodotObject)_logContainerScript.New();
        HideMainLog();
        SetParentNode(GetTree().Root);

        for (int i = 0; i < 64; i++)
        {
            Color color = Colors.Red;
            color.h += i * (1 / 4.0f + 1 / 32.0f);
            color.v *= 1 - (i / 128.0f) * 0.5f;
            colors.Add(color);
        }
    }

    private void HideMainLog()
    {
        _mainLog.Call("hide");
    }

    private void SetParentNode(Node parentNode)
    {
        _mainLog.Set("parent_node", parentNode);
    }

    public GodotObject Log(Node node, params object[] prints)
    {
        GodotObject container = GetContainer(node);
        if (prints.Length > 0)
        {
            container.Call("print", prints);
        }
        container.Call("set_scale", new Vector2(1, 1) * 0.75f);
        return container;
    }

    public void RemoveLog(Node node)
    {
        Godot.Collections.Array containers = (Godot.Collections.Array)Get("containers");
        for (int i = 0; i < containers.Count; i++)
        {
            GodotObject container = (GodotObject)containers[i];
            if (container.Get("parent_node") == node)
            {
                container.Call("queue_free");
                containers.Remove(i);
                break;
            }
        }
    }

    public void PauseLog(Node node)
    {
        Godot.Collections.Array containers = (Godot.Collections.Array)Get("containers");
        foreach (GodotObject container in containers)
        {
            if (container.Get("parent_node") == node)
            {
                container.Set("tracking", false);
                break;
            }
        }
    }

    public void ContinueLog(Node node)
    {
        Godot.Collections.Array containers = (Godot.Collections.Array)Get("containers");
        foreach (GodotObject container in containers)
        {
            if (container.Get("parent_node") == node)
            {
                container.Set("tracking", true);
                break;
            }
        }
    }

    private GodotObject GetContainer(Node node)
    {
        Godot.Collections.Array containers = (Godot.Collections.Array)Get("containers");
        foreach (GodotObject container in containers)
        {
            if (container.Get("parent_node") == node)
            {
                return container;
            }
        }

        GodotObject newContainer = (GodotObject)_logContainerScript.New();
        AddChild((Node)newContainer);
        node.Connect("tree_exiting", new Callable(newContainer, "remove_at"));
        newContainer.Set("parent_node", node);
        newContainer.Call("_set_name", node.Name);
        containers.Add(newContainer);
        return newContainer;
    }

    public void Print(params object[] args)
    {
        _mainLog.Call("print", args);
    }
    
    // Tracker functionalities
    public GodotObject AddText()
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("add_text");
    }

    public GodotObject Text(string properties, Node node = null)
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("text", properties, node);
    }

    public GodotObject AddAngle()
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("add_angle");
    }

    public GodotObject Angle(string properties = "rotation", Node node = null)
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("angle", properties, node);
    }

    public GodotObject AddGraph()
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("add_graph");
    }

    public GodotObject Graph(string properties, Node node = null, float? rangeMin = null, float? rangeMax = null)
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("graph", properties, node, rangeMin, rangeMax);
    }

    public GodotObject AddColor()
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("add_color");
    }

    public GodotObject Color(string properties = "modulate", Node node = null)
    {
        _mainLog.Call("show");
        return (GodotObject)_mainLog.Call("color", properties, node);
    }

    // Sketch functionalities
    public void SketchLine(Vector2 from, Vector2 to, float duration = 0.0f, Color color = RDPipelineColorBlendState.)
    {
        _mainLog.Call("sketch_line", from, to, duration, color);
    }

    public void SketchArrow(Vector2 position, Vector2 vector, float duration = 0.0f, Color color = Colors.Tomato)
    {
        _mainLog.Call("sketch_arrow", position, vector, duration, color);
    }

    public void SketchCircle(Vector2 position, float radius, float duration = 0.0f, Color color = Colors.Tomato)
    {
        _mainLog.Call("sketch_circle", position, radius, duration, color);
    }

    public void SketchBox(Vector2 position, Vector2 size, float duration = 0.0f, Color color = Colors.Tomato)
    {
        _mainLog.Call("sketch_box", position, size, duration, color);
    }

    public void SketchRect(Rect2 rect, float duration = 0.0f, Color color = Colors.Tomato)
    {
        _mainLog.Call("sketch_rect", rect, duration, color);
    }
}
