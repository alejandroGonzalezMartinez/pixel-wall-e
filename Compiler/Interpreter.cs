namespace pixel_wall_e.Compiler;

public class Interpreter
{
    private readonly Dictionary<string, object> _variables = new();

    public static void Interpret(List<Statement> statements)
    {
        foreach (var stmt in statements)
        {
            Execute(stmt);
        }
    }

    private static void Execute(Statement stmt)
    {
        switch (stmt)
        {
            // logica con cada tipo de statement...

            default:
                throw new Exception($"No se puede ejecutar el statement {stmt.GetType()}");
        }
    }

    private object Evaluate(Expression expr)
    {
        return expr switch
        {
            LiteralExpression lit => lit.Value,
            VariableExpression varExpr => _variables.TryGetValue(varExpr.Name, out var val) ? val :
                throw new Exception($"Variable no definida: {varExpr.Name}"),

            //BinaryExpression bin => EvaluateBinary(bin),

            // Agrega soporte para otras expresiones aquí...

            _ => throw new Exception($"No se puede evaluar {expr.GetType()}")
        };
    }
}