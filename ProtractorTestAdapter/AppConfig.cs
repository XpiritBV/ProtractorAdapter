using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtractorTestAdapter
{
    public enum TestFramework
    {
        None,
        Jasmine
    }
    public static class AppConfig
    {
        public static IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("adapter.json", optional: true).Build();

        public const string VSTestFileExtension = ".feature"; // This can't be dynamic, we need to recompile the extension
        public static string FileExtension { get => config["extension"] ?? VSTestFileExtension; }
        public static TestFramework Framework { get {
                TestFramework result;
                return Enum.TryParse<TestFramework>(config["framework"], true, out result) ? result : TestFramework.None;
            } }
        public static string Program { get => config["program"] ?? "npm"; }
        public static string Arguments { get => config["arguments"] ?? "run protractor --"; }
    }
}
