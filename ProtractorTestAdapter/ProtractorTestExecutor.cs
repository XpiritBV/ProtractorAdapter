using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace ProtractorTestAdapter
{
//    [ExtensionUri(ProtractorTestExecutor.ExecutorUriString)]
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

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="sources">Path to files to look for tests in.</param>
        /// <param name="runContext">Context to use when executing the tests.</param>
        /// <param param name="frameworkHandle">Handle to the framework to record results and to do framework operations.</param>
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            List<TestCase> tests = new List<TestCase>();
            foreach (var source in sources)
            {
                new ProtractorExternalTestExecutor().GetTestCases(source, null);
            }

            frameworkHandle.SendMessage(TestMessageLevel.Error, "Running test");

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
                // Setup the test result as indicated by the test case.
                var testResult = new TestResult(test)
                {
                    Outcome = TestOutcome.Passed, //(TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome),
                    //ErrorMessage = (string)test.GetPropertyValue(TestResultProperties.ErrorMessage),
                    //ErrorStackTrace = (string)test.GetPropertyValue(TestResultProperties.ErrorStackTrace)
                };

                frameworkHandle.RecordResult(testResult);
            }
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
