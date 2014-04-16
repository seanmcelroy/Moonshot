namespace Moonshot.Server
{
    using Owin;
    using System.Net.Http.Headers;
    using System.Web.Http;
    
    public class Startup 
    { 
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder) 
        { 
            // Configure Web API for self-host. 
            var config = new HttpConfiguration(); 
            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}", 
                defaults: new { id = RouteParameter.Optional } 
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            appBuilder.UseWebApi(config);
        } 
    } 
} 