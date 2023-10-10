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
        private bool _showAuth = false;
        private string _previewURL = string.Empty;

        private SerializedProperty _customHeaders;
        private SerializedProperty _customParameters;
        private SerializedProperty _rateLimitHeaders;
        private SerializedProperty _versionHeaders;
        private GUIStyle _headerLabels;
        private GUIContent _paramTypeContent;
        private GUIContent _mediaTypeContent;

        private void OnEnable()
        {
            _customHeaders = serializedObject.FindProperty("CustomHeaders");
            _customParameters = serializedObject.FindProperty("CustomParameters");
            _rateLimitHeaders = serializedObject.FindProperty("RateLimitHeaders");
            _versionHeaders = serializedObject.FindProperty("VersionHeaders");

            _headerLabels = new GUIStyle();
            _headerLabels.fontStyle = FontStyle.Bold;
            _headerLabels.alignment = TextAnchor.MiddleCenter;
            _headerLabels.fontSize = 14;
            _headerLabels.normal.textColor = Color.white;

            _paramTypeContent = new GUIContent("Parameter Style", "This will determine the type of parameters based on the RESTful api structure" +
            "\n\nQuery: The query parameters are appended to the URL after the '?' character." +
            "\n\nDirectPath: Include parameters directly in the URL path itself" +
            "\n\nRequestBody: Parameters are sent in the request body as JSON, form data, or XML instead of being included in the URL.");

            _mediaTypeContent = new GUIContent("Media Type (Optional)", "Provides support for the media type and quality in a Content-Type header, i.e 'application/vnd.api+json'");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RestAPISettings settings = (RestAPISettings)target;

            EditorGUILayout.LabelField("API Settings", _headerLabels);

            if (string.IsNullOrEmpty(settings.BaseURL))
                EditorGUILayout.HelpBox("Base URL must be filled out!", MessageType.Warning, true);

            settings.BaseURL = EditorGUILayout.TextField("Base URL", settings.BaseURL);

            settings.AuthenticationType = (AuthenticationType)EditorGUILayout.EnumPopup("Authentication Type", settings.AuthenticationType);
            DisplayAuthentication(settings);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("HTTP Settings", _headerLabels);

            settings.ParamType = (ParamType)EditorGUILayout.EnumPopup(_paramTypeContent, settings.ParamType);
            EditorGUILayout.PropertyField(_customHeaders, true); // true makes it display child properties
            EditorGUILayout.PropertyField(_customParameters, true); // true makes it display child properties
            settings.ContentType = EditorGUILayout.TextField(_mediaTypeContent, settings.ContentType);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Manager Settings", _headerLabels);
            settings.TimeoutDuration = EditorGUILayout.DelayedIntField("Timeout Duration", settings.TimeoutDuration);
            settings.MaxRetries = EditorGUILayout.DelayedIntField("Max Retries", settings.MaxRetries);
            settings.RetryDelaySeconds = EditorGUILayout.DelayedIntField("Retry Delay (s)", settings.RetryDelaySeconds);
            settings.PrintAPIRequest = EditorGUILayout.Toggle("Print API Request", settings.PrintAPIRequest);
            settings.PrintAPIResponse = EditorGUILayout.Toggle("Print API Response", settings.PrintAPIResponse);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Sorting and Filtering", _headerLabels);
            settings.SortField = EditorGUILayout.TextField("Sort Field", settings.SortField);
            settings.SortAscending = EditorGUILayout.Toggle("Sort Ascending", settings.SortAscending);
            settings.FilterField = EditorGUILayout.TextField("Filter Field", settings.FilterField);
            settings.FilterValue = EditorGUILayout.TextField("Filter Value", settings.FilterValue);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Pagination", _headerLabels);
            settings.PageSize = EditorGUILayout.DelayedIntField("Page Size", settings.PageSize);
            settings.PageNumber = EditorGUILayout.DelayedIntField("Page Number", settings.PageNumber);
            settings.Cursor = EditorGUILayout.TextField("Cursor", settings.Cursor);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Custom Endpoints", _headerLabels);
            settings.CustomEndpointPath = EditorGUILayout.TextField("Custom Endpoint Path", settings.CustomEndpointPath);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Headers for Specific Features", _headerLabels);
            EditorGUILayout.PropertyField(_rateLimitHeaders, true); // true makes it display child properties
            EditorGUILayout.PropertyField(_versionHeaders, true); // true makes it display child properties

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Request and Response Models", _headerLabels);
            settings.RequestModel = EditorGUILayout.TextField("Request Model", settings.RequestModel);
            settings.ResponseModel = EditorGUILayout.TextField("Response Model", settings.ResponseModel);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Caching Settings", _headerLabels);
            settings.EnableCaching = EditorGUILayout.Toggle("Enable Caching", settings.EnableCaching);
            settings.CacheDurationSeconds = EditorGUILayout.DelayedIntField("Cache Duration (s)", settings.CacheDurationSeconds);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Webhooks", _headerLabels);
            settings.WebhookURL = EditorGUILayout.TextField("Webhook URL", settings.WebhookURL);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Error Handling", _headerLabels);
            settings.ErrorHandling = (RestAPISettings.ErrorHandlingStrategy)EditorGUILayout.EnumPopup("Error Handling", settings.ErrorHandling);

            EditorGUILayout.TextArea("Preview URL: ", _previewURL);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayAuthentication(RestAPISettings settings)
        {
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
    }
}