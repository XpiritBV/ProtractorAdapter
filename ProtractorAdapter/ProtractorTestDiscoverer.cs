using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            //if (Debugger.IsAttached) Debugger.Break();
            //else Debugger.Launch();
            string baseDir = Environment.CurrentDirectory;
            try
            {
                baseDir = XElement.Parse(discoveryContext.RunSettings.SettingsXml).Element("RunConfiguration").Element("SolutionDirectory").Value;
            } catch (Exception) { }
            logger.SendMessage(TestMessageLevel.Informational, Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id.ToString());
            foreach (var source in sources)
            {
                logger.SendMessage(TestMessageLevel.Informational, source);
            }

            try
            {
                GetTests(sources, discoverySink, baseDir);
            }
            catch (Exception e)
            {
                logger.SendMessage(TestMessageLevel.Error, "Framework: Exception thrown during test discovery: " + e.Message);
            }

        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink, string baseDir = "")
        {
            var tests = new List<TestCase>();
            
            foreach (string source in sources)
            {
                var TestNames = GetTestNameFromFile(source, baseDir);
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

        private static string GetLongestCommonPrefix(string[] directories)
        {
            if (directories == null || directories.Length == 0)
                return String.Empty;
            char SEPARATOR = Path.DirectorySeparatorChar;
            string[] prefixParts =
                directories.Select(dir => dir.Split(SEPARATOR))
                .Aggregate(
                    (first, second) => first.Zip(second, (a, b) =>
                                            new { First = a, Second = b })
                                        .TakeWhile(pair => pair.First.Equals(pair.Second))
                                        .Select(pair => pair.First)
                                        .ToArray()
                );
            return string.Join(SEPARATOR.ToString(), prefixParts);
        }

        private static Dictionary<string, int> GetTestNameFromFile(string source, string baseDir = "")
        {
            switch(AppConfig.Framework)
            {
                case TestFramework.Jasmine:
                    return GetTestNameFromFileJS(source);
                default:
                    var common = GetLongestCommonPrefix(new string[] { baseDir, source });
                    var reduced = source.Substring(common.Length);
                    if (reduced.StartsWith(Path.DirectorySeparatorChar.ToString())) reduced = reduced.Substring(1);
                    return new Dictionary<string, int>
                    {
                        { Path.GetFileNameWithoutExtension(source), 0 }
                    };
            }
        }

        private const string DescribeToken = "describe('";
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
