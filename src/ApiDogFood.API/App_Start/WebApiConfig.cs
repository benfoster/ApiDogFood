using Newtonsoft.Json.Serialization;
using System;
using System.Web.Http;
using Thinktecture.IdentityModel.Http.Cors;
using Thinktecture.IdentityModel.Http.Cors.WebApi;
using Thinktecture.IdentityModel.Tokens.Http;

namespace ApiDogFood.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.ApplyTo(
                ConfigureAuth,
                ConfigureCors,
                ConfigureFilters,
                ConfigureFormatters,
                ConfigureRoutes
            );
        }

        private static void ConfigureAuth(HttpConfiguration config)
        {
            var authConfig = new AuthenticationConfiguration
            {
                DefaultAuthenticationScheme = "Basic",
                EnableSessionToken = true,
                SendWwwAuthenticateResponseHeader = true,
                RequireSsl = false 
            };

            authConfig.AddBasicAuthentication((username, password) =>
            {
                return username == "admin" && password == "password";
            });

            config.MessageHandlers.Add(new AuthenticationHandler(authConfig));
        }

        private static void ConfigureCors(HttpConfiguration config)
        {
            var corsConfig = new WebApiCorsConfiguration();
            config.MessageHandlers.Add(new CorsMessageHandler(corsConfig, config));

            corsConfig
                .ForAllOrigins()
                .AllowAllMethods()
                .AllowAllRequestHeaders(); 
        }

        private static void ConfigureFilters(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ValidateModelStateFilter());
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();
        }

        private static void ConfigureRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void ApplyTo<T>(this T source, params Action<T>[] targets)
        {
            foreach (var target in targets)
            {
                target(source);
            }
        }
    }
}
