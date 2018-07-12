using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProtractorTestAdapter
{
    public enum TestFramework
    {
        None,
        Jasmine
    }
    public static class AppConfig
    {
        private static XElement _config;
        public static XElement config { get {
                if (_config != null) return _config;
                var path = Path.Combine(Environment.CurrentDirectory, "adapter.xml");
                return _config = (File.Exists(path) ? XElement.Parse(File.ReadAllText(path)) : null);
            }
        }
        public static string GetConfig(string element)
        {
            try
            {
                return config.Element(element).Value;
            }
            catch (Exception e) { return null; }
        }

        public const string VSTestFileExtension = ".feature"; // This can't be dynamic, we need to recompile the extension
        public static string Include { get => GetConfig("include") ?? $"**/*{VSTestFileExtension}"; }
        public static TestFramework Framework { get {
                TestFramework result;
                return Enum.TryParse<TestFramework>(GetConfig("framework"), true, out result) ? result : TestFramework.None;
            } }
        public static string Program { get => GetConfig("program") ?? "npm"; }
        public static string Arguments { get => GetConfig("arguments") ?? "run protractor --"; }
        public static string Exclude { get => GetConfig("exclude") ?? "node_modules"; }
    }
}
