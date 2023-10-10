using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace JoshBowersDev.RestAPI.Runtime
{
    public static class RestAPI
    {
        #region Public Methods

        public static void GetRequest(string uri, RestAPICertificate cert, List<(string, string)> headers = null, List<(string, string)> parameters = null, Action<UnityWebRequest> callback = null)
        {
            if (parameters != null)
            {
                uri += "?";
                foreach (var parameter in parameters)
                {
                    uri += $"{parameter.Item1}={parameter.Item2}&";
                }
            }

            UnityWebRequest request = UnityWebRequest.Get(uri);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader($"{header.Item1}", $"{header.Item2}");
                }
            }
            request.certificateHandler = cert;
            var operation = request.SendWebRequest();
            operation.completed += (context) =>
            {
                callback?.Invoke(request);
                cert?.Dispose();
            };
        }

        public static void PostRequest(string uri, string data, RestAPICertificate cert, List<(string, string)> headers = null, List<(string, string)> parameters = null, Action<UnityWebRequest> callback = null)
        {
            if (parameters != null)
            {
                uri += "?";
                foreach (var parameter in parameters)
                {
                    uri += $"{parameter.Item1}={parameter.Item2}&";
                }
            }

            UnityWebRequest request = UnityWebRequest.Put(uri, data);
            request.method = UnityWebRequest.kHttpVerbPOST;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader($"{header.Item1}", $"{header.Item2}");
                }
            }
            request.certificateHandler = cert;
            var operation = request.SendWebRequest();
            operation.completed += (context) =>
            {
                callback?.Invoke(request);
                cert?.Dispose();
            };
        }

        public static void DeleteRequest(string uri, RestAPICertificate cert, List<(string, string)> headers = null, List<(string, string)> parameters = null, Action<UnityWebRequest> callback = null)
        {
            if (parameters != null)
            {
                uri += "?";
                foreach (var parameter in parameters)
                {
                    uri += $"{parameter.Item1}={parameter.Item2}&";
                }
            }

            UnityWebRequest request = UnityWebRequest.Delete(uri);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader($"{header.Item1}", $"{header.Item2}");
                }
            }
            request.certificateHandler = cert;
            var operation = request.SendWebRequest();
            operation.completed += (context) =>
            {
                callback?.Invoke(request);
                cert?.Dispose();
            };
        }

        #endregion Public Methods
    }

    // Certificates

    public class RestAPICertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}