using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using MQTTnet.Server;
using ViewModelsSamples.Lines.AutoUpdate;

namespace MauiSample.Lines.AutoUpdate;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool? _isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, System.EventArgs e)
    {
        var vm = (ViewModel)BindingContext;

        _isStreaming = _isStreaming is null ? true : !_isStreaming;

        while (_isStreaming.Value)
        {
            vm.RemoveItem();
            vm.AddItem();
            await Task.Delay(1000);
        }
    }

    /*
      {"docType":"jts","version":"1.0","header":{"startTime":"2024-07-12T16:17:24.232662+02:00","endTime":"2024-07-12T16:17:24.2326693+02:00","recordCount":1,"columns":{"0":{"id":"f6edf0ec-a078-4d03-90d6-14fdd32f19b2","name":"Temperature","dataType":"NUMBER","renderType":"VALUE","format":"0.###","aggregate":"NONE"}}},"data":[{"ts":"2024-07-12T16:17:24.2330232+02:00","f":{"0":{"v":17.89,"q":100,"a":"SensorFrontDoorOutside"}}}]}
     */

    private async void ButtonOutdoorTemperatureFrontdoor_Clicked(object sender, EventArgs e)
    {
        var vm = (ViewModel)BindingContext;

        _isStreaming = _isStreaming is null ? true : !_isStreaming;

        /*
        while (_isStreaming.Value)
        {
            //vm.RemoveItem();
            //vm.AddItem();
            System.Diagnostics.Debug.WriteLine("Waiting...");
            await Task.Delay(1000);
        }
        */
    }
}
