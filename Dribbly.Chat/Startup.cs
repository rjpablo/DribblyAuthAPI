﻿using Dribbly.Chat.Resolvers;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Owin;

namespace Dribbly.Chat
{
    public static class Startup
    {
        public static void MapSignalR(IAppBuilder app)
        {
            app.MapSignalR("/chatHub", new HubConfiguration());

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
        }
    }
}
