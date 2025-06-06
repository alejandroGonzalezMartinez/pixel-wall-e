namespace pixel_wall_e.Compiler;

public class Scanner
{
    private readonly string Source;
    private readonly List<Token> Tokens = [];
    private int Start = 0;
    private int Current = 0;
    private int Line = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "Spawn", TokenType.SPAWN },
        { "Color", TokenType.COLOR },
        { "DrawLine", TokenType.DRAW_LINE },
        { "DrawRectangle", TokenType.DRAW_RECTANGLE },
        { "DrawCircle", TokenType.DRAW_CIRCLE },
        { "Size", TokenType.SIZE },
        { "Fill", TokenType.FILL },
        { "GetActualX", TokenType.GET_ACTUAL_X },
        { "GetActualY", TokenType.GET_ACTUAL_Y },
        { "GetCanvasSize", TokenType.GET_CANVAS_SIZE },
        { "GetColorCount", TokenType.GET_COLOR_COUNT },
        { "IsBrushColor", TokenType.IS_BRUSH_COLOR },
        { "IsBrushSize", TokenType.IS_BRUSH_SIZE },
        { "IsCanvasColor", TokenType.IS_CANVAS_COLOR },
        { "GoTo", TokenType.GOTO }
    };

    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            Start = Current;
            ScanToken();
        }

        AddToken(TokenType.EOF);

        return Tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '[': AddToken(TokenType.LEFT_BRACKET); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET); break;
            case ',': AddToken(TokenType.COMMA); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '+': AddToken(TokenType.PLUS); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '*':
                AddToken(Match('*') ? TokenType.POWER : TokenType.STAR);
                break;
            case '/': AddToken(TokenType.SLASH); break;
            case '%': AddToken(TokenType.MODULO); break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                if (Match('-'))
                    AddToken(TokenType.ASSIGN);
                else if (Match('='))
                    AddToken(TokenType.LESS_EQUAL);
                else
                    AddToken(TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case ' ':
            case '\r':
            case '\t':
                break; // ignorar espacios
            case '\n':
                Line++;
                AddToken(TokenType.NEW_LINE);
                break;
            case '"':
                String();
                break;
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Console.WriteLine($"[Línea {Line}] Carácter inesperado: '{c}'");
                }
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = Source.Substring(Start, Current - Start);
        TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.IDENTIFIER;
        AddToken(type);
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        /* if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            while (IsDigit(Peek())) Advance();
        } */

        string numStr = Source.Substring(Start, Current - Start);
        AddToken(TokenType.NUMBER, int.Parse(numStr));
    }

    private void String()
    {
        while (!IsAtEnd() && Peek() != '"')
        {
            if (Peek() == '\n') Line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Console.WriteLine($"[Línea {Line}] Cadena sin cerrar.");
            return;
        }

        Advance();

        string value = Source.Substring(Start + 1, Current - Start - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Advance() => Source[Current++];

    private bool Match(char expected)
    {
        if (IsAtEnd() || Source[Current] != expected) return false;
        Current++;
        return true;
    }

    private char Peek() => IsAtEnd() ? '\0' : Source[Current];
    //private char PeekNext() => Current + 1 >= Source.Length ? '\0' : Source[Current + 1];

    private bool IsDigit(char c) => c >= '0' && c <= '9';
    private bool IsAlpha(char c) => char.IsLetter(c) || c == '_';
    private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    private bool IsAtEnd() => Current >= Source.Length;

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = Source.Substring(Start, Current - Start);
        Tokens.Add(new Token(type, text, literal, Line));
    }
}