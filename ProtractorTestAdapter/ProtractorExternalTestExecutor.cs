using System;
using System.Collections.Generic;
using DanTup.TestAdapters;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ProtractorTestAdapter
{
    internal class ProtractorExternalTestExecutor : ExternalCommandTestExecutor
    {
        static readonly string extensionFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public override string ExtensionFolder { get { return extensionFolder; } }

        static readonly string protractorExecutable = Path.Combine(extensionFolder, "protractor");
        static readonly string testFrameworkFile = Path.Combine(extensionFolder, "conf.js");

        protected override ProcessStartInfo CreateProcessStartInfo(string source, string args)
        {
            args = string.Format("\"{0}\" \"{1}\"", 
                testFrameworkFile.Replace("\"", "\\\""),
                source.Replace("\"", "\\\""), args);

            return new ProcessStartInfo(protractorExecutable, args)
            {
                WorkingDirectory = Path.GetDirectoryName(source),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        public override IEnumerable<GenericTest> GetTestCases(string source, Action<string> logger)
        {
            return GetTests(source);
        }

        //public override IEnumerable<GenericTest> GetTestResults(string source, Action<string> logger)
        //{
        //    return  new List<GenericTest>{ new GenericTest() { Outcome = TestOutcome.Passed } };
        //}

        private const string DescribeToken = "describe('";

        internal static IEnumerable<GenericTest> GetTests(string source)
        {
            var tests = new List<GenericTest>();

                var TestNames = GetTestNameFromFile(source);
                foreach (var name in TestNames)
                {

                    var testCase = new GenericTest() { DisplayName = name, CodeFilePath = source };
                    tests.Add(testCase);

                }

                
            return tests;
        }

        private static List<string> GetTestNameFromFile(string source)
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
                            if (resultLine != null && resultLine.Contains(DescribeToken))
                            {
                                var name = GetNameFromDescribeLine(resultLine);
                                testNames.Add(name);
                            }
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