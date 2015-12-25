using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using ProtractorTestAdapter.EventWatchers;
using ProtractorTestAdapter.EventWatchers.EventArgs;
using DanTup.TestAdapters;

namespace ProtractorTestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class ProtractorTestContainerDiscoverer : TestContainerDiscoverer
    {
        protected override string[] TestContainerFileExtensions { get { return new[] { ".js" }; } }
        protected override string[] WatchedFilePatterns { get { return new[] {  "*.js" }; } }

        [ImportingConstructor]
        public ProtractorTestContainerDiscoverer([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
        }

        public override Uri ExecutorUri
        {
            get { return ProtractorTestExecutor.ExecutorUri; }
        }
    }

}
