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
    [FileExtension(AppConfig.VSTestFileExtension)]
    [DefaultExecutorUri(ProtractorTestExecutor.ExecutorUriString)]
    public class ProtractorTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            logger.SendMessage(TestMessageLevel.Informational, Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id.ToString());
            foreach (var source in sources)
            {
                logger.SendMessage(TestMessageLevel.Informational, source);
            }

            try
            {
                GetTests(sources, discoverySink);
            }
            catch (Exception e)
            {
                logger.SendMessage(TestMessageLevel.Error, "Framework: Exception thrown during test discovery: " + e.Message);
            }

        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            //if(!Debugger.IsAttached)
            //        Debugger.Launch();
            var tests = new List<TestCase>();
            
            foreach (string source in sources)
            {
                var TestNames = GetTestNameFromFile(source);
                foreach (var testName in TestNames)
                {
                    var normalizedSource = source.ToLowerInvariant();
                    var testCase = new TestCase(testName.Key, ProtractorTestExecutor.ExecutorUri, normalizedSource);
                    tests.Add(testCase);
                    testCase.CodeFilePath = source;
                    testCase.LineNumber = testName.Value;

                    if (discoverySink != null)
                    {
                        discoverySink.SendTestCase(testCase);
                    }
                }
            }
            return tests;
        }

        private const string DescribeToken = "describe('";

        private static Dictionary<string, int> GetTestNameFromFile(string source)
        {
            switch(AppConfig.Framework)
            {
                case TestFramework.Jasmine:
                    return GetTestNameFromFileJS(source);
                default:
                    return new Dictionary<string, int>
                    {
                        { source, 0 }
                    };
            }
        }

        private static Dictionary<string, int> GetTestNameFromFileJS(string source)
        {
            var testNames = new Dictionary<string, int>();
            if (File.Exists(source))
            {
                int lineNumber = 1;
                using (var stream = File.OpenRead(source))
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        while (!textReader.EndOfStream)
                        {
                            var resultLine = textReader.ReadLine();
                            if (resultLine != null && resultLine.Contains(DescribeToken))
                            {
                                var name = GetNameFromDescribeLine(resultLine);
                                testNames.Add(name, lineNumber);
                            }
                            lineNumber++;
                        }
                    }
                    stream.Close();
                }
            }
            return testNames;
        }

        //TODO, get proper javascript parsing in place to find the description
        private static string GetNameFromDescribeLine(string resultLine)
        {
            //describe('angularjs homepage', function() { 
            int startIndex = resultLine.IndexOf(DescribeToken) + DescribeToken.Length;
            int endOfdescription = resultLine.IndexOf("',");

            var testname = resultLine.Substring(startIndex, endOfdescription - startIndex);
            return testname;
        }
    }
}
