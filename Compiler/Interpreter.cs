using System.Data;

namespace pixel_wall_e.Compiler;

public class Interpreter
{
    private static InterpreterState? _state;

    public static string Interpret(List<Statement> statements, InterpreterState state)
    {
        _state = state;

        for (int i = 0; i < statements.Count; i++)
        {
            if (statements[i] is LabelStatement label)
            {
                if (!_state.Labels.ContainsKey(label.LabelName))
                    _state.Labels[label.LabelName] = i;
            }
        }

        _state.InstructionPointer = 0;
        while (_state.InstructionPointer < statements.Count)
        {
            try
            {
                _state.Jumped = false;
                Execute(statements[_state.InstructionPointer]);

                if (!_state.Jumped)
                    _state.InstructionPointer++;
            }
            catch (Exception ex)
            {
                return $"[ERROR en línea {_state.InstructionPointer}]: {ex.Message}";
            }
        }
        return "Ejecución finalizada correctamente.";
    }

    private static void Execute(Statement stmt)
    {
        if (_state == null) return;
        switch (stmt)
        {
            case SpawnStatement spawn:
                _state.X = spawn.X;
                _state.Y = spawn.Y;
                break;

            case ColorStatement color:
                _state.Color = color.ColorName;
                break;

            case AssignmentStatement assign:
                var result = Evaluate(assign.Value);
                _state.Variables[assign.VariableName] = result;
                break;

            case LabelStatement:
                break;

            case GotoStatement gotoStmt:
                var conditionValue = Evaluate(gotoStmt.Condition);
                if (IsTruthy(conditionValue) && Convert.ToInt32(conditionValue) == 1)
                {
                    if (!_state.Labels.TryGetValue(gotoStmt.Label, out int targetIndex))
                        throw new Exception($"Etiqueta no encontrada: {gotoStmt.Label}");
                    _state.InstructionPointer = targetIndex;
                    _state.Jumped = true;
                }
                break;

            case SizeStatement size:
                _state.Size = size.Size % 2 == 0 ? size.Size - 1 : size.Size;
                break;

            case DrawLineStatement drawStmt:
                for (int i = 0; i < drawStmt.Distance; i++)
                {
                    Draw(_state.X, _state.Y);

                    _state.X += drawStmt.DirX;
                    _state.Y += drawStmt.DirY;
                }
                break;
            
            case DrawCircleStatement drawStmt:
                int radius = drawStmt.Radius;
                int dirX = drawStmt.DirX;
                int dirY = drawStmt.DirY;

                int centerX = _state.X + dirX * radius;
                int centerY = _state.Y + dirY * radius;

                _state.X = centerX;
                _state.Y = centerY;

                double r = radius;
                double tolerance = 0.5;

                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    for (int y = centerY - radius; y <= centerY + radius; y++)
                    {
                        double dix = x - centerX;
                        double diy = y - centerY;
                        double distance = Math.Sqrt(dix * dix + diy * diy);

                        if (Math.Abs(distance - r) <= tolerance)
                        {
                            Draw(x, y);
                        }
                    }
                }
                break;

            case DrawRectangleStatement rectangle:
                int dx = rectangle.DirX;
                int dy = rectangle.DirY;
                int dist = rectangle.Distance;
                int width = rectangle.Width % 2 == 0 ? rectangle.Width + 1 : rectangle.Width;
                int height = rectangle.Height % 2 == 0 ? rectangle.Height + 1 : rectangle.Height;

                int fullWidth = width + 2;
                int fullHeight = height + 2;

                centerX = _state.X + dx * dist;
                centerY = _state.Y + dy * dist;

                _state.X = centerX;
                _state.Y = centerY;

                int startX = centerX - fullHeight / 2;
                int startY = centerY - fullWidth / 2;
                int endX = startX + fullHeight - 1;
                int endY = startY + fullWidth - 1;

                for (int x = startX; x <= endX; x++)
                {
                    Draw(x, startY);
                    Draw(x, endY);  
                }

                for (int y = startY + 1; y < endY; y++)
                {
                    Draw(startX, y);
                    Draw(endX, y);
                }
                break;

            case FillStatement:
                Fill(_state.X, _state.Y);
                break;

            case GetActualXStatement:
                break;
            
            case GetActualYStatement:
                break;
            
            case GetCanvasSizeStatement:
                break;

            case GetColorCountStatement:
                break;

            case IsBrushColorStatement:
                break;

            case IsBrushSizeStatement:
                break;

            case IsCanvasColorStatement:
                break;
                
            default:
                throw new Exception($"No se puede ejecutar el statement {stmt.GetType()}");
        }
    }

    private static void Fill(int x, int y)
    {
        if (_state == null) return;

        string targetColor = _state.Canvas.GetPixel(x, y);

        if (targetColor == _state.Color) return;

        FloodFill(x, y, targetColor);
    }

    private static void FloodFill(int x, int y, string targetColor)
    {
        if (_state == null) return;

        if (x < 0 || x >= _state.Canvas.Height || y < 0 || y >= _state.Canvas.Width)
            return;

        if (_state.Canvas.GetPixel(x, y) != targetColor)
            return;

        _state.Canvas.SetPixel(x, y, _state.Color);

        FloodFill(x - 1, y, targetColor);
        FloodFill(x + 1, y, targetColor);
        FloodFill(x, y - 1, targetColor);
        FloodFill(x, y + 1, targetColor);
    }

    private static void Draw(int _x, int _y)
    {
        if (_state == null) return;

        for (int x = _x - _state.Size / 2; x <= _x + _state.Size / 2; x++)
        {
            for (int y = _y - _state.Size / 2; y <= _y + _state.Size / 2; y++)
            {
                _state.Canvas.SetPixel(x, y, _state.Color);
            }
        }
    }

    private static object Evaluate(Expression expr)
    {
        if (_state == null) return 0;
        return expr switch
        {
            LiteralExpression literal => literal.Value,
            VariableExpression variable =>
                _state.Variables.TryGetValue(variable.Name, out var value)
                    ? value
                    : throw new Exception($"Variable no definida: {variable.Name}"),
            BinaryExpression binary => EvaluateBinary(binary),
            CallExpression call => EvaluateCall(call),
            _ => throw new Exception($"Expresión desconocida: {expr.GetType()}")
        };
    }

    private static object EvaluateBinary(BinaryExpression expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        return expr.Operator.Type switch
        {
            TokenType.PLUS => (dynamic)left + (dynamic)right,
            TokenType.MINUS => (dynamic)left - (dynamic)right,
            TokenType.STAR => (dynamic)left * (dynamic)right,
            TokenType.SLASH => (dynamic)left / (dynamic)right,
            TokenType.MODULO => (dynamic)left % (dynamic)right,
            TokenType.POWER => Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right)),
            TokenType.EQUAL_EQUAL => IsEqual(left, right) ? 1 : 0,
            TokenType.BANG_EQUAL => !IsEqual(left, right) ? 1 : 0,
            TokenType.GREATER => (dynamic)left > (dynamic)right ? 1 : 0,
            TokenType.GREATER_EQUAL => (dynamic)left >= (dynamic)right ? 1 : 0,
            TokenType.LESS => (dynamic)left < (dynamic)right ? 1 : 0,
            TokenType.LESS_EQUAL => (dynamic)left <= (dynamic)right ? 1 : 0,
            TokenType.AND => IsTruthy(left) && IsTruthy(right),
            TokenType.OR => IsTruthy(left) || IsTruthy(right),
            _ => throw new Exception($"Operador no soportado: {expr.Operator.Type}")
        };
    }

    private static object EvaluateCall(CallExpression expr)
    {
        if (_state == null) return 0;
        var args = expr.Arguments.Select(Evaluate).ToList();

        return expr.FunctionName switch
        {
            "GetActualX" => _state.X,
            "GetActualY" => _state.Y,
            "GetCanvasSize" => _state.Canvas.Width == _state.Canvas.Height ? _state.Canvas.Width : $"{_state.Canvas.Width} x {_state.Canvas.Height}",
            "GetColorCount" => GetColorCount(args),
            "IsBrushColor" => _state.Color == (string)args[0] ? 1 : 0,
            "IsBrushSize" => IsCanvasColor(args) ? 1 : 0,
            "IsCanvasColor" => _state.Canvas.GetPixel(_state.X + (int)args[2], _state.Y + (int)args[1]) == (string)args[0] ? 1 : 0,
            _ => throw new Exception($"Función desconocida: {expr.FunctionName}")
        };
    }

    private static bool IsCanvasColor(List<object> args)
    {
        if (_state == null) return false;

        int px = _state.X + (int)args[2];
        int py = _state.Y + (int)args[1];

        if (px < 0 || px >= _state.Canvas.Height || py < 0 || py >= _state.Canvas.Width)
            return false;

        return _state.Canvas.GetPixel(px, py) == (string)args[0] ? true : false;
    }

    private static int GetColorCount(List<object> args)
    {
        if (_state == null) return 0;

        string color = (string)args[0];
        int x1 = (int)args[1];
        int y1 = (int)args[2];
        int x2 = (int)args[3];
        int y2 = (int)args[4];

        if (
            x1 < 0 || x1 >= _state.Canvas.Height || y1 < 0 || y1 >= _state.Canvas.Width ||
            x2 < 0 || x2 >= _state.Canvas.Height || y2 < 0 || y2 >= _state.Canvas.Width
        ) return 0;

        int c = 0;
        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                if (_state.Canvas.GetPixel(x, y) == color) c++;
            }
        }

        return c;
    }
    
    private static bool IsTruthy(object value)
    {
        if (value is bool b) return b;
        if (value is double d) return d != 0;
        if (value == null) return false;
        return true;
    }

    private static bool IsEqual(object a, object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }
}