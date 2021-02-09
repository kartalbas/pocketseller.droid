using System.Text.RegularExpressions;

namespace pocketseller.core.Tools
{
    public class CChecker
    {
        public static bool IsEmail(string strEmail)
        {
            Regex objRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match objMatch = objRegex.Match(strEmail);
            return objMatch.Success ? true : false;
        }
    }
}
