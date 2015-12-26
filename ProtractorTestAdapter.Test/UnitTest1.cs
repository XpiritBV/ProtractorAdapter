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

            var outcome = ProtractorTestExecutor.GetOutcomeFromJsonResultFile("TestResult.txt");
            Assert.IsTrue(outcome == TestOutcome.Passed);
        }
    }
}
