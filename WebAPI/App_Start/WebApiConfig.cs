using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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


            var apiCallHistories = new ConcurrentDictionary<string, APICallHistory>();

            var cityApiRateLimiter = new RateLimiter.RateLimiter(2000, 1, apiCallHistories);
            var roomApiRateLimiter = new RateLimiter.RateLimiter(3000, 2, apiCallHistories);

            var cityApiRateLimiterHandler =
                new RateLimiterHandler(GlobalConfiguration.Configuration, cityApiRateLimiter);

            var roomApiRateLimiterHandler =
                new RateLimiterHandler(GlobalConfiguration.Configuration, roomApiRateLimiter);

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
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
