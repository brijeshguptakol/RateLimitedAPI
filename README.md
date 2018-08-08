# Rate Limited API 

## Approach
To rate limit an API call in ASP Dot NEt WEB API, I am using MessageHandler.
A message handler is a class that receives an HTTP request and returns an HTTP response. Message handlers derive from the abstract HttpMessageHandler class. We can define a message handler per route. So I am using this message handlers to intercept all request for a given route template.

Once the request is intercepted, I am using RateLimiter class to validate is the request is valid by comparing with previous api call histories. 

I am maintaining a thread safe dictionary (concurrentDictionary) with all APICallHistories. APICallHistory contains information like 
  1. LastCallTime, 
  2. CurrentCount and 
  3. AverageTime.
 
### Validation Algorithm
1. Try find and entry in ApiCallHistories with current request uri as key. 
2.  If history record exists, 
      Than
        calculate time elapsed since last call
        calculate new average time per call.
        If new average time per call is greater than minimum threashold
          Than
            set isAllowed to true
          Else
            set isAllowed to false
        If Time elapsed since last call is greater than time interval
          Than
            Reset History and set isAllowed to true
          Else
        Else
          Create a new history
3. Set the history in ApiCallHistories   
4. Return isAllowed. 

This whole logic is executed within a lock, to make this logic thread safe.

### How to use Rate Limiter

We need to put the below code in WebAPIConfig.cs

 
            var cityApiRateLimiter = new RateLimiter.RateLimiter(10000, 100);
            var roomApiRateLimiter = new RateLimiter.RateLimiter(10000, 1000);

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
          
