using System.Collections.Concurrent;
using System.Diagnostics;
using RazorLight;

namespace RazorPlayground
{
    public class RazorTemplate
    {
        private RazorLightEngine _engine;
        private string _key;

        private ConcurrentQueue<ITemplatePage> _templatePool = new();

        public RazorTemplate(RazorLightEngine engine, string key)
        {
            _engine = engine;
            _key = key;
        }

        public ITemplatePage Template
        {
            get
            {
                var templateTask = _engine.CompileTemplateAsync(_key);
                Debug.Assert(templateTask.IsCompletedSuccessfully);
                return templateTask.GetAwaiter().GetResult();
            }
        }

        public IResult RenderResult<T>(T model)
        {
            return new RazorResult<T>(_engine, Template, model);
        }

        public IResult RenderStringResult<T>(T model)
        {
            return new RazorStringResult<T>(_engine, Template, model);
        }

        public IResult RenderPooledResult<T>(T model)
        {
            return new RazorPooledResult<T>(this, model);
        }

        public IResult RenderPooledStringResult<T>(T model)
        {
            return new RazorPooledStringResult<T>(this, model);
        }

        private class RazorResult<T> : IResult
        {
            private RazorLightEngine _engine;
            private ITemplatePage _template;
            private T _model;

            public RazorResult(RazorLightEngine engine, ITemplatePage template, T model)
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

        private class RazorStringResult<T> : IResult
        {
            private RazorLightEngine _engine;
            private ITemplatePage _template;
            private T _model;

            public RazorStringResult(RazorLightEngine engine, ITemplatePage template, T model)
            {
                _engine = engine;
                _template = template;
                _model = model;
            }

            public async Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.ContentType = "text/html";

                var body = await _engine.RenderTemplateAsync(_template, _model);
                await httpContext.Response.WriteAsync(body);
            }
        }

        private class RazorPooledResult<T> : IResult
        {
            private RazorTemplate _parent;
            private T _model;

            public RazorPooledResult(RazorTemplate template, T model)
            {
                _parent = template;
                _model = model;
            }

            public async Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.ContentType = "text/html";

                if (!_parent._templatePool.TryDequeue(out var template))
                {
                    template = _parent.Template;
                }
                
                await using var textWriter = new StreamWriter(httpContext.Response.Body);
                await _parent._engine.RenderTemplateAsync(template, _model, textWriter);

                _parent._templatePool.Enqueue(template);
            }
        }


        private class RazorPooledStringResult<T> : IResult
        {
            private RazorTemplate _parent;
            private T _model;

            public RazorPooledStringResult(RazorTemplate template, T model)
            {
                _parent = template;
                _model = model;
            }

            public async Task ExecuteAsync(HttpContext httpContext)
            {
                httpContext.Response.ContentType = "text/html";

                if (!_parent._templatePool.TryDequeue(out var template))
                {
                    template = _parent.Template;
                }
                
                var body = await _parent._engine.RenderTemplateAsync(template, _model);
                await httpContext.Response.WriteAsync(body);

                _parent._templatePool.Enqueue(template);
            }
        }
    }
}
