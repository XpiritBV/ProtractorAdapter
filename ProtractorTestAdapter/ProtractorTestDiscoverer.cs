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

namespace ProtractorTestAdapter
{
    [FileExtension(".js")]
    [DefaultExecutorUri(ProtractorTestExecutor.ExecutorUriString)]
    public class ProtractorTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            foreach (var source in sources)
            {
                Debugger.Break();
                logger.SendMessage(TestMessageLevel.Error, source);
            }

            GetTests(sources, discoverySink);
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var tests = new List<TestCase>();

            foreach (string source in sources)
            {
                string TestName = GetTestNameFromFile(source);
                var testCase = new TestCase(TestName, ProtractorTestExecutor.ExecutorUri, source);
                        tests.Add(testCase);

                if (discoverySink != null)
                {
                    discoverySink.SendTestCase(testCase);
                }

            }
            return tests;
        }

        private static string GetTestNameFromFile(string source)
        {
            var testNames = new List<string>();
            if (File.Exists(source))
            {

                using (var stream = File.OpenRead(source))
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        while (!textReader.EndOfStream)
                        {
                            var resultLine = textReader.ReadLine();
                            if (resultLine != null && resultLine.Contains("describe('"))
                            {
                                var name = GetNameFromDescribeLine(resultLine);
                                testNames.Add(name);
                            }
                        }
                    }
                    stream.Close();
                }
            }
            return string.Join(", ", testNames);
        }

        private static string GetNameFromDescribeLine(string resultLine)
        {
            throw new NotImplementedException();
        }
    }
}
