using RazorLight;
using RazorPlayground;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();

app.MapRazorPages();

var razorEngine = new RazorLightEngineBuilder().Build();

var razorCompiler = new RazorCompiler();

app.MapGet("/", () => "Hello World!");

app.MapGet("/razor/{input}", (int input) => razorEngine.CompileRenderStringAsync("key1", """
@for (var i = 0; i < @Model; i++)
{ 
    @: Hi #@i!
}
""", input));

var razorTemplate = await razorCompiler.CompileAsync("""
<html>
@for (var i = 0; i < @Model; i++)
{ 
    <p>Hi #@i!</p>
}
</html>
""");

app.MapGet("/razor-result/{input}", (int input) => razorTemplate.RenderResult(input));
app.MapGet("/razor-string-result/{input}", (int input) => razorTemplate.RenderStringResult(input));
app.MapGet("/razor-pooled-result/{input}", (int input) => razorTemplate.RenderPooledResult(input));
app.MapGet("/razor-pooled-string-result/{input}", (int input) => razorTemplate.RenderPooledStringResult(input));

app.MapGet("/hand-rolled/{input}", (int input) => new HandRolledResult(input));

app.Run();
