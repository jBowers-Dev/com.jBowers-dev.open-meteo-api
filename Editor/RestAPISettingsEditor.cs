using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JoshBowersDev.RestAPI.Editor
{
    [CustomEditor(typeof(RestAPISettings))]
    public class RestAPISettingsEditor : UnityEditor.Editor
    {
        private bool _showAPIHeader = true;
        private bool _showHTTPHeader = true;
        private bool _showManagerHeader = true;
        private bool _showSortingHeader = true;
        private bool _showPageHeader = true;
        private bool _showCustomEndPointHeader = true;
        private bool _showSpecificFeaturesHeader = true;
        private bool _showRequestResponseHeader = true;
        private bool _showCachingSettingsHeader = true;
        private bool _showWebhooksHeader = true;
        private bool _showErrorHandlingHeader = true;

        private bool _showAuth = false;
        private string _previewURL = string.Empty;

        private SerializedProperty _customHeaders;
        private SerializedProperty _customParameters;
        private SerializedProperty _rateLimitHeaders;
        private SerializedProperty _versionHeaders;
        private GUIStyle _headerStyle;
        private GUIStyle _backgroundStyle;

        // Header GUIContent
        private GUIContent _apiHeader;

        private GUIContent _httpHeader;
        private GUIContent _managerHeader;
        private GUIContent _sortingHeader;
        private GUIContent _pageHeader;
        private GUIContent _customEndPointHeader;
        private GUIContent _specificFeaturesHeader;
        private GUIContent _requestResponseHeader;
        private GUIContent _cachingSettingsHeader;
        private GUIContent _webhooksHeader;
        private GUIContent _errorHandlingHeader;

        // Other GUIContent
        private GUIContent _paramTypeContent;

        private GUIContent _mediaTypeContent;

        private Texture2D _headerTex;
        private Texture2D _backgroundTex;
        private Texture2D _separatorTex;

        private void OnEnable()
        {
            _customHeaders = serializedObject.FindProperty("CustomHeaders");
            _customParameters = serializedObject.FindProperty("CustomParameters");
            _rateLimitHeaders = serializedObject.FindProperty("RateLimitHeaders");
            _versionHeaders = serializedObject.FindProperty("VersionHeaders");

            _headerTex = new Texture2D(1, 1);
            _headerTex.SetPixel(0, 0, new Color(.3f, .3f, .8f, .4f));
            _headerTex.Apply();

            _separatorTex = new Texture2D(1, 1);
            _separatorTex.SetPixel(0, 0, Color.black);
            _separatorTex.Apply();

            _backgroundTex = new Texture2D(1, 1);
            _backgroundTex.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            _backgroundTex.Apply();

            CreateHeaderStyle();
            CreateGUIContent();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RestAPISettings settings = (RestAPISettings)target;

            // Inside your OnInspectorGUI method
            _showAPIHeader = EditorGUILayout.Foldout(_showAPIHeader, _apiHeader, true, _headerStyle);
            if (_showAPIHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                if (string.IsNullOrEmpty(settings.BaseURL))
                    EditorGUILayout.HelpBox("Base URL must be filled out!", MessageType.Warning, true);

                settings.BaseURL = EditorGUILayout.TextField("Base URL", settings.BaseURL);

                settings.AuthenticationType = (AuthenticationType)EditorGUILayout.EnumPopup("Authentication Type", settings.AuthenticationType);
                DisplayAuthentication(settings);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showHTTPHeader = EditorGUILayout.Foldout(_showHTTPHeader, _httpHeader, true, _headerStyle);
            if (_showHTTPHeader)
            {
                EditorGUILayout.Space(3);
                Rect r = EditorGUILayout.BeginVertical();
                GUI.DrawTexture(r, _backgroundTex);
                settings.ParamType = (ParamType)EditorGUILayout.EnumPopup(_paramTypeContent, settings.ParamType);
                EditorGUILayout.PropertyField(_customHeaders, true); // true makes it display child properties
                EditorGUILayout.PropertyField(_customParameters, true); // true makes it display child properties
                settings.ContentTypes = (HttpEnums.ContentType)EditorGUILayout.EnumFlagsField(_mediaTypeContent, settings.ContentTypes);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showManagerHeader = EditorGUILayout.Foldout(_showManagerHeader, _managerHeader, true, _headerStyle);
            if (_showManagerHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.TimeoutDuration = EditorGUILayout.DelayedIntField("Timeout Duration", settings.TimeoutDuration);
                settings.MaxRetries = EditorGUILayout.DelayedIntField("Max Retries", settings.MaxRetries);
                settings.RetryDelaySeconds = EditorGUILayout.DelayedIntField("Retry Delay (s)", settings.RetryDelaySeconds);
                settings.PrintAPIRequest = EditorGUILayout.Toggle("Print API Request", settings.PrintAPIRequest);
                settings.PrintAPIResponse = EditorGUILayout.Toggle("Print API Response", settings.PrintAPIResponse);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showSortingHeader = EditorGUILayout.Foldout(_showSortingHeader, _sortingHeader, true, _headerStyle);
            if (_showSortingHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.SortField = EditorGUILayout.TextField("Sort Field", settings.SortField);
                settings.SortAscending = EditorGUILayout.Toggle("Sort Ascending", settings.SortAscending);
                settings.FilterField = EditorGUILayout.TextField("Filter Field", settings.FilterField);
                settings.FilterValue = EditorGUILayout.TextField("Filter Value", settings.FilterValue);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showPageHeader = EditorGUILayout.Foldout(_showPageHeader, _pageHeader, true, _headerStyle);
            if (_showPageHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.PageSize = EditorGUILayout.DelayedIntField("Page Size", settings.PageSize);
                settings.PageNumber = EditorGUILayout.DelayedIntField("Page Number", settings.PageNumber);
                settings.Cursor = EditorGUILayout.TextField("Cursor", settings.Cursor);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showCustomEndPointHeader = EditorGUILayout.Foldout(_showCustomEndPointHeader, _customEndPointHeader, true, _headerStyle); if (_showCustomEndPointHeader)
            {
                EditorGUILayout.Space(3);
                Rect r = EditorGUILayout.BeginVertical();
                GUI.DrawTexture(r, _backgroundTex);
                settings.CustomEndpointPath = EditorGUILayout.TextField("Custom Endpoint Path", settings.CustomEndpointPath);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showSpecificFeaturesHeader = EditorGUILayout.Foldout(_showSpecificFeaturesHeader, _specificFeaturesHeader, true, _headerStyle);
            if (_showSpecificFeaturesHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                EditorGUILayout.PropertyField(_rateLimitHeaders, true); // true makes it display child properties
                EditorGUILayout.PropertyField(_versionHeaders, true); // true makes it display child properties
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showRequestResponseHeader = EditorGUILayout.Foldout(_showRequestResponseHeader, _requestResponseHeader, true, _headerStyle);
            if (_showRequestResponseHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.RequestModel = EditorGUILayout.TextField("Request Model", settings.RequestModel);
                settings.ResponseModel = EditorGUILayout.TextField("Response Model", settings.ResponseModel);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showCachingSettingsHeader = EditorGUILayout.Foldout(_showCachingSettingsHeader, _cachingSettingsHeader, true, _headerStyle);
            if (_showCachingSettingsHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.EnableCaching = EditorGUILayout.Toggle("Enable Caching", settings.EnableCaching);
                settings.CacheDurationSeconds = EditorGUILayout.DelayedIntField("Cache Duration (s)", settings.CacheDurationSeconds);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showWebhooksHeader = EditorGUILayout.Foldout(_showWebhooksHeader, _webhooksHeader, true, _headerStyle);
            if (_showWebhooksHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.WebhookURL = EditorGUILayout.TextField("Webhook URL", settings.WebhookURL);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            _showErrorHandlingHeader = EditorGUILayout.Foldout(_showErrorHandlingHeader, _errorHandlingHeader, true, _headerStyle);
            if (_showErrorHandlingHeader)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(_backgroundStyle);
                settings.ErrorHandling = (RestAPISettings.ErrorHandlingStrategy)EditorGUILayout.EnumPopup("Error Handling", settings.ErrorHandling);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(10);

            EditorGUILayout.TextArea("Preview URL: ", _previewURL);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayAuthentication(RestAPISettings settings)
        {
            if (settings.AuthenticationType == AuthenticationType.None) { return; }

            _showAuth = EditorGUILayout.Foldout(_showAuth, "Authentication", true);
            if (_showAuth)
            {
                switch (settings.AuthenticationType)
                {
                    case AuthenticationType.APIKey:
                        settings.APIKey = EditorGUILayout.TextField("API Key", settings.APIKey);
                        break;

                    case AuthenticationType.BearerToken:
                        settings.BearerToken = EditorGUILayout.TextField("Bearer Token", settings.BearerToken);
                        break;

                    case AuthenticationType.Basic:
                        settings.AuthenticationBasic.Username = EditorGUILayout.TextField("User ID", settings.AuthenticationBasic.Username);
                        settings.AuthenticationBasic.Password = EditorGUILayout.TextField("Password", settings.AuthenticationBasic.Password);
                        break;

                    case AuthenticationType.OAuth:
                        settings.OAuth.ClientId = EditorGUILayout.TextField("Client ID", settings.OAuth.ClientId);
                        settings.OAuth.ClientSecret = EditorGUILayout.TextField("Client Secret", settings.OAuth.ClientSecret);
                        settings.OAuth.AccessToken = EditorGUILayout.TextField("Access Token", settings.OAuth.AccessToken);
                        settings.OAuth.RefreshToken = EditorGUILayout.TextField("Refresh Token", settings.OAuth.RefreshToken);

                        break;

                    default:
                        break;
                }
            }
        }

        private void CreateHeaderStyle()
        {
            _headerStyle = new GUIStyle();
            _headerStyle.fontStyle = FontStyle.Bold;
            _headerStyle.alignment = TextAnchor.MiddleCenter;
            _headerStyle.fontSize = 18;
            _headerStyle.normal.textColor = Color.white;
            _headerStyle.normal.background = _headerTex;
            _headerStyle.margin.left = 14;
            //_headerStyle.border.right = -1000;
            _headerStyle.border.top = 20;
            _headerStyle.border.bottom = 20;

            _backgroundStyle = new GUIStyle();
            _backgroundStyle.normal.background = _backgroundTex;
        }

        private void CreateGUIContent()
        {
            // Create Headers
            _apiHeader = new GUIContent("API", _headerTex, "The base settings for the API call");
            _httpHeader = new GUIContent("HTTP Settings", "Settings related to HTTP requests and headers");
            _managerHeader = new GUIContent("Manager Settings", "Settings for the API manager");
            _sortingHeader = new GUIContent("Sorting and Filtering", "Settings for sorting and filtering API data");
            _pageHeader = new GUIContent("Pagination", "Settings for pagination of API data");
            _customEndPointHeader = new GUIContent("Custom Endpoints", "Settings for custom API endpoints");
            _specificFeaturesHeader = new GUIContent("Headers for Specific Features", "Headers for specific features in the API");
            _requestResponseHeader = new GUIContent("Request and Response Models", "Settings for API request and response models");
            _cachingSettingsHeader = new GUIContent("Caching Settings", "Settings related to data caching");
            _webhooksHeader = new GUIContent("Webhooks", "Settings for webhooks");
            _errorHandlingHeader = new GUIContent("Error Handling", "Settings for handling API errors");

            // Create other custom content
            _paramTypeContent = new GUIContent("Parameter Style", "This will determine the type of parameters based on the RESTful api structure" +
            "\n\nQuery: The query parameters are appended to the URL after the '?' character." +
            "\n\nDirectPath: Include parameters directly in the URL path itself" +
            "\n\nRequestBody: Parameters are sent in the request body as JSON, form data, or XML instead of being included in the URL.");

            _mediaTypeContent = new GUIContent("Media Type (Optional)", "Provides support for the media type and quality in a Content-Type header, i.e 'application/vnd.api+json'");
        }
    }
}