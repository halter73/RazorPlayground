using RazorLight;
using RazorPlayground;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var razorEngine = new RazorLightEngineBuilder()
                //.UseEmbeddedResourcesProject(typeof(Program))
                //.UseMemoryCachingProvider()
                .Build();

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

app.MapGet("/razor-result/{input}", (int input) => razorTemplate.Render(input));

//app.MapGet("/razor2/{input}", (int input) => razorEngine.CompileRenderStringAsync("key1", """
//@for (var i = 0; i < @Model; i++)
//{ 
//    @: Hello #@i!
//}
//""", input));

app.Run();
