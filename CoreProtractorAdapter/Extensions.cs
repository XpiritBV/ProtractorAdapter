namespace ProtractorTestAdapter
{
    public static class Extensions
    {
        public static bool Like(this string str, string pattern) => DotNet.Globbing.Glob.Parse(pattern).IsMatch(str);
    }
}
