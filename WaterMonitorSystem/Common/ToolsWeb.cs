using System.Web;

namespace Common
{
    public class ToolsWeb
    {
        public static string GetIP(HttpRequest request)
        {
            if (request.ServerVariables["HTTP_VIA"] != null)
                return request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                return request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
