using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoshBowersDev.RestAPI
{
    #region Helper Enums and Structs

    public enum AuthenticationType
    {
        APIKey,
        BearerToken,
        Basic,
        OAuth,
        None
    }

    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum ParamType
    {
        Query,
        DirectPath,
        RequestBody
    }

    public struct AuthenticationBasic
    {
        [Header("User ID")]
        public string Username;

        [Header("Password")]
        public string Password;
    }

    public struct AuthenticationOAuth
    {
        public string ClientId;
        public string ClientSecret;
        public string AccessToken;
        public string RefreshToken;
    }

    public struct AuthorizationData
    {
        public AuthenticationType AuthenticationType;
        public object Data;
    }

    [System.Serializable]
    public struct HTTPField
    {
        public string Item1;
        public string Item2;
    }

    #endregion Helper Enums and Structs

    [CreateAssetMenu(fileName = "RESTfulAPISettings", menuName = "RESTFul API Settings")]
    public class RestAPISettings : ScriptableObject
    {
        public string BaseURL;
        public AuthenticationType AuthenticationType;
        public string APIKey;
        public string BearerToken;
        public AuthenticationBasic AuthenticationBasic;
        public AuthenticationOAuth OAuth;
        public List<HTTPField> CustomHeaders;
        public List<string> CustomParameters;
        public HTTPMethod HTTPMethod; // TBD
        public ParamType ParamType;
        public HttpEnums.ContentType ContentTypes;
        public int TimeoutDuration;
        public int MaxRetries;
        public int RetryDelaySeconds;
        public bool PrintAPIRequest;
        public bool PrintAPIResponse;

        // Sorting and Filtering Parameters
        public string SortField;

        public bool SortAscending;
        public string FilterField;
        public string FilterValue;

        // Pagination Parameters
        public int PageSize;

        public int PageNumber;
        public string Cursor;

        // Custom Endpoints
        public string CustomEndpointPath;

        // Headers for Specific Features
        public List<HTTPField> RateLimitHeaders;

        public List<HTTPField> VersionHeaders;

        // Request and Response Models (Class or Struct names)
        public string RequestModel;

        public string ResponseModel;

        // Caching Settings
        public bool EnableCaching;

        public int CacheDurationSeconds;

        // Webhooks
        public string WebhookURL;

        // Error Handling (Add more fields or enums for error handling strategy)
        public enum ErrorHandlingStrategy
        {
            LogErrors,
            RetryOnFailure,
            RaiseException
        }

        public ErrorHandlingStrategy ErrorHandling;

        /// <summary>
        /// Retrieves the current Authorization type and data.
        /// </summary>
        /// <returns></returns>
        public AuthorizationData GetAuthorizationData()
        {
            var result = new AuthorizationData();
            result.AuthenticationType = AuthenticationType;

            switch (AuthenticationType)
            {
                case AuthenticationType.APIKey:
                    result.Data = APIKey;
                    break;

                case AuthenticationType.BearerToken:
                    result.Data = BearerToken;
                    break;

                case AuthenticationType.Basic:
                    result.Data = AuthenticationBasic;
                    break;

                case AuthenticationType.OAuth:
                    result.Data = OAuth;
                    break;

                default:
                    break;
            }
            return result;
        }
    }
}