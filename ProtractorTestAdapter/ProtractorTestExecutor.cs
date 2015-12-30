using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ProtractorTestAdapter
{
    [ExtensionUri(ProtractorTestExecutor.ExecutorUriString)]
    public class ProtractorTestExecutor : ITestExecutor
    {
        #region Constants

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public const string ExecutorUriString = "executor://ProtractorTestExecutor";

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public static readonly Uri ExecutorUri = new Uri(ProtractorTestExecutor.ExecutorUriString);

        /// <summary>
        /// specifies whether execution is cancelled or not
        /// </summary>
        private bool m_cancelled;

        #endregion

        #region ITestExecutor
        public void RunTests(IEnumerable<string> sources, IRunContext runContext,
          IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running from process:" + Process.GetCurrentProcess() + " ID:" + Process.GetCurrentProcess().Id.ToString());
            foreach(var source in sources)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Finding tests in source:" + source);
            }
            
            IEnumerable<TestCase> tests = ProtractorTestDiscoverer.GetTests(sources, null);
            foreach(var test in tests)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Found test:" + test.DisplayName);
            }
            RunTests(tests, runContext, frameworkHandle);
        }
        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="tests">Tests to be run.</param>
        /// <param name="runContext">Context to use when executing the tests.</param>
        /// <param param name="frameworkHandle">Handle to the framework to record results and to do framework operations.</param>
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            m_cancelled = false;

            foreach (TestCase test in tests)
            {
                if (m_cancelled)
                {
                    break;
                }
                frameworkHandle.RecordStart(test);
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Starting external test for " + test.DisplayName);
                var testOutcome = RunExternalTest(test, runContext, frameworkHandle,test);
                
                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test result:" + testOutcome.Outcome.ToString());

                frameworkHandle.RecordResult(testOutcome);
            }
        }

        private TestResult RunExternalTest(TestCase test, IRunContext runContext, IFrameworkHandle frameworkHandle, TestCase testCase)
        {
            var resultFile = RunProtractor(test, runContext, frameworkHandle);
            var  testResult = GetResultsFromJsonResultFile(resultFile, testCase);

            // clean the temp file

            File.Delete(resultFile);

            return testResult;
        }

        public static TestResult GetResultsFromJsonResultFile(string resultFile, TestCase testCase)
        {
            var jsonResult = "";
            if (File.Exists(resultFile))
            {

                using (var stream = File.OpenRead(resultFile))
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        jsonResult = textReader.ReadToEnd();
                    }
                    stream.Close();
                }
            }

            var results = JsonConvert.DeserializeObject<List<ProtractorResult>>(jsonResult);
            var resultOutCome = new TestResult(testCase);
            resultOutCome.Outcome = TestOutcome.Passed;
            foreach (var result in results)
            {
                foreach (var assert in result.assertions)
                {
                    if (!assert.passed)
                    {
                        resultOutCome.Outcome = TestOutcome.Failed;
                        resultOutCome.ErrorStackTrace = assert.stackTrace;
                        resultOutCome.ErrorMessage = assert.errorMsg;
                    }
                }
            }

            return resultOutCome;
        }

        private string RunProtractor(TestCase test, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var resultFile = Path.GetFileNameWithoutExtension(test.Source);
            resultFile += ".result.json";
            
            resultFile = Path.Combine(Path.GetTempPath(), resultFile);
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "result file: " + resultFile);
           
            ProcessStartInfo info = new ProcessStartInfo()
            {
                Arguments = string.Format("--resultJsonOutputFile \"{0}\" --specs \"{1}\" --framework jasmine", resultFile, test.Source),
                FileName = "protractor.cmd"
            };

            frameworkHandle.SendMessage(TestMessageLevel.Informational, "starting protractor with arguments:" + info.Arguments);


            Process p = new Process();
            p.StartInfo = info;
            p.Start();
            p.WaitForExit();

            return resultFile;
        }

        /// <summary>
        /// Cancel the execution of the tests
        /// </summary>
        public void Cancel()
        {
            m_cancelled = true;
        }


        #endregion

    }

}
