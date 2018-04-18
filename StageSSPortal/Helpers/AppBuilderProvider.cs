using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace StageSSPortal.Helpers
{
    public class AppBuilderProvider : IDisposable
    {
        private IAppBuilder _app;
        public AppBuilderProvider(IAppBuilder app)
        {
            _app = app;
        }
        public IAppBuilder Get() { return _app; }
        public void Dispose() { }
    }
}