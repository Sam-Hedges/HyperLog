using Godot;

public partial class TrackerGraph : Tracker
{
    private const string MAX_STR = "Max: {0:F1}";
    private const string MIN_STR = "Min: {0:F1}";

    private float min_value;
    private float max_value = 0.1f;
    private int tracking_length = 1;

    private float next_min_value;
    private float next_max_value;

    private float? force_min_value;
    private float? force_max_value;

    private int step_size = 20;
    private int _step;
    private bool _dirty;

    private Label name_label;
    private Label max_label;
    private Label min_label;

    public override void _Ready()
    {
        min_label = GetNode<Label>("min_value");
        name_label = GetNode<Label>("labels/name_label");
        max_label = GetNode<Label>("labels/max_value");
        SetHeight(80);
    }

    protected override ValueMonitor AddTracker(string property, Node node = null)
    {
        if (container.GetParent() != null && node != container.GetParent())
        {
            name_label.Text = $"{container.GetParent().GetPathTo(node)} > {property}";
        }
        else
        {
            name_label.Text = property;
        }
        return base.AddTracker(property, node);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!container.Tracking) return;
        if (!_insideViewport) return;
        _step -= 1;
        if (_step < step_size)
        {
            _step += step_size;
            TrackersStoreValue();
            _dirty = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!container.Tracking) return;
        if (!_insideViewport) return;
        if (_dirty)
        {
            QueueRedraw();
            _dirty = false;
        }
    }

    public override void _Draw()
    {
        next_min_value = 999999.0f;
        next_max_value = -999999.0f;
        int next_tracking_length = 1;

        if (min_value < 0 && max_value > 0)
        {
            float zero_height = _RangeValue(0);
            DrawLine(new Vector2(0, zero_height), new Vector2(Size.X, zero_height), new Color(0.8f, 0.8f, 0.8f, 0.4f), 1.0f, true);
        }

        DrawLine(new Vector2(0.0f, 1.0f), new Vector2(Size.X, 1), new Color(1.0f, 1.0f, 1.0f, 0.5f), 1.0f, true);
        DrawLine(new Vector2(0.0f, Size.Y - 1.0f), new Vector2(Size.X, Size.Y - 1), new Color(1.0f, 1.0f, 1.0f, 0.5f), 1.0f, true);

        int color_index = 0;
        foreach (ValueMonitor tracker in monitors)
        {
            next_tracking_length = Mathf.Max(next_tracking_length, tracker.backlog.Count);

            if (tracker.backlog.Count < 2)
            {
                continue;
            }

            if (tracker.backlog[0].Obj is Vector2)
            {
                for (int i = 0; i < tracker.backlog.Count - 1; i++)
                {
                    _GraphSegment(((Vector2)tracker.backlog[i]).X, ((Vector2)tracker.backlog[i + 1]).X, i, color_index);
                    _GraphSegment(((Vector2)tracker.backlog[i]).Y, ((Vector2)tracker.backlog[i + 1]).Y, i, color_index + 1);
                }
                color_index += 2;
            }
            else if (tracker.backlog[0].Obj is Vector3)
            {
                for (int i = 0; i < tracker.backlog.Count - 1; i++)
                {
                    _GraphSegment(((Vector3)tracker.backlog[i]).X, ((Vector3)tracker.backlog[i + 1]).X, i, color_index);
                    _GraphSegment(((Vector3)tracker.backlog[i]).Y, ((Vector3)tracker.backlog[i + 1]).Y, i, color_index + 1);
                    _GraphSegment(((Vector3)tracker.backlog[i]).Z, ((Vector3)tracker.backlog[i + 1]).Z, i, color_index + 2);
                }
                color_index += 3;
            }
            else
            {
                for (int i = 0; i < tracker.backlog.Count - 1; i++)
                {
                    _GraphSegment((float)tracker.backlog[i], (float)tracker.backlog[i + 1], i, color_index);
                }
                color_index += 1;
            }
        }

        if (force_min_value == null)
        {
            min_value = next_min_value;
            min_label.Text = string.Format(MIN_STR, min_value);
        }
        if (force_max_value == null)
        {
            max_value = next_max_value;
            max_label.Text = string.Format(MAX_STR, max_value);
        }
        tracking_length = next_tracking_length;

        if (min_value == max_value)
        {
            max_value += 0.00001f;
        }
    }

    void _GraphSegment(float from, float to, int pos, int color_index)
    {
        next_min_value = Mathf.Min(next_min_value, from);
        next_max_value = Mathf.Max(next_max_value, from);

        Vector2 pos_from = new Vector2(pos / (float)tracking_length * Size.X, _RangeValue(from));
        Vector2 pos_to = new Vector2((pos + 1) / (float)tracking_length * Size.X, _RangeValue(to));

        DrawLine(pos_from, pos_to, HyperLog.colors[color_index], 1.0f, true);
    }

    float _RangeValue(float value)
    {
        return (1 - ((value - min_value) / (max_value - min_value))) * (CustomMinimumSize.Y - 2) + 1;
    }

    public TrackerGraph SetRangeMin(float value)
    {
        force_min_value = value;
        min_value = value;
        min_label.Text = string.Format(MIN_STR, min_value);
        return this;
    }

    public TrackerGraph SetRangeMax(float value)
    {
        force_max_value = value;
        max_value = value;
        max_label.Text = string.Format(MAX_STR, max_value);
        return this;
    }

    public TrackerGraph SetRange(float value_min, float value_max)
    {
        force_min_value = value_min;
        min_value = value_min;
        min_label.Text = string.Format(MIN_STR, min_value);
        force_max_value = value_max;
        max_value = value_max;
        max_label.Text = string.Format(MAX_STR, max_value);
        return this;
    }

    public TrackerGraph SetSteps(int value)
    {
        step_size = value;
        return this;
    }
}
