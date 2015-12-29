using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ProtractorTestAdapter.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DeploymentItem("TestResult.txt")]
        public void TestJsonOutcomeParsing()
        {
            TestCase tc = new TestCase("Test", ProtractorTestExecutor.ExecutorUri, "bla");
            var outcome = ProtractorTestExecutor.GetResultsFromJsonResultFile("TestResult.txt",tc);
            Assert.IsTrue(outcome.Outcome == TestOutcome.Passed);
        }
    }
}
