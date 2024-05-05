using System;
using System.Collections.Generic;
using Godot;

public partial class ValueMonitor : Resource
{
    private Node node;
    internal string property = "";
    internal string propertyName = "";
    internal List<Variant> backlog = new();

    internal readonly int MaxLogLength = 1024;

    private enum FormatType
    {
        None,
        FormatString,
        Int,
        Bool,
        Round,
        Angle
    }

    private FormatType formatType = FormatType.None;
    private string formatString = "";
    
    public ValueMonitor(Node node, string property, Node parent = null)
    {
        this.node = node;
        int castIndex = property.Find('>');
        if (castIndex != -1)
        {
            this.property = property.Substr(0, castIndex);

            string formatArgument = property.Substr(castIndex + 1, property.Length);
            formatType = FormatType.FormatString;

            if (formatArgument.StartsWith('%'))
            {
                formatType = FormatType.FormatString;
                formatString = formatArgument;
            }
            else
            {
                switch (formatArgument)
                {
                    case "int":
                        formatType = FormatType.Int;
                        break;
                    case "bool":
                        formatType = FormatType.Bool;
                        break;
                    case "round":
                        formatType = FormatType.Round;
                        break;
                    case "angle":
                        formatType = FormatType.Angle;
                        break;
                }
            }
        }
        else
        {
            this.property = property;
        }

        if (parent != null && node != parent)
        {
            propertyName = parent.GetPathTo(node) + " > " + this.property;
        }
        else
        {
            propertyName = this.property;
        }
    }

    public Variant GetValue()
    {
        return node.GetIndexed(property);
    }

    public object Format(Variant value)
    {
        bool err;
        switch (formatType)
        {
            case FormatType.FormatString:
            {
                if (value.Obj is Vector2)
                {
                    object temp = $"({string.Format(formatString, ((Vector2)value).X)}, {string.Format(formatString, ((Vector2)value).Y)})";
                    return StringFormat.Format(formatString, value, out err);
                }

                if (value.Obj is Vector3)
                {
                    return
                        $"({string.Format(formatString, ((Vector3)value).X)}, {string.Format(formatString, ((Vector3)value).Y)}, {string.Format(formatString, ((Vector3)value).Z)})";
                }

                return StringFormat.Format(formatString, value, out err);
            }
            case FormatType.Int:
                return (int)value;
            case FormatType.Bool:
                return (bool)value;
            case FormatType.Round:
            {
                if (value.Obj is Vector2 or Vector3)
                {
                    return ((Vector2)value).Round();
                }

                return Mathf.Round((float)value);
            }
            case FormatType.Angle:
                return value.Obj is Vector2 ? ((Vector2)value).Angle() : value;
            case FormatType.None:
            default:
                return value;
        }
    }

    public void StoreValue()
    {
        backlog.Insert(0, GetValue());
        while (backlog.Count > MaxLogLength)
        {
            backlog.RemoveAt(backlog.Count - 1);
        }
    }
}