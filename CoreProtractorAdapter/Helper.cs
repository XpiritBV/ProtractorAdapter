using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtractorAdapter
{
    public static class Helper
    {
        public static string GetLongestCommonPrefix(string[] directories)
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

        public static string FindPackageJson(string source)
        {
            DirectoryInfo directory = File.Exists(source) && !Directory.Exists(source) ? Directory.GetParent(source) : new DirectoryInfo(source);
            while (directory != null && directory.GetFiles("package.json").Length == 0)
            {
                directory = directory.Parent;
            }
            return directory == null ? null : directory.FullName;
        }
        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                        {
                            exe = path;
                            break;
                        }
                    }
                }
                else throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            exe = Path.GetFullPath(exe);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (File.Exists(exe + ".exe")) exe += ".exe";
                else if (File.Exists(exe + ".bat")) exe += ".bat";
                else if (File.Exists(exe + ".cmd")) exe += ".cmd";
            }
            return exe;
        }
    }
}
