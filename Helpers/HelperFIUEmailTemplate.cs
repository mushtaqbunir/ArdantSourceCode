using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public class HelperFIUEmailTemplate
    {
        public static string GetCreatedTempalte()
        {
            var sb = new StringBuilder();
            var html = string.Empty;
            sb.AppendLine("Hi Audit Team, <br/><br/>");
            sb.AppendLine("A new FIU Escalation has been raised, please process it further.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Thanks,<br/>");
            sb.AppendLine("FGC IT Team<br/>");
            sb.AppendLine("<img style='width:85px' src='https://www.fgcerp.com/images/fgcLogo.png' />");
            html = sb.ToString();
            return html;
        }
        public static string GetDeclinedTempalte()
        {
            var sb = new StringBuilder();
            var html = string.Empty;
            sb.AppendLine("Hi Audit Team, <br/><br/>");
            sb.AppendLine("A new FIU Escalation has been declined.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Thanks,<br/>");
            sb.AppendLine("FGC IT Team<br/>");
            sb.AppendLine("<img style='width:85px' src='https://www.fgcerp.com/images/fgcLogo.png' />");
            html = sb.ToString();
            return html;
        }
        public static string GetForwaredTempalte(string Role)
        {
            var sb = new StringBuilder();
            var html = string.Empty;
            sb.AppendLine($"Hi {Role}, <br/><br/>");
            sb.AppendLine("A new FIU Escalation has been forwarded to you, please process it further.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Thanks,<br/>");
            sb.AppendLine("FGC IT Team<br/>");
            sb.AppendLine("<img style='width:85px' src='https://www.fgcerp.com/images/fgcLogo.png' />");
            html = sb.ToString();
            return html;
        }
        public static string GetCACTempalte()
        {
            var sb = new StringBuilder();
            var html = string.Empty;
            sb.AppendLine("Hi Audit Team, <br/><br/>");
            sb.AppendLine("FIU Escalation has been returned to CAC, please process it further.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Thanks,<br/>");
            sb.AppendLine("FGC IT Team<br/>");
            sb.AppendLine("<img style='width:85px' src='https://www.fgcerp.com/images/fgcLogo.png' />");
            html = sb.ToString();
            return html;
        }
    }
}
