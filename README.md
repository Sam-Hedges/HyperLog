## ✨ HyperLog-CSharp ✨

_For Godot 4 with C# support - [Original HyperLog for GDScript](https://github.com/GuyUnger/HyperLog)_

**HyperLog-CSharp** is a C# port of the original **HyperLog** utility for Godot. It allows you to easily track node variables and information on screen, making debugging much easier.

**HyperLog-CSharp** maintains all the original functionality, but utilizes C# scripts, catering to developers who prefer to use C# with Godot. The project is currently a work in progress, but fully functional.

Special thanks to [GuyUnger](https://github.com/GuyUnger) for creating the GDScript version of which this is built!

[![Watch the video](https://img.youtube.com/vi/tZ3UGLp86l8/hqdefault.jpg)](https://youtu.be/tZ3UGLp86l8)

## Future Plans 🚀

The intention is to improve this project with the goal of either merging it with the original **HyperLog** or supporting it as a separate version. Contributions and feedback are welcome!

### TODO
- Update the syntax to be more c# oriented
- Improve StringFormat utility
- Fix scaling issues at non 16:9 aspect ratios
- Figure out the rest of this list!!!

## Things to Keep in Mind

- As this is a wip the syntax is still very idiomatically similar to gdscript
- There still may be undiscovered bugs (Please raise any issues if you find any!)

## How to use it in your Project 🤷♂️

First, download or clone this repo.

Copy the `hyperlog` directory into your `res://addons/` directory.

Open `Project / Project Settings / Plugins` and enable the HyperLog addon

After activating the addon in Godot, you will be able to use **HyperLog-CSharp** via the `HyperLog` global.

### HyperLog for 3D / Spatial nodes

To use **HyperLog in 3D or on Spatial nodes** you need to set the `Camera3D` variable on `HyperLog`:

```csharp
HyperLog.Camera3D = GetNode<Camera3D>("Camera");
```

If you don't do that, all logs will be displayed on the top-left corner.

## Example Scenes

Inside the project, you will find example scenes showcasing how **HyperLog** is used in 2D and 3D. We highly recommend checking them out.

## Example

We have a Scene called `Player_Ship` with the following variables in its script:

```csharp
using Godot;
using System;

public class PlayerShip : KinematicBody2D
{
    public Vector2 position = new Vector2(10.241f, 282.2035f);
    public Vector2 direction = new Vector2(-1, 0);
    public float angle = 1.570796f;
    public int health_current = 8;
    public int health_max = 12;
    public float speed = 0.0f;

    private PlayerShip ship;

    public override void _Ready()
    {
        ship = this;
        ...
    }
}
```

To use **HyperLog** to track any of these variables (or properties from the class), **add these snippets to your `_Ready()` function**.

You do not have to add it to `_Process()` or `_PhysicsProcess()` or update it in any other way, the changes of the variables are tracked automatically by **HyperLog**.

## Functions available

### Log text

> Displays the **text**: `position:x 10` next to the `ship` node.

```csharp
HyperLog.Log(ship).Text("position:x>round");
```

> Displays the **text**: `direction 3.141593`.

```csharp
HyperLog.Log(ship).Text("direction>angle");
```

> Displays the **text**: `position (10.24, 282.20)` next to `self` (in our example, the ship).

```csharp
HyperLog.Log(this).Text("position>%0.2f");
```

### Log color

> Logs the rotation of the ship's `gun` to the log-panel of the ship.

```csharp
HyperLog.Log(ship).Color("modulate");
```

### Log line-chart / graph

> Draws a line chart **graph** out the x and y `position` of the ship.

```csharp
HyperLog.Log(ship).Graph("position");
```

> Draws a line chart **graph** out of `speed`, rounds it to one decimal and updates it every second frame.

```csharp
HyperLog.Log(ship).Graph("speed>%0.1f").SetSteps(2);
```

> Draws a line chart **graph** out the `health`, using `health_max` as the maximum value of the line-chart.

```csharp
HyperLog.Log(ship).Graph("health_current").SetRange(0, health_max);
```

### Log angles

> Draws an **angle**\-log, a visual angle indicator.

```csharp
HyperLog.Log(ship).Angle(new[] { "direction", "angle" });
```

> Draws an **angle**\-log of the `rotation` of the ship's `gun` to the log-panel of the `ship`.

```csharp
HyperLog.Log(ship).Angle("rotation", ship.GetNode("gun"));
```

### Change the Log Panel position

> Adds an **offset** of (200, -20) to the panel.

```csharp
HyperLog.Log(ship).Offset(new Vector2(200, -20));
```

> Will **align** the panel to the right horizontally and to the center vertically.

```csharp
HyperLog.Log(ship).Align(HyperLog.HAlignRight, HyperLog.VAlignCenter);
```

> Will display a health line-chart **graph** above the ship.

```csharp
HyperLog.Log(ship)
    .Align(HyperLog.HAlignCenter, HyperLog.VAlignBottom)
    .Offset(new Vector2(0, -50))
    .Graph("health_current")
    .SetRange(0, health_max);
```

### Logging to the top main panel

Alternatively, you can log to the main panel by accessing the functions directly from **HyperLog**:

```csharp
HyperLog.Graph("position", ship);
```

### Drawing Tools / Sketch functions

There are some drawing tools that you can use for additional debugging. You can call those from anywhere to draw visual debugging elements.

For example, calling `HyperLog.SketchArrow(position, direction, 1)` will draw an arrow on position, direction for one second.

**These draw tools behave differently from the normal HyperLog logging and will not automatically update!**

**You can use them in \_Process() or on certain functions in your game (e.g. display the direction of your projectile when shooting via HyperLog.SketchLine).**

It's best to check out the provided example scenes or the code in the plugin to get a better understanding of those draw tools.

### Sketch Functions reference

> Draw a line from `from` to `to` for `duration` seconds.

```csharp
HyperLog.SketchLine(new Vector2(0, 0), new Vector2(1, 1), 0, new Color("tomato"));
```

> Draw an arrow at `position`, pointing to `vector` for `duration` seconds.

```csharp
HyperLog.SketchArrow(new Vector2(0, 0), new Vector2(1, 1), 0, new Color("tomato"));
```

> Draw a circle at `position` with the radius `radius` for `duration` seconds.

```csharp
HyperLog.SketchCircle(new Vector2(0, 0), 10.0f, 0, new Color("tomato"));
```

> Draw a box at `position` with the dimensions `size` for `duration` seconds.

```csharp
HyperLog.SketchBox(new Vector2(0, 0), new Vector2(10, 10), 0, new Color("tomato"));
```

> Draw a Rect2 with the data from `rect` for `duration` seconds.

```csharp
HyperLog.SketchRect(new Rect2(0, 0, 10, 10), 0, new Color("tomato"));
```