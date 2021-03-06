// Copyright 2021 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using ArcGISRuntime.Helpers;
using Esri.ArcGISRuntime.Http;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using AuthenticationManager = Esri.ArcGISRuntime.Security.AuthenticationManager;

namespace ArcGISRuntime.UWP.Samples.OAuth
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        name: "Authenticate with OAuth",
        category: "Security",
        description: "Authenticate with ArcGIS Online (or your own portal) using OAuth2 to access secured resources (such as private web maps or layers).",
        instructions: "When you run the sample, the app will load a web map which contains premium content. You will be challenged for an ArcGIS Online login to view the private layers. Enter a user name and password for an ArcGIS Online named user account (such as your ArcGIS for Developers account). If you authenticate successfully, the traffic layer will display, otherwise the map will contain only the public basemap layer.",
        tags: new[] { "OAuth", "OAuth2", "authentication", "cloud", "credential", "portal", "security" })]
    [ArcGISRuntime.Samples.Shared.Attributes.ClassFile("Helpers\\ArcGISLoginPrompt.cs")]
    public partial class OAuth
    {
        // Constants for OAuth-related values.
        // - The URL of the portal to authenticate with
        private const string ServerUrlSharing = "https://ua-gas-gisportal.southernco.com/portal/sharing/rest";
        private const string ServerUrlHome = "https://ua-gas-gisportal.southernco.com/portal/home/";
        private const string AuthorizeURL = "https://ua-gas-gisportal.southernco.com/portal/sharing/oauth2/authorize";
        private const string TokenURL = "https://ua-gas-gisportal.southernco.com/portal/sharing/oauth2/token";
        private const string AppClientId = "oHvyHoTBFYyzwTXV";
        private const string OAuthRedirectUrl = "my-ags-app://auth";
        private Credential credential;

        // - The ID for a web map item hosted on the server
        private const string WebMapId = "e5039444ef3c48b8a8fdc9227f9be7c1";

        public OAuth()
        {
            InitializeComponent();

            // Call a function to initialize the app.
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                //Force the Portal to Login
                // Set up the AuthenticationManager to use OAuth for secure ArcGIS Online requests.
                // Define the server information for ArcGIS Online
                ArcGISLoginPrompt.SetChallengeHandler();

                // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(ArcGISLoginPrompt.PromptCredentialAsync);

                //Create Handler to monitor response end flags 
                ArcGISHttpClientHandler.HttpResponseEnd += ArcGISHttpClientHandler_HttpResponseEnd;
               
                //Force a login to the Portal
                ArcGISPortal arcgisPortal = await ArcGISPortal.CreateAsync(new Uri(ServerUrlHome), true);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Do the Web View Login");
                webView1.Navigate(new Uri(AuthorizeURL));
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }

        private void ArcGISHttpClientHandler_HttpRequestBegin(object s, System.Net.Http.HttpRequestMessage r)
        {
            if (r.RequestUri.Host == "ua-gas-gisportal.southernco.com")
            {
                HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
                var cookieManager = myFilter.CookieManager;
                HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://ua-gas-gisportal.southernco.com"));

                HttpClientHandler httpClientHandler = ((ArcGISHttpRequestMessage)r).Handler as HttpClientHandler;

                foreach (HttpCookie cook in myCookieJar)
                {
                    Debug.WriteLine(cook.Name);
                    Debug.WriteLine(cook.Value);
                    Cookie cookie = new Cookie();
                    cookie.Name = cook.Name;
                    cookie.Value = cook.Value;
                    cookie.Domain = cook.Domain;

                    httpClientHandler.CookieContainer.Add(cookie);
                }
            }
        }

        private void ArcGISHttpClientHandler_HttpResponseEnd(object sender, HttpResponseEndEventArgs e)
        {
            if (e.Response.Headers.Server.ToString().Contains("BigIP"))
            {
                throw new UnauthorizedAccessException("Redirect to the Web View");
            }
        }


        private async void webView1_NavigationCompleted_1(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            if (e.Uri.AbsoluteUri.ToString() == ServerUrlHome)
            {
                ArcGISHttpClientHandler.HttpResponseEnd -= ArcGISHttpClientHandler_HttpResponseEnd;
                ArcGISHttpClientHandler.HttpRequestBegin += ArcGISHttpClientHandler_HttpRequestBegin;
            } else
                Console.WriteLine(e.Uri.AbsoluteUri.ToString());
        }
    }


}