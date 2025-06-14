using pixel_wall_e.Components;
using pixel_wall_e.Models;

namespace pixel_wall_e.Compiler;

public class InterpreterState
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; set; } = "#ddd";
    public int Size { get; set; } = 1;
    public Dictionary<string, object> Variables { get; } = new();
    public Dictionary<string, int> Labels { get; } = new();
    public int InstructionPointer { get; set; } = 0;
    public bool Jumped { get; set; } = false;
    public int CanvasWidth { get; set; } = 32;
    public int CanvasHeight { get; set; } = 32;
    public PixelCanvas Canvas { get; set; } = new PixelCanvas();

    public InterpreterState(PixelCanvas canvas)
    {
        Canvas = canvas;
    }
}