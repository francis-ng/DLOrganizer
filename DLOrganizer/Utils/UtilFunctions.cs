using System.Text;

namespace DLOrganizer.Utils
{
    static class StringExtension
    {
        public static string RemoveWhitespace(this string str)
        {
            var sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string FilterHTMLSpaces(this string str)
        {
            string temp = str.Replace("&nbsp", "");
            return temp.Replace(";;", ";");
        }
    }
}
