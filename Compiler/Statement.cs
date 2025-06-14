namespace pixel_wall_e.Compiler;

// Clase base abstracta para todas las instrucciones
public abstract class Statement { }

// Spawn(x, y)
public class SpawnStatement : Statement
{
    public int X { get; }
    public int Y { get; }

    public SpawnStatement(int x, int y)
    {
        X = x;
        Y = y;
    }
}

// Color("Red")
public class ColorStatement : Statement
{
    public string ColorName { get; }

    public ColorStatement(string colorName)
    {
        ColorName = colorName;
    }
}

// Size(k)
public class SizeStatement : Statement
{
    public int Size { get; }

    public SizeStatement(int size)
    {
        Size = size;
    }
}

// DrawLine(dx, dy, distance)
public class DrawLineStatement : Statement
{
    public int DirX { get; }
    public int DirY { get; }
    public int Distance { get; }

    public DrawLineStatement(int dirX, int dirY, int distance)
    {
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
    }
}

// DrawRectangle(width, height)
public class DrawRectangleStatement : Statement
{
    public int DirX { get; }
    public int DirY { get; }
    public int Distance { get; }
    public int Width { get; }
    public int Height { get; }

    public DrawRectangleStatement(int dirX, int dirY, int distance, int width, int height)
    {
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
        Width = width;
        Height = height;
    }
}

// DrawCircle(radius)
public class DrawCircleStatement : Statement
{
    public int Radius { get; }
    public int DirX { get; }
    public int DirY { get; }

    public DrawCircleStatement(int dirX, int dirY, int radius)
    {
        DirX = dirX;
        DirY = dirY;
        Radius = radius;
    }
}

// Fill()
public class FillStatement : Statement { }

// GetActualX()
public class GetActualXStatement : Statement { }

// GetActualY()
public class GetActualYStatement : Statement { }

// GetCanvasSize()
public class GetCanvasSizeStatement : Statement { }

// GetColorCount()
public class GetColorCountStatement : Statement
{
    public string Color;
    public int X1;
    public int X2;
    public int Y1;
    public int Y2;

    public GetColorCountStatement(string color, int x1, int y1, int x2, int y2)
    {
        Color = color;
        X1 = x1;
        X2 = x2;
        Y1 = y1;
        Y2 = y2;
    }    
}

// IsBrushColor(color)
public class IsBrushColorStatement : Statement
{
    public string Color { get; }

    public IsBrushColorStatement(string color)
    {
        Color = color;
    }
}

// IsBrushSize(size)
public class IsBrushSizeStatement : Statement
{
    public int Size { get; }

    public IsBrushSizeStatement(int size)
    {
        Size = size;
    }
}

// IsCanvasColor(x, y, color)
public class IsCanvasColorStatement : Statement
{
    public int X { get; }
    public int Y { get; }
    public string Color { get; }

    public IsCanvasColorStatement(string color, int x, int y)
    {
        X = x;
        Y = y;
        Color = color;
    }
}

public class GotoStatement : Statement
{
    public string Label { get; }
    public Expression Condition { get; }

    // Constructor
    public GotoStatement(string label, Expression condition)
    {
        Label = label;
        Condition = condition;
    }
}

public class AssignmentStatement : Statement
{
    public string VariableName { get; }
    public Expression Value { get; }

    public AssignmentStatement(string variableName, Expression value)
    {
        VariableName = variableName;
        Value = value;
    }
}

public class LabelStatement : Statement
{
    public string LabelName;

    public LabelStatement(string labelName)
    {
        LabelName = labelName;
    }
}
