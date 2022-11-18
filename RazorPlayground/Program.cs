using RazorLight;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var razorEngine = new RazorLightEngineBuilder()
                //.UseEmbeddedResourcesProject(typeof(Program))
                //.UseMemoryCachingProvider()
                .Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/razor/{input}", (int input) => razorEngine.CompileRenderStringAsync("key1", """
@for (var i = 0; i < @Model; i++)
{ 
    @: Hi #@i!
}
""", input));

app.MapGet("/razor2/{input}", (int input) => razorEngine.CompileRenderStringAsync("key1", """
@for (var i = 0; i < @Model; i++)
{ 
    @: Hello #@i!
}
""", input));


app.Run();
