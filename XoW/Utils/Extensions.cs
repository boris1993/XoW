namespace XoW.Utils
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : string.Concat(str[0].ToString().ToUpper(), str.Substring(1));
        }
    }
}
