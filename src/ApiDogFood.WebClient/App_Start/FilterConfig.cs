using System.Web;
using System.Web.Mvc;

namespace ApiDogFood.WebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute()); // Secure by default
        }
    }
}