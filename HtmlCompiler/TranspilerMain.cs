using HtmlCompiler;

public class TranspilerMain
{
    private static void Main(string[] args)
    {
        const string path = @"C:\Users\dougl\Documents\Dev\C#\HtmlCompiler\Files\input.yaml";

        if (!File.Exists(path))
        {
            Console.WriteLine($"Arquivo {path.Split("\\").LastOrDefault()} não encontrado!");
            return;
        }

        var lines = File.ReadAllLines(path);

        var htmlOutput = HTMLTranspiler.Transpile(lines);

        File.WriteAllText(@"C:\Users\dougl\Documents\Dev\C#\HtmlCompiler\Files\output.html", htmlOutput);
    }
}