using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler;

public abstract class Expression
{
    // Clase base abstracta para todas las expresiones
}

public class LiteralExpression : Expression
{
    public object Value { get; }

    public LiteralExpression(object value)
    {
        Value = value;
    }
}

public class VariableExpression : Expression
{
    public string Name { get; }

    public VariableExpression(string name)
    {
        Name = name;
    }
}

public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public BinaryExpression(Expression left, Token op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}

public class GroupingExpression : Expression
{
    public Expression Inner { get; }

    public GroupingExpression(Expression inner)
    {
        Inner = inner;
    }
}

public class CallExpression : Expression
{
    public string FunctionName { get; }
    public List<Expression> Arguments { get; }

    public CallExpression(string functionName, List<Expression> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }
}