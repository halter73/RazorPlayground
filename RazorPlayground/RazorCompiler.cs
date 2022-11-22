using RazorLight;

namespace RazorPlayground
{
    public class RazorCompiler
    {
        private RazorLightEngine _engine = new RazorLightEngineBuilder().Build();

        public async Task<RazorTemplate> CompileAsync(string templateString)
        {
            var key = Guid.NewGuid().ToString();
            _engine.Options.DynamicTemplates[key] = templateString;

            // Precompile. This will be cached for later using the key.
            _ = await _engine.CompileTemplateAsync(key);
            return new RazorTemplate(_engine, key);
        }
    }
}
