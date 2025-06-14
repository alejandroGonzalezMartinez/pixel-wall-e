namespace pixel_wall_e.Compiler;

public class Parser
{
    private readonly List<Token> Tokens;
    private int Current = 0;

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public List<Statement> Parse()
    {
        List<Statement> statements = new();

        while (!IsAtEnd())
        {
            Statement stmt = ParseStatement();
            if (stmt != null)
                statements.Add(stmt);

            while (Peek().Type == TokenType.NEW_LINE) Advance();
        }

        return statements;
    }

    private Statement ParseStatement()
    {
        while (Peek().Type == TokenType.NEW_LINE) Advance();

        Token token = Advance();

        switch (token.Type)
        {
            case TokenType.SPAWN:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'Spawn'.");
                int x = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' después del primer argumento.");
                int y = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de los argumentos.");
                return new SpawnStatement(x, y);

            case TokenType.COLOR:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'Color'.");
                string color = ParseString();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del color.");
                return new ColorStatement(color);

            case TokenType.SIZE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'Size'.");
                int size = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del tamaño.");
                return new SizeStatement(size);

            case TokenType.DRAW_LINE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'DrawLine'.");
                int dx = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' entre dirX y dirY.");
                int dy = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' entre dirY y distance.");
                int distance = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de los argumentos.");
                return new DrawLineStatement(dx, dy, distance);

            case TokenType.DRAW_RECTANGLE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'DrawRectangle'.");
                int dirX = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ','.");
                int dirY = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ','.");
                int dist = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ','.");
                int width = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' entre ancho y alto.");
                int height = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del alto.");
                return new DrawRectangleStatement(dirX, dirY, dist, width, height);

            case TokenType.DRAW_CIRCLE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'DrawCircle'.");
                int diX = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ','.");
                int diY = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ','.");
                int radius = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del radio.");
                return new DrawCircleStatement(diX, diY, radius);

            case TokenType.FILL:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'Fill'.");
                string fillColor = ParseString();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del color.");
                return new FillStatement();

            case TokenType.GET_ACTUAL_X:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'GetActualX'.");
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de '('.");
                return new GetActualXStatement();

            case TokenType.GET_ACTUAL_Y:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'GetActualY'.");
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de '('.");
                return new GetActualYStatement();

            case TokenType.GET_CANVAS_SIZE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'GetCanvasSize'.");
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de '('.");
                return new GetCanvasSizeStatement();

            case TokenType.GET_COLOR_COUNT:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'GetColorCount'.");
                string colorGetColorCount = ParseString();
                Consume(TokenType.COMMA, "Se esperaba ',' despues del color.");
                int x1 = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
                int y1 = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
                int x2 = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
                int y2 = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ',' despues de la coordenada)");
                return new GetColorCountStatement(colorGetColorCount, x1, y1, x2, y2);

            case TokenType.IS_BRUSH_COLOR:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'IsBrushColor'.");
                string colorCheck = ParseString();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del color.");
                return new IsBrushColorStatement(colorCheck);

            case TokenType.IS_BRUSH_SIZE:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'IsBrushSize'.");
                int sizeCheck = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después del tamaño.");
                return new IsBrushSizeStatement(sizeCheck);

            case TokenType.IS_CANVAS_COLOR:
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'IsCanvasColor'.");
                string canvasColor = ParseString();
                Consume(TokenType.COMMA, "Se esperaba ',' después del color.");
                int _x = ParseInt();
                Consume(TokenType.COMMA, "Se esperaba ',' después de la coordenada X.");
                int _y = ParseInt();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' al final.");
                return new IsCanvasColorStatement(canvasColor, _x, _y);

            case TokenType.GOTO:
                Consume(TokenType.LEFT_BRACKET, "Se esperaba '[' después de 'GoTo'.");
                string label = Consume(TokenType.IDENTIFIER, "Se esperaba un nombre de etiqueta.").Lexeme;
                Consume(TokenType.RIGHT_BRACKET, "Se esperaba ']' después del nombre de la etiqueta.");
                Consume(TokenType.LEFT_PAREN, "Se esperaba '(' con la condición.");
                Expression condition = ParseExpression();
                Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' después de la condición.");
                return new GotoStatement(label, condition);

            case TokenType.IDENTIFIER:
                Token identifier = token;

                if (Match(TokenType.ASSIGN))
                {
                    Expression value = ParseExpression();
                    return new AssignmentStatement(identifier.Lexeme, value);
                }
                else if (Check(TokenType.NEW_LINE) || IsAtEnd())
                {
                    return new LabelStatement(identifier.Lexeme);
                }
                else
                {
                    throw Error(Peek(), $"Identificador inesperado: '{identifier.Lexeme}'.");
                }
    
            default:
                throw new Exception($"Instrucción inesperada: {Current}");
        }
    }

    private Token Advance()
    {
        if (!IsAtEnd()) Current++;
        return Previous();
    }

    private Token Previous() => Tokens[Current - 1];
    private bool IsAtEnd() => Peek().Type == TokenType.EOF;
    private Token Peek() => Tokens[Current];

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }


    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private Exception Error(Token token, string message)
    {
        Console.WriteLine($"[línea {token.Line}] Error en '{token.Lexeme}': {message}");
        return new Exception($"Error de análisis en línea {token.Line}: {message}");
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private int ParseInt()
    {
        if (Peek().Type == TokenType.NUMBER && Peek().Literal is int)
            return (int)Advance().Literal!;
        throw new Exception("Se esperaba un número entero.");
    }

    private string ParseString()
    {
        if (Peek().Type == TokenType.STRING && Peek().Literal is string)
            return (string)Advance().Literal!;
        throw new Exception("Se esperaba una cadena.");
    }

    private Expression ParseExpression()
    {
        return ParseOr();
    }

    private Expression ParseOr()
    {
        Expression expr = ParseAnd();

        while (Match(TokenType.OR))
        {
            Token op = Previous();
            Expression right = ParseAnd();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParseAnd()
    {
        Expression expr = ParseEquality();

        while (Match(TokenType.AND))
        {
            Token op = Previous();
            Expression right = ParseEquality();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParseEquality()
    {
        Expression expr = ParseComparison();

        while (Match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
        {
            Token op = Previous();
            Expression right = ParseComparison();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParseComparison()
    {
        Expression expr = ParseTerm();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expression right = ParseTerm();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParseTerm()
    {
        Expression expr = ParseFactor();

        while (Match(TokenType.PLUS, TokenType.MINUS))
        {
            Token op = Previous();
            Expression right = ParseFactor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParseFactor()
    {
        Expression expr = ParsePower();

        while (Match(TokenType.STAR, TokenType.SLASH, TokenType.MODULO))
        {
            Token op = Previous();
            Expression right = ParsePower();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParsePower()
    {
        Expression expr = ParsePrimary();

        while (Match(TokenType.POWER))
        {
            Token op = Previous();
            Expression right = ParsePrimary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression ParsePrimary()
    {
        if (Match(TokenType.NUMBER) || Match(TokenType.STRING))
        {
            object? value = Previous().Literal;
            if (value == null)
                throw Error(Previous(), "Literal nulo en expresión.");

            return new LiteralExpression(value);
        }

        if (Match(TokenType.IDENTIFIER))
            return new VariableExpression(Previous().Lexeme);

        if (Match(TokenType.GET_ACTUAL_X))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'GetActualX'.");
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' despues de '('.");
            return new CallExpression("GetActualX", new List<Expression>());
        }

        if (Match(TokenType.GET_ACTUAL_Y))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'GetActualY'.");
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' despues de '('.");
            return new CallExpression("GetActualY", new List<Expression>());
        }

        if (Match(TokenType.GET_CANVAS_SIZE))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'GetCanvasSize'.");
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' despues de '('.");
            return new CallExpression("GetCanvasSize", new List<Expression>());
        }
            
        if (Match(TokenType.GET_COLOR_COUNT))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'GetColorCount'.");
            Expression color = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' despues del color.");
            Expression x1 = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
            Expression y1 = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
            Expression x2 = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' despues de la coordenada)");
            Expression y2 = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ',' despues de la coordenada)");
            return new CallExpression("GetColorCount", [color, x1, y1, x2, y2]);
        }

        if (Match(TokenType.IS_BRUSH_COLOR))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'IsBrushColor'.");
            Expression color = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' despues del color)");
            return new CallExpression("IsBrushColor", [color]);
        }

        if (Match(TokenType.IS_BRUSH_SIZE))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' despues de 'IsBrushSize'.");
            Expression size = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' despues del color)");
            return new CallExpression("IsBrushSize", [size]);
        }

        if (Match(TokenType.IS_CANVAS_COLOR))
        {
            Consume(TokenType.LEFT_PAREN, "Se esperaba '(' después de 'IsCanvasColor'.");
            Expression color = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' después del color.");
            Expression x = ParseExpression();
            Consume(TokenType.COMMA, "Se esperaba ',' después de la coordenada X.");
            Expression y = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, "Se esperaba ')' al final.");
            return new CallExpression("IsCanvasColor", [color, x, y]);
        }

        throw Error(Peek(), "Se esperaba una expresión.");
    }
}