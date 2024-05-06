using Godot;
using System;
using System.Collections.Generic;

public partial class Tracker : Control
{
    public List<ValueMonitor> monitors = new();
    public LogContainer container;
    internal bool _insideViewport => GetViewportRect().Encloses(GetRect());

    public void Track(object properties, object node = null)
    {
        if (node is string)
        {
            string nodeStr = (string)node;
            if (nodeStr.EndsWith("/*"))
            {
                node = container.GetParent().GetNode(nodeStr.Substr(0, nodeStr.Length - 2));
                foreach (Node child in ((Node)node).GetChildren())
                {
                    Track(properties, child);
                }
                return;
            }
            node = container.GetParent().GetNode(nodeStr);
        }
        else if (node == null && container._parentNode != null)
        {
            node = container._parentNode;
        }

        if (properties is string)
        {
            AddTracker((string)properties, (Node)node);
        }
        else if (properties is Array)
        {
            foreach (string property in (Array)properties)
            {
                AddTracker(property, (Node)node);
            }
        }
    }

    protected virtual ValueMonitor AddTracker(string property, Node node)
    {
        ValueMonitor monitor = new ValueMonitor(node, property, container._parentNode);
        monitors.Add(monitor);
        return monitor;
    }

    protected virtual void RemoveTracker(ValueMonitor monitor)
    {
        monitors.Remove(monitor);
    }

    protected virtual Tracker SetHeight(float value)
    {
        Vector2 newSize = new Vector2
        {
            X = CustomMinimumSize.X,
            Y = value
        };
        SetDeferred(Control.PropertyName.CustomMinimumSize, newSize);
        newSize = new Vector2
        {
            X = Size.X,
            Y = value
        };
        SetDeferred(Control.PropertyName.Size, newSize);
        return this;
    }

    protected void TrackersStoreValue()
    {
        foreach (ValueMonitor tracker in monitors)
        {
            tracker.StoreValue();
        }
    }
}