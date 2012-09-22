using System.Web.Mvc;
using System.Web.Routing;

namespace CloudConfigCrypto.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default",
                "{controller}/{action}/{id}",
                new
                    {
                        controller = "CloudConfigCrypto",
                        action = "Index",
                        id = UrlParameter.Optional
                    }
            );
        }
    }
}