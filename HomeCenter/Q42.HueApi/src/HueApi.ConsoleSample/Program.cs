// See https://aka.ms/new-console-template for more information
using HueApi;
using HueApi.Models;
using HueApi.Models.Responses;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

/*
 Follow 3 Easy Steps
Step 1

First make sure your bridge is connected to your network and is functioning properly. Test that the smartphone app can control the lights on the same network.

Step 2

Then you need to discover the IP address of the bridge on your network. You can do this in a few ways.

NOTE – When you are ready to make a production app, you need to discover the bridge automatically using Hue Bridge Discovery Guide.

1. Use an mDNS discovery app to find Philips hue in your network.
2. Use our broker server discover process by visiting https://discovery.meethue.com

=> [{"id":"001788fffe2b44cd","internalipaddress":"192.168.1.15","port":443},{"id":"001788fffe20bd7f","internalipaddress":"192.168.0.179","port":443}]

3. Log into your wireless router and look Philips hue up in the DHCP table.
4. Hue App method: Download the official Philips hue app. Connect your phone to the network the hue bridge is on. Start the hue app. Push link connect to the bridge. Use the app to find the bridge and try controlling lights. All working — Go to the settings menu in the app. Go to Hue Bridges. Select your bridge. The ip address of the bridge will show.

Step 3

Once you have the address load the test app by visiting the following address in your web browser.

https://<bridge ip address>/debug/clip.html
You should see an interface.

https://192.168.1.15/debug/clip.html:  {"devicetype":"lvhome#hueupstairs"}

[
	{
		"success": {
			"username": "w7G-n8c5cWdwXSMzn2C0X1fyJ0CyAGmwcV8s-dCz"
		}
	}
]


https://192.168.0.179/debug/clip.html: {"devicetype":"lvhome#huedownstairs"}

[
	{
		"success": {
			"username": "Xj9OWvQTPvvQLKkGm2uRX9t8-cMHseznTkpYEztA"
		}
	}
]

url: /api
body: see above
method: POST ... AFTER PRESS ON BUTTON!

=> use username as key
 */

// https://github.com/philipdaubmeier/GraphIoT

internal class Program
{
  private static readonly Dictionary<string, string> _ipNames = new() { { "", "unknown" }, { "192.168.0.179", "downstairs" }, { "192.168.1.15", "upstairs" } };
  private static readonly Dictionary<string, string> _deviceNames = new() { };

  // "/groups/15", "/groups/0"
  private static void PrintChildren(HueResponse<HueResource> responseResourcesDownstairs, Guid? owner, int level)
  {
    var children = responseResourcesDownstairs.Data.Where(x => x.Owner?.Rid == owner);

    foreach (var child in children)
    {
      string spaces = new(' ', level);
      Console.WriteLine(spaces + $" - {child.Type}, {child.IdV1}");

      PrintChildren(responseResourcesDownstairs, child.Id, level + 1);
    }
  }

