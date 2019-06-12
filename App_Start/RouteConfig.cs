using System.Web.Mvc;
using System.Web.Routing;

namespace CSCLite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Import_1",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "ImportCSV", action = "Index", id = UrlParameter.Optional }
             );
        }
    }
}
