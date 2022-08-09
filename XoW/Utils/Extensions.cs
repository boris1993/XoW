using System.Text;

namespace XoW.Utils
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string str) =>
            string.IsNullOrWhiteSpace(str)
                ? str
                : string.Concat(str[0].ToString().ToUpper(), str.Substring(1));

        public static string ToHexString(this byte[] hexBytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var outputByte in hexBytes)
            {
                stringBuilder.Append(outputByte.ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}
