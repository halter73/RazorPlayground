using RazorLight;

namespace RazorPlayground
{
    public class RazorTemplate
    {
        private RazorLightEngine _engine;
        private ITemplatePage _template;

        public RazorTemplate(RazorLightEngine engine, ITemplatePage template)
        {
            _engine = engine;
            _template = template;
        }

        public IResult Render<T>(T model)
        {
            return new RenderedRazorResult<T>(_engine, _template, model);
        }

        private class RenderedRazorResult<T> : IResult
        {
            private RazorLightEngine _engine;
            private ITemplatePage _template;
            private T _model;

            public RenderedRazorResult(RazorLightEngine engine, ITemplatePage template, T model)
            {
                _engine = engine;
                _template = template;
                _model = model;
            }

            public async Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.ContentType = "text/html";

                // We would not need to dispose the inner stream or explicitly flush via DisposeAsync
                // Not using "using" would save us an async state machine.
                await using var textWriter = new StreamWriter(httpContext.Response.Body);
                await _engine.RenderTemplateAsync(_template, _model, textWriter);
            }
        }
    }
}
