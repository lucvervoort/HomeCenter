// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

// using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using MQTTnet.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HomeCenter.Mqtt.Server;

public class Startup
{
    private readonly Dictionary<string, string> _users = new() { { "sidlvet", "KrommeBeet55" } };

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint().WithKeepAlive())
            .AddMqttConnectionHandler()
            .AddConnections();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapConnectionHandler<MqttConnectionHandler>(
                "/mqtt",
                httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                                                       protocolList =>
                                                           protocolList.FirstOrDefault() ?? string.Empty);
        });

        app.UseMqttServer(server =>
        {
            // Todo: Do something with the server
            server.ValidatingConnectionAsync += Server_ValidatingConnectionAsync;
            server.AcceptNewConnections = true;            
            server.ClientConnectedAsync += Server_ClientConnectedAsync;
            server.ClientDisconnectedAsync += Server_ClientDisconnectedAsync;            
        });
    }

    private Task Server_ValidatingConnectionAsync(MQTTnet.Server.ValidatingConnectionEventArgs arg)
    {
        if (!string.IsNullOrWhiteSpace(arg.UserName) && !string.IsNullOrWhiteSpace(arg.Password))
        {
            if (!(_users.TryGetValue(arg.UserName, out var password) && password == arg.Password))
            {
                arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                return Task.CompletedTask;
            }
        }
        else if(string.IsNullOrWhiteSpace(arg.UserName) || string.IsNullOrWhiteSpace(arg.Password))
        {
            arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.NotAuthorized;
            return Task.CompletedTask; 
        }
        
        arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
        return Task.CompletedTask;
    }

    private Task Server_ClientDisconnectedAsync(MQTTnet.Server.ClientDisconnectedEventArgs arg)
    {
        Console.WriteLine($"Client disconnect {arg.ClientId}");
        return Task.CompletedTask;
    }

    private Task Server_ClientConnectedAsync(MQTTnet.Server.ClientConnectedEventArgs arg)
    {
        Console.WriteLine($"Client connect {arg.ClientId}");
        return Task.CompletedTask;
    }
}
