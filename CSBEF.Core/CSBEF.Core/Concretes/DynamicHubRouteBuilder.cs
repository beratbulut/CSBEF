using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSBEF.Core.Concretes
{
    public static class DynamicHubRouteBuilder
    {
        private static readonly MethodInfo MapHubMethod = typeof(HubRouteBuilder).GetMethod("MapHub", new[] { typeof(PathString) });

        public static HubRouteBuilder MapSignalrRoutes(this HubRouteBuilder hubRouteBuilder)
        {
            foreach (var module in GlobalConfiguration.Modules)
            {
                IEnumerable<Type> pluginHubTypes = module.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Hub)) && !t.IsAbstract);

                foreach (var pluginHubType in pluginHubTypes)
                {
                    var url = "/signalr/" + module.ShortName + "/" + pluginHubType.Name;
                    MapHubMethod.MakeGenericMethod(pluginHubType).Invoke(hubRouteBuilder, new object[] { new PathString(url) });
                }
            }

            hubRouteBuilder.MapHub<GlobalHub>("/signalr/GlobalHub");

            return hubRouteBuilder;
        }
    }
}