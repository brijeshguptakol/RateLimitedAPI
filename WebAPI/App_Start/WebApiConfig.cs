using System.Web.Http;
using WebAPI.RateLimiter;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var cityApiRateLimiter = new RateLimiter.RateLimiter(10000, 2);
            var roomApiRateLimiter = new RateLimiter.RateLimiter(10000, 3);

            var defaultApiRateLimiter = new RateLimiter.RateLimiter(10000, 20);

            var cityApiRateLimiterHandler =
                new RateLimiterHandler(GlobalConfiguration.Configuration, cityApiRateLimiter);

            var roomApiRateLimiterHandler =
                new RateLimiterHandler(GlobalConfiguration.Configuration, roomApiRateLimiter);

            var defaultApiRateLimiterHandler =
                new RateLimiterHandler(GlobalConfiguration.Configuration, defaultApiRateLimiter);

            config.Routes.MapHttpRoute(
                name: "CityApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, sortOrder = RouteParameter.Optional },
                constraints: null,
                handler: cityApiRateLimiterHandler
            );

            config.Routes.MapHttpRoute(
                name: "RoomAPI",
                routeTemplate: "api/{controller}/{id}/{sortOrder}",
                defaults: new { id = RouteParameter.Optional, sortOrder = RouteParameter.Optional },
                constraints: null,
                handler: roomApiRateLimiterHandler
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: defaultApiRateLimiterHandler
            );

        }
    }
}
