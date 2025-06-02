using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PixelWallE;
using Compiler;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient 
        { 
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
        });

        string sourceCode =  @"
            Spawn(0, 0)
            Color(""Black"")
            n <- 5
            k <- 3 + 3 * 10
            n <- k * 2
            actual_x <- GetActualX()
            i <- 0
            loop1
            DrawLine(1, 0, 1)
            i <- i + 1
            GoTo [loop1] (i < 10)
            Color(""Blue"")
            GoTo [loop1] (1 == 1)
            loop_ends_here
        "; 

        Scanner scanner = new Scanner(sourceCode);
        List<Token> tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token.Type); 
        }

        // Parseo de tokens
        Parser parser = new Parser(tokens);
        List<Statement> statements = parser.Parse();

        foreach (Statement stmt in statements)
        {
            Console.WriteLine(stmt);
        }

        await builder.Build().RunAsync();
    }
}