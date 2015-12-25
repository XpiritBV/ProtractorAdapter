using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DanTup.TestAdapters;
using System.Reflection;

namespace ProtractorTestAdapter
{
    [FileExtension(".js")]
    [DefaultExecutorUri(ProtractorTestExecutor.ExecutorUriString)]
    public class ProtractorTestDiscoverer : TestDiscoverer
    {

        static readonly string extensionFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public override string ExtensionFolder { get { return extensionFolder; } }

        readonly ProtractorExternalTestExecutor executor = new ProtractorExternalTestExecutor();

        protected override ExternalTestExecutor ExternalTestExecutor { get { return executor; } }

        protected override Uri ExecutorUri { get { return ProtractorTestExecutor.ExecutorUri; } }




      }
}
