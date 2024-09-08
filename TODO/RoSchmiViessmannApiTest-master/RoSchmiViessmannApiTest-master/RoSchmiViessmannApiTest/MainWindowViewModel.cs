using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Net.Http;
using System.Windows;

// https://developer.viessmann.com/start.html?sessionid=1#token=eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzZXNzaW9uSWQiOiJmMWQyZDM4Yi0wZDgwLTRjMGYtOTg5OS0wOGY4YzkyOTUzZmQiLCJhcHBJZCI6ImRpZ2l0YWwtaHViIiwiaXNzIjoiYXBpLnZpZXNzbWFubi5jb20iLCJ4LXV1aWQiOiI1MGIxMGM5ZS00YzhlLTQ1ODEtOWFiZC03MjhjZGI2YzRiZGQiLCJjb21wYW55IjoiMTIwMjEzMjMzNiIsImlkZW50aXR5VHlwZSI6IkVLIiwibG9naW4iOiJsY3ZlcnZvb3J0QHlhaG9vLmNvbSIsInVzZXItbGFuZ3VhZ2UiOiJubCIsImlhdCI6MTcyMTU0OTYyNiwiZXhwIjoxNzIxNTg1NjI2fQ.ijlFh9oiiM5HvZiTt_D6w1W4C8Sl_3jstr0y-j0u0VquekJc4KuXtOZznfBogriZykFoRHCo-dPv0s7A696PfJkuaEHoTl-E5vZzImDsChuuEGCH4U5bJBSRl0xRGqytxAmNHYMxre5AVH6uk8BePNpoK2lOVZJW0nkL2ayiUisyjQtrHPSdRCM1_UcAojIk-z80Cv6YJzwOSTsyVSCcGSJmfsANw9rOYTksq0K7pqa1goCP_iDrLBgYsePiMh0xS7vyBhSCqyapRDRpEWZmxtwWXul4kseO1qaNlM_QWUjzsB_aEjv6BQbgxy2y5GjJCvWxXLA-wCU4GfTknRKkWg
// Access token: eyJraWQiOiI0ZWNhM2VkNi0yNDY2LTQ2YWUtYTRhNC02MjZiNGEwM2RhMGUiLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI1MGIxMGM5ZS00YzhlLTQ1ODEtOWFiZC03MjhjZGI2YzRiZGQiLCJjaC5hZG5vdnVtLm5ldmlzaWRtLnByb2ZpbGVJZCI6IjE0MTc1MSIsIm5iZiI6MTcyMTU0OTY5Miwic2NvcGUiOiJJb1Qgb3BlbmlkIiwiaXNzIjoiaHR0cHM6Ly9pYW0udmllc3NtYW5uLmNvbSIsImV4cCI6MTcyMTU1MzI5MiwiaWF0IjoxNzIxNTQ5NjkyLCJjbGllbnRfaWQiOiJhMWI1OTU0MTA2MDg1YWMzMGY5NWUxYjJiY2M5MGZmZiJ9.N-I2NrkIH-MLIRz56SJlnHDK-04N0fuPNFDabUfNRBKcrfgcltxmEYXEH8AX_llJ3yRbb8Boor6mHm0_BL3DUt5paJjackdsPLpxSGJeKHAwDjexGMQECaXlHPhcfXvXEzbVfOFPnaumi28nGGNoUrqSt93olXYXIWq47qJUYu5r-c3e-hqP-0Lbp_WPzUmKKH3Cn9bnVXQVZD9Gfi1c8897i1bXOj76XI2KI3jsFqfZKuE_hdM2qAMzEYZC9pH_kV-KUY3J-Zl3P9a4Ul80ox0G8itQ1FewGhVsxuItRPHikyyP0FhxNey0NLUnOOdiwOnKrqYRAHcaYspln7CCPXFaTWnZIbL_xT-fnn9zMFOTHu9iiH5lQ6KPD27A9r5SJq0EUVib7gk540YlYacz6r4r9FJdXqkSTZ96F2M7l-RCgymV74PXQv6JB-ezAvUWrI-vHmrBDgrs-IPY8IzDVgQKcyI-OI0UOXbzfH1lZaVW33iMqRxmKIZ60TuWKwcpC3nwKi8TVIDt5oA2iRkMXQluOfWvSz2aEMU65a8J0PzM-QxaMiFIxAQQFopYcY9VLceNCjCHWQjzy1BB3kDCOn2irh3CLGjI4VlB48e0PH7Ytl54JPFyCGwUm6kElY6ouyYL7drjiWU2Yp7WSn899B5zEEXSkto8ZMLood-XV5k

