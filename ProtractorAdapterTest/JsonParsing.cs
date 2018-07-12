using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtractorTestAdapter;

namespace ProtractorAdapterTest
{
    [TestClass]
    public class JsonParsing
    {
        [TestMethod]
        [DeploymentItem("TestResult.txt")]
        public void TestJsonOutcomeParsing()
        {
            TestCase tc = new TestCase("Test", ProtractorTestExecutor.ExecutorUri, "bla");
            var outcome = ProtractorTestExecutor.GetResultsFromJsonResultFile("TestResult.txt", tc);
            Assert.IsTrue(outcome.Outcome == TestOutcome.Passed);
        }
    }
}
