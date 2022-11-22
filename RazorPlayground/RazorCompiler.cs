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
            var template = await _engine.CompileTemplateAsync(key);
            return new RazorTemplate(_engine, template);
        }
    }
}