  private static void EventStreamMessage(string bridgeIp, List<EventStreamResponse> events)
  {
    try
    {
      var loc = _ipNames[bridgeIp];
      // Console.WriteLine($"{DateTimeOffset.UtcNow} | {events.Count} new events");

      foreach (var hueEvent in events)
      {
        if (hueEvent.Data.Count <= 0)
        {
          Console.WriteLine("Data?");
        }
        else
        {
          foreach (var data in hueEvent.Data)
          {
            // Console.WriteLine($"Bridge IP: {bridgeIp} | Data: {data.Metadata?.Name} / {data.IdV1}");
            var dn = "?";
            if (!string.IsNullOrEmpty(data.IdV1) && _deviceNames != null && _deviceNames.TryGetValue("/"+loc+data.IdV1, out string? value))
              dn = value;
            switch (data.Type)
            {
              case "light_level":
                data.ExtensionData["light"].GetProperty("light_level").TryGetDecimal(out decimal l);
                Console.WriteLine($"{loc}: light level {l} on /{loc}{data.IdV1} ({dn})");
                break;
              case "light":
                Console.WriteLine($"{_ipNames[bridgeIp]}: light on /{loc}{data.IdV1} ({dn})");
                break;
              case "temperature":
                data.ExtensionData["temperature"].GetProperty("temperature").TryGetDecimal(out decimal t);
                Console.WriteLine($"{loc}: temperature {t} on /{loc}{data.IdV1} ({dn})");
                break;
              case "motion":
                Console.WriteLine($"{loc}: motion on /{loc}{data.IdV1} ({dn})");
                break;
              case "grouped_motion":
                // Console.WriteLine($"{_ipNames[bridgeIp]}: grouped motion");
                break;
              default:
                Console.WriteLine($"{loc}: unprocessed type {data.Type} ({dn})");
                break;
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString());
    }   
  }

  private static async Task Main(string[] args)
  {
    //var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
    //var config = builder.Build();

    string ipDownstairs = "192.168.0.179";
    string keyDownstairs = "Xj9OWvQTPvvQLKkGm2uRX9t8-cMHseznTkpYEztA";
    string ipUpstairs = "192.168.1.15";
    string keyUpstairs = "w7G-n8c5cWdwXSMzn2C0X1fyJ0CyAGmwcV8s-dCz";

    Console.WriteLine($"Connecting to {ipDownstairs} with key: {keyDownstairs}");
    var localHueClientDownstairs = new LocalHueApi(ipDownstairs, keyDownstairs);

    Console.WriteLine($"Connecting to {ipUpstairs} with key: {keyUpstairs}");
    var localHueClientUpstairs = new LocalHueApi(ipUpstairs, keyUpstairs);

    // Console.WriteLine("Getting all resources...");

    var resourcesDownstairs = await localHueClientDownstairs.GetResourcesAsync();
    // var rootsDownstairs = resourcesDownstairs.Data.Where(x => x.Owner == null);

    var resourcesUpstairs = await localHueClientUpstairs.GetResourcesAsync();
    // var rootsUpstairs = resourcesUpstairs.Data.Where(x => x.Owner == null);

    // Console.WriteLine("DOWNSTAIRS");
    // Console.WriteLine("----------");

    // PrintChildren(resourcesDownstairs, null, 0);

    var devicesDownstairs = await localHueClientDownstairs.GetDevicesAsync();
    foreach (var device in devicesDownstairs.Data)
    {
      if(!string.IsNullOrEmpty(device.IdV1) && device.Metadata != null)
        _deviceNames.Add("/" + _ipNames[ipDownstairs] + device.IdV1, device.Metadata.Name);
    }
     
    // Console.WriteLine("UPSTAIRS");
    // Console.WriteLine("----------");

    // PrintChildren(resourcesUpstairs, null, 0);

    var devicesUpstairs = await localHueClientUpstairs.GetDevicesAsync();
    foreach (var device in devicesUpstairs.Data)
    {
      if (!string.IsNullOrEmpty(device.IdV1) && device.Metadata != null)
        _deviceNames.Add("/" + _ipNames[ipUpstairs] + device.IdV1, device.Metadata.Name);
    }

    localHueClientDownstairs.OnEventStreamMessage += EventStreamMessage;
    localHueClientDownstairs.StartEventStream();

    localHueClientUpstairs.OnEventStreamMessage += EventStreamMessage;
    localHueClientUpstairs.StartEventStream();

    Console.WriteLine("Waiting for Hue Bridge events (press any key to stop)...");

    //await Task.Delay(TimeSpan.FromHours(1));

    Console.ReadLine();
    localHueClientDownstairs.StopEventStream();
    localHueClientUpstairs.StopEventStream();

    Console.WriteLine("Stopped listening for Hue Bridge events...");

    Console.ReadLine();
  }
}