namespace RoSchmiViessmannApiTest
{
    internal  sealed partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            AccessToken = "eyJraWQiOiI0ZWNhM2VkNi0yNDY2LTQ2YWUtYTRhNC02MjZiNGEwM2RhMGUiLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI1MGIxMGM5ZS00YzhlLTQ1ODEtOWFiZC03MjhjZGI2YzRiZGQiLCJjaC5hZG5vdnVtLm5ldmlzaWRtLnByb2ZpbGVJZCI6IjE0MTc1MSIsIm5iZiI6MTcyMTU1MDI1OSwic2NvcGUiOiJJb1QgVXNlciBvcGVuaWQiLCJpc3MiOiJodHRwczovL2lhbS52aWVzc21hbm4uY29tIiwiZXhwIjoxNzIxNTUzODU5LCJpYXQiOjE3MjE1NTAyNTksImNsaWVudF9pZCI6ImExYjU5NTQxMDYwODVhYzMwZjk1ZTFiMmJjYzkwZmZmIn0.fREB9SBLHrc01jSFcu1aR0TyGHVyGIqdfzT9Dc_14mUwvO1UVbaJjgFrWqsmeV6Dw9QtlkyX74OYf4V1EAY1G2nzUNJNmwR1Q7uCJJclN1B0lOvZu73SJbof1CA1AgmIkmUBZlrLJVdzbrVA-mezw91HdFJCu0wpGVrp8zu8ORWjsJJ7MeKUy7DI-j6NokhGAOR80RpKRMIaeyBozTS0D41x-Ud74mqDBA9uGIpYnC58i--543TSN9uX6x7QAM6n7p2EH5ut-g6o2NV_a_Nctqlw8FWaOBJig3XN9ku50ucNCusNO_mt0Em0mHUR0r81ITjmV5k-oehVFRIcYo_O3Qb0mRmpaCqmGU7OItPcuMiTzFFAAcWxD5DgRuZU4RZX22PFzT3gxKHYtu1Y1EnKVicnu6uKDue7-agfHNXPg6AJz-lRjGziGs4Y231BhPWyssUVZ_STvPpaYXkcHZxtqejh3R2KEvupYiE_O7skAtE1VoAe3YrirhAxuNhPdt3K3LTt9Jn1xkFndRmTakyORjngwlZS5v-qhUNDm1y7hpbZt4i0HqjUM0dHVfovECak57pWML3t_kPlEG2BJzQuM8lMGRKAez9pLzhxkyRde1XXkP1KX_Yos7WjyvVyXnC1KGt8135VvMKv9kW3K2fduw8jx1mNa0WucNx6QAT3rbA";
            Get_Authorization_Clicked_Command = new AsyncRelayCommand(GetAuthorizationResponse);
            Get_Access_Token_Clicked_Command = new AsyncRelayCommand(GetTokenResponse);        
            Copy_Url_to_Clipboard_Clicked_Command = new RelayCommand(CopyUrlToClipboard);
            Copy_Access_Token_to_Clipboard_Clicked_Command = new RelayCommand(CopyAccessTokenToClipboard);
            Copy_Refresh_Token_to_Clipboard_Clicked_Command = new RelayCommand(CopyRefreshTokenToClipboard);
            Create_New_Codeverifier_Clicked_Command = new RelayCommand(CreateNewCodeVerifier);
            Get_Identity_Clicked_Command = new AsyncRelayCommand(GetIdentityResponse);
            Get_Equipment_Clicked_Command = new AsyncRelayCommand(GetEquipmentResponse);
            Get_Features_Clicked_Command = new AsyncRelayCommand(GetFeaturesResponse);
            Refresh_Access_Token_Clicked_Command = new AsyncRelayCommand(RefreshAccessToken);
        }

        public IAsyncRelayCommand Get_Authorization_Clicked_Command { get; }
        public IAsyncRelayCommand Get_Access_Token_Clicked_Command { get; }
        public IAsyncRelayCommand Get_Identity_Clicked_Command { get; }
        public IAsyncRelayCommand Get_Equipment_Clicked_Command { get; }

        public IAsyncRelayCommand Get_Features_Clicked_Command { get; }

        public IAsyncRelayCommand Refresh_Access_Token_Clicked_Command { get; }


        public ICommand Copy_Url_to_Clipboard_Clicked_Command { set; get; }
        public ICommand Copy_Access_Token_to_Clipboard_Clicked_Command { set; get; }
        public ICommand Copy_Refresh_Token_to_Clipboard_Clicked_Command { set; get; }
        public ICommand Create_New_Codeverifier_Clicked_Command { set; get; }


        private void CreateNewCodeVerifier() => Code_verifier = randomCodeVerifier(45);
        private void CopyUrlToClipboard() => Clipboard.SetText(RequestUrl);
        private void CopyAccessTokenToClipboard() => Clipboard.SetText(AccessToken);

        private void CopyRefreshTokenToClipboard() => Clipboard.SetText(RefreshToken);

        #region Task<string?> GetAuthorizationResponse()
        private async Task<string?> GetAuthorizationResponse()
        {
           
            AuthenticationCode = "";
            
            byte[] hashBytes;
            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashBytes = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(Code_verifier));

            }
            base64Url_Code_verifier = Base64UrlEncoder.Encode(hashBytes);
            code_challenge = base64Url_Code_verifier;

            // Create Query-String with parameters
            string queryString = $"client_id={Uri.EscapeDataString(Client_id)}" +
                $"&redirect_uri={Uri.EscapeDataString(redirect_uri)}" +
                $"&scope={Uri.EscapeDataString(AddRefreshToken ? scope_IoT_User_offline_access : scope_IoT_User)}" +
                $"&response_type={Uri.EscapeDataString(response_type)}" +
                $"&code_challenge_method={Uri.EscapeDataString(code_challenge_method)}" +
                $"&code_challenge={code_challenge}";

            System.Net.Http.HttpResponseMessage? response;
            string? content = null; ;

            using (var client = new HttpClient())
            {
                RequestUrl = $"{authorizeBaseUri}?{queryString}";
                response = await client.GetAsync(RequestUrl);
                content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }

        #endregion

        #region Task<string?> GetTokenResponse()
        private async Task<string?> GetTokenResponse()
        {
            // Create Content-String for POST request
            string sendContentString = $"client_id={Uri.EscapeDataString(Client_id)}" +
                $"&redirect_uri={redirect_uri}" +
                $"&grant_type={Uri.EscapeDataString(grant_type_authorization)}" +
                $"&code_verifier={Uri.EscapeDataString(Code_verifier)}" +
                $"&code={Uri.EscapeDataString(AuthenticationCode)}";

           // string encodedUrl = $"{tokenBaseUri}";


            HttpResponseMessage? responseMessage = null;
            string? responseContent = null;

            ViApiToken? viApiToken;

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(sendContentString, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        Headers = { { "Host", "iam.viessmann.com"
                            } },
                        RequestUri = new Uri(tokenBaseUri),
                        Content = content
                    };

                    responseMessage = await client.SendAsync(requestMessage);
                    responseContent = await responseMessage.Content.ReadAsStringAsync();

                    viApiToken = JsonSerializer.Deserialize<ViApiToken>(responseContent);

                    AccessToken = viApiToken != null ? viApiToken.access_token : string.Empty;

                    RefreshToken = viApiToken != null ? viApiToken.refresh_token != null ? viApiToken.refresh_token : string.Empty : string.Empty;
                }
                catch (Exception e)
                {
                    string? theExceptionMessage = e.Message;
                };
            }
            return responseContent;
        }
        #endregion

        #region Task<string?> GetIdentityResponse()
        private async Task<string?> GetIdentityResponse()
        {
            // Create Query-String with parameters
            string queryString = $"sections=identity"; 

            string encodedUrl = $"{userBaseUri}?{queryString}";
            HttpResponseMessage? responseMessage = null;
            string? responseContent = null;

            UserIdentity? userIdentity; // = new Installations();

            using (var client = new HttpClient())
            {
                string authorizationHeader = $"Bearer {AccessToken}";
                try
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Headers = { { "Authorization", authorizationHeader
                            } },
                        RequestUri = new Uri(encodedUrl),            
                    };

                    responseMessage = await client.SendAsync(requestMessage);
                    responseContent = await responseMessage.Content.ReadAsStringAsync();

                    userIdentity = JsonSerializer.Deserialize<UserIdentity>(responseContent);

                    Identity = userIdentity != null ?  $"{userIdentity.name.firstName} {userIdentity.name.familyName}" : "";
                    
                    
                    int dummy0 = 1;
                }
                catch (Exception e)
                {
                    string? theExceptionMessage = e.Message;
                };
            }
            return responseContent;
        }

        #endregion

        #region Task<string?> GetEquipmentResponse()
        private async Task<string?> GetEquipmentResponse()
        {

            // Create Query-String with parameters
            string queryString = $"includeGateways=true";
            
            string addendum = $"equipment/installations";

            string completeUrl = $"{iotBaseUri}{addendum}?{queryString}";
            HttpResponseMessage? responseMessage = null;
            string? responseContent = null;

            Installations? installations = new Installations();

            using (var client = new HttpClient())
            {
                string authorizationHeader = $"Bearer {AccessToken}";
                try
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Headers = { { "Authorization", authorizationHeader
                            } },
                        RequestUri = new Uri(completeUrl),
                    };

                    responseMessage = await client.SendAsync(requestMessage);
                    responseContent = await responseMessage.Content.ReadAsStringAsync();


                    installations = JsonSerializer.Deserialize<Installations>(responseContent);


                    string? cursorNext = installations == null ? null : installations.cursor.next;

                    int? theId = installations.data[0].id;

                    InstallationId = theId == null ? "" : theId.ToString();

                    GatewaySerial = installations.data[0].gateways[0].serial;

                    DeviceId = installations.data[0].gateways[0].devices[0].id.ToString();

                    string? theDescription = installations.data[0].description;

                    string? addressCity = installations.data[0].address.city;

                    string? addressStreet = installations.data[0].address.street;

                }
                catch (Exception e)
                {
                    string? theExceptionMessage = e.Message;
                };
            }

            return responseContent;
        }
        #endregion

        #region Task<string?> GetFeaturestResponse()
        private async Task<string?> GetFeaturesResponse()
        {
            if (string.IsNullOrEmpty(InstallationId))
            {
                await GetEquipmentResponse();
            }

            string addendum = $"features/installations/{InstallationId}/gateways/{GatewaySerial}/devices/{DeviceId}/features";
            string completeUrl = $"{iotBaseUri}{addendum}";
            HttpResponseMessage? responseMessage = null;
            string? responseContent = null;

            //Features? features = new Features();

            Features? features = null;

            using (var client = new HttpClient())
            {
                string authorizationHeader = $"Bearer {AccessToken}";
                try
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Headers = { { "Authorization", authorizationHeader
                            } },
                        RequestUri = new Uri(completeUrl),
                    };

                    responseMessage = await client.SendAsync(requestMessage);
                    responseContent = await responseMessage.Content.ReadAsStringAsync();

                    features = JsonSerializer.Deserialize<Features>(responseContent);

                    DateTime localDateTime = DateTime.MinValue;

                    try
                    {
                        if ((features != null) && (features.data != null))
                        {
                            try
                            {
                                localDateTime = ((DateTime)features.data[3].timestamp).ToLocalTime();
                                TimeTempMain = $"{localDateTime.Hour.ToString("00")}:{localDateTime.Minute.ToString("00")}.{localDateTime.Second.ToString("00")}";
                                TemperatureMain = features.data[3].properties.value.value.ToString();
                            }
                            catch (Exception ex)
                            {
                            }

                            try
                            {
                                localDateTime = ((DateTime)features.data[95].timestamp).ToLocalTime();
                                TimeTempOutside = $"{localDateTime.Hour.ToString("00")}:{localDateTime.Minute.ToString("00")}.{localDateTime.Second.ToString("00")}";
                                TemperatureOutside = features.data[95].properties.value.value.ToString();
                            }
                            catch(Exception ex)
                            {

                            }

                            try
                            {
                                localDateTime = ((DateTime)features.data[91].timestamp).ToLocalTime();
                                TimeTempOutlet = $"{localDateTime.Hour.ToString("00")}:{localDateTime.Minute.ToString("00")}.{localDateTime.Second.ToString("00")}";
                                TemperatureOutlet = features.data[91].properties.value.value.ToString();
                            }
                            catch(Exception ex)
                            {

                            }

                            try
                            {
                                localDateTime = ((DateTime)features.data[77].timestamp).ToLocalTime();
                                TimeTempSupply = $"{localDateTime.Hour.ToString("00")}:{localDateTime.Minute.ToString("00")}.{localDateTime.Second.ToString("00")}";
                                TemperatureSupply = features.data[77].properties.value.value.ToString();
                            }
                            catch(Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception e1)
                    {
                        string? theExceptionMessage = e1.Message;
                    }
                }
                catch (Exception e)
                {
                    string? theExceptionMessage = e.Message;
                };
            }

            return responseContent;
        }
        #endregion


        #region Task<string?> GetRefreshAccessToken()
        private async Task<string?> RefreshAccessToken()
        {
            // Create Content-String for POST request
            string sendContentString = $"grant_type={Uri.EscapeDataString(grant_type_refresh)}" +
                $"&client_id={Uri.EscapeDataString(Client_id)}" +
                $"&refresh_token={Uri.EscapeDataString(refreshToken)}";


            ViApiToken? viApiToken;
            HttpResponseMessage? responseMessage = null;
            string? responseContent = null;
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(sendContentString, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        Headers = { { "Host", "iam.viessmann.com"
                            } },
                        RequestUri = new Uri(tokenBaseUri),
                        Content = content
                    };

                    responseMessage = await client.SendAsync(requestMessage);
                    responseContent = await responseMessage.Content.ReadAsStringAsync();

                    viApiToken = JsonSerializer.Deserialize<ViApiToken>(responseContent);

                    AccessToken = viApiToken != null ? viApiToken.access_token : string.Empty;

                    RefreshToken = viApiToken != null ? viApiToken.refresh_token != null ? viApiToken.refresh_token : string.Empty : string.Empty;         
                }
                catch (Exception e)
                {
                    string? theExceptionMessage = e.Message;
                };
            }
            return responseContent;
        }
        #endregion


        private static string randomCodeVerifier(int length)
        {
            // This is a quick and dirty function, not certain that it will always work properly

            // Restrict to length between 43 and 100
            length = length < 43 ? 43 : length >100 ? 100 : length ;
            // Create random Byte-Array
            byte[] randomBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }          
            return Base64UrlEncoder.Encode(randomBytes);
        }


        [ObservableProperty]
        private string? requestUrl;

        [ObservableProperty]
        private string? authenticationCode;

        [ObservableProperty]
        private string? accessToken;

        [ObservableProperty]
        private string? refreshToken;

        [ObservableProperty]
        private string client_id = "a1b5954106085ac30f95e1b2bcc90fff";

        [ObservableProperty]
        private string code_verifier = randomCodeVerifier(43); // between 43 and 128 Char

        [ObservableProperty]
        private bool addRefreshToken = false;

        [ObservableProperty]
        private string identity = "";

        [ObservableProperty]
        private string installationId = "";

        [ObservableProperty]
        private string gatewaySerial = "";

        [ObservableProperty]
        private string deviceId = "";

        [ObservableProperty]
        private string timeTempMain = "";

        [ObservableProperty]
        private string temperatureMain = "";

        [ObservableProperty]
        private string timeTempOutside = "";

        [ObservableProperty]
        private string temperatureOutside = "";

        [ObservableProperty]
        private string timeTempOutlet = "";

        [ObservableProperty]
        private string temperatureOutlet = "";

        [ObservableProperty]
        private string timeTempSupply = "";

        [ObservableProperty]
        private string temperatureSupply = "";

        private const string authorizeBaseUri = "https://iam.viessmann.com/idp/v3/authorize";
        private const string tokenBaseUri = "https://iam.viessmann.com/idp/v3/token";

        private const string userBaseUri = "https://api.viessmann.com/users/v1/users/me";

        private const string iotBaseUri = "https://api.viessmann.com/iot/v1/";

        private const string redirect_uri = "http://localhost:4200/";
        private const string scope_IoT_User = "IoT User";
        private const string scope_IoT_User_offline_access = "IoT User offline_access";
        private const string response_type = "code";
        private const string code_challenge_method = "S256";
        private const string grant_type_authorization = "authorization_code";
        private const string grant_type_refresh = "refresh_token";

        private string? requestResult;
        private string code_challenge = "";

        private string? sha256_hashed_codeverifier;
        private string? base64Url_Code_verifier;
    }
}
