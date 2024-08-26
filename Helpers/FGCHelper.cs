using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public class FGCHelper
    {
        public static string RemoveHtmlTags(string htmlString)
        {
            // Remove HTML tags and spaces using regular expressions
            string plainText = Regex.Replace(htmlString, "<.*?>", "");
            plainText = Regex.Replace(plainText, @"\s+", "");
            return plainText;
        }
        public static string RemoveHtmlTagsAndSpacesTags(string htmlString)
        {
            // Remove HTML tags and spaces using regular expressions
            string plainText = Regex.Replace(htmlString, "<.*?>", "");
            plainText = plainText.Replace("&nbsp;"," ");
            return plainText;
        }
    }
}
