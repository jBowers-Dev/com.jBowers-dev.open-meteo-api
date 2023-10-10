using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JoshBowersDev.RestAPI.Runtime
{
    public static class APIManager<T>
    {
        // Fetch the default
        private static RestAPISettings _settings = (RestAPISettings)Resources.Load("Resources/RESTfulAPISettings", typeof(RestAPISettings));

        public static async Task<T> RequestGetAPI(CancellationTokenSource cts, RestAPISettings settings = null)
        {
            T result = default;

            using (HttpClient client = new HttpClient())
            {
                // Set Request headers
                client.DefaultRequestHeaders.Accept.Clear();
                if (!string.IsNullOrEmpty(settings.ContentType))
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(settings.ContentType));
                client.DefaultRequestHeaders.Authorization = GetHeaderValue(settings.GetAuthorizationData());

                // Add Custom Headers
                if (settings.CustomHeaders != null)
                {
                    foreach (var header in settings.CustomHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Item1, header.Item2);
                    }
                }

                // Build the URL based on ParamType
                string requestUrl = settings.BaseURL;

                switch (settings.ParamType)
                {
                    case ParamType.Query:
                        if (settings.CustomParameters != null && settings.CustomParameters.Count > 0)
                        {
                            string queryParams = string.Join("&", settings.CustomParameters);
                            requestUrl += $"?{queryParams}";
                        }
                        break;

                    case ParamType.DirectPath:
                        if (!string.IsNullOrEmpty(settings.CustomEndpointPath))
                        {
                            requestUrl += settings.CustomEndpointPath;
                        }
                        break;

                    case ParamType.RequestBody:
                        // Add logic for RequestBody if needed
                        break;

                    default:
                        break;
                }

                // Send off the HTTP Get request
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(requestUrl, cts.Token);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Parse and process the response as needed
                    // Example: result = JsonConvert.DeserializeObject<T>(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Debug.LogError($"HTTP Request Error: {e.Message}");
                }
                catch (TaskCanceledException e)
                {
                    Debug.LogWarning($"Request was canceled: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"An error occurred: {e.Message}");
                }
            }

            return result;
        }

        #region Private Methods

        // Get Authorization header based on AuthenticationType
        private static AuthenticationHeaderValue GetHeaderValue(AuthorizationData auth)
        {
            switch (auth.AuthenticationType)
            {
                case AuthenticationType.APIKey:
                    if (auth.Data != null)
                    {
                        return new AuthenticationHeaderValue("Authorization", $"Api-Key {(string)auth.Data}");
                    }
                    break;

                case AuthenticationType.BearerToken:
                    if (auth.Data != null)
                    {
                        return new AuthenticationHeaderValue("Bearer", (string)auth.Data);
                    }
                    break;

                case AuthenticationType.Basic:
                    string[] data = (string[])auth.Data;
                    try
                    {
                        if (!string.IsNullOrEmpty(data[0]) && !string.IsNullOrEmpty(data[1]))
                        {
                            string base64Credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{data[0]}:{data[1]}"));
                            return new AuthenticationHeaderValue("Basic", base64Credentials);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        return null;
                    }

                    break;

                case AuthenticationType.OAuth:
                    if (auth.Data != null)
                    {
                        return new AuthenticationHeaderValue("Bearer", (string)auth.Data);
                    }
                    break;

                default:
                    break;
            }
            Debug.LogError("Invalid authentication data.");
            return null;
        }

        #endregion Private Methods
    }
}