using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace QuantumLeap
{
    public static class QuantumLeapManager
    {
        private static bool _isInitialized = false;
        private static float _requestTimeout = 30f;
        private static int _maxRetries = 3;
        private static float _retryDelay = 1f;
        private static readonly object _lockObject = new object();

        public static event Action OnInitialized;

        public static event Action<string, object> OnApiResponseReceived;

        public static event Action<string> OnError;

        public static bool IsInitialized => _isInitialized;
        public static int MaxRetries => _maxRetries;
        public static float RetryDelay => _retryDelay;

        public static void Initialize()
        {
            Initialize(30f);
        }

        public static void Initialize(float timeoutSeconds)
        {
            Initialize(timeoutSeconds, 3, 1f);
        }

        public static void Initialize(float timeoutSeconds, int maxRetries, float retryDelay)
        {
            if (_isInitialized)
            {
                QuantumLeapLogger.LogWarning("QuantumLeapManager is already initialized");
                return;
            }

            lock (_lockObject)
            {
                if (_isInitialized) return;

                try
                {
                    _requestTimeout = timeoutSeconds;
                    _maxRetries = maxRetries;
                    _retryDelay = retryDelay;
                    _isInitialized = true;

                    QuantumLeapLogger.Log($"QuantumLeapManager initialized successfully with timeout: {timeoutSeconds}s, max retries: {maxRetries}, retry delay: {retryDelay}s");
                    OnInitialized?.Invoke();
                }
                catch (Exception ex)
                {
                    QuantumLeapLogger.LogError($"Failed to initialize QuantumLeapManager: {ex.Message}");
                    OnError?.Invoke($"Initialization failed: {ex.Message}");
                }
            }
        }

        public static async Task<string> FetchDataAsync(string url, Dictionary<string, string> headers = null)
        {
            return await FetchDataWithRetryAsync(url, headers, 0);
        }

        private static async Task<string> FetchDataWithRetryAsync(string url, Dictionary<string, string> headers, int currentRetry)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("QuantumLeapManager must be initialized before fetching data");
            }

            try
            {
                using (var request = UnityWebRequest.Get(url))
                {
                    request.timeout = (int)_requestTimeout;

                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    var operation = request.SendWebRequest();

                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var errorMessage = $"Failed to fetch data from {url} (attempt {currentRetry + 1}/{_maxRetries + 1}): {request.error}";
                        QuantumLeapLogger.LogWarning(errorMessage);

                        if (currentRetry < _maxRetries)
                        {
                            QuantumLeapLogger.Log($"Retrying fetch in {_retryDelay} seconds... (attempt {currentRetry + 1}/{_maxRetries})");
                            await Task.Delay((int)(_retryDelay * 1000));
                            return await FetchDataWithRetryAsync(url, headers, currentRetry + 1);
                        }
                        else
                        {
                            QuantumLeapLogger.LogError($"Max retries ({_maxRetries}) reached for fetch. Giving up.");
                            OnError?.Invoke(errorMessage);
                            throw new Exception(errorMessage);
                        }
                    }

                    var content = request.downloadHandler.text;

                    QuantumLeapLogger.Log($"API Response received from {url}: {content}");
                    OnApiResponseReceived?.Invoke(url, content);

                    return content;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to fetch data from {url} (attempt {currentRetry + 1}/{_maxRetries + 1}): {ex.Message}";
                QuantumLeapLogger.LogError(errorMessage);

                if (currentRetry < _maxRetries)
                {
                    QuantumLeapLogger.Log($"Retrying fetch in {_retryDelay} seconds... (attempt {currentRetry + 1}/{_maxRetries})");
                    await Task.Delay((int)(_retryDelay * 1000));
                    return await FetchDataWithRetryAsync(url, headers, currentRetry + 1);
                }
                else
                {
                    QuantumLeapLogger.LogError($"Max retries ({_maxRetries}) reached for fetch. Giving up.");
                    OnError?.Invoke(errorMessage);
                    throw;
                }
            }
        }

        public static async Task<string> PostDataAsync(string url, string data, Dictionary<string, string> headers = null)
        {
            return await PostDataWithRetryAsync(url, data, headers, 0);
        }

        private static async Task<string> PostDataWithRetryAsync(string url, string data, Dictionary<string, string> headers, int currentRetry)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("QuantumLeapManager must be initialized before posting data");
            }

            try
            {
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                uploadHandler.contentType = "application/json";

                using (var request = new UnityWebRequest(url, "POST"))
                {
                    request.timeout = (int)_requestTimeout;
                    request.uploadHandler = uploadHandler;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    var operation = request.SendWebRequest();

                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var errorMessage = $"Failed to post data to {url} (attempt {currentRetry + 1}/{_maxRetries + 1}): {request.error}";
                        QuantumLeapLogger.LogWarning(errorMessage);

                        if (currentRetry < _maxRetries)
                        {
                            QuantumLeapLogger.Log($"Retrying post in {_retryDelay} seconds... (attempt {currentRetry + 1}/{_maxRetries})");
                            await Task.Delay((int)(_retryDelay * 1000));
                            return await PostDataWithRetryAsync(url, data, headers, currentRetry + 1);
                        }
                        else
                        {
                            QuantumLeapLogger.LogError($"Max retries ({_maxRetries}) reached for post. Giving up.");
                            OnError?.Invoke(errorMessage);
                            throw new Exception(errorMessage);
                        }
                    }

                    var content = request.downloadHandler.text;

                    QuantumLeapLogger.Log($"POST Response received from {url}: {content}");
                    OnApiResponseReceived?.Invoke(url, content);

                    return content;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to post data to {url} (attempt {currentRetry + 1}/{_maxRetries + 1}): {ex.Message}";
                QuantumLeapLogger.LogError(errorMessage);

                if (currentRetry < _maxRetries)
                {
                    QuantumLeapLogger.Log($"Retrying post in {_retryDelay} seconds... (attempt {currentRetry + 1}/{_maxRetries})");
                    await Task.Delay((int)(_retryDelay * 1000));
                    return await PostDataWithRetryAsync(url, data, headers, currentRetry + 1);
                }
                else
                {
                    QuantumLeapLogger.LogError($"Max retries ({_maxRetries}) reached for post. Giving up.");
                    OnError?.Invoke(errorMessage);
                    throw;
                }
            }
        }

        public static void SetRetryConfiguration(int maxRetries, float retryDelay)
        {
            if (maxRetries < 0)
            {
                QuantumLeapLogger.LogWarning("Max retries cannot be negative. Setting to 0.");
                _maxRetries = 0;
            }
            else
            {
                _maxRetries = maxRetries;
            }

            if (retryDelay < 0)
            {
                QuantumLeapLogger.LogWarning("Retry delay cannot be negative. Setting to 0.");
                _retryDelay = 0f;
            }
            else
            {
                _retryDelay = retryDelay;
            }

            QuantumLeapLogger.Log($"Retry configuration updated: max retries = {_maxRetries}, retry delay = {_retryDelay}s");
        }

        public static void Shutdown()
        {
            if (!_isInitialized) return;

            lock (_lockObject)
            {
                if (!_isInitialized) return;

                try
                {
                    _isInitialized = false;

                    QuantumLeapLogger.Log("QuantumLeapManager shut down successfully");
                }
                catch (Exception ex)
                {
                    QuantumLeapLogger.LogError($"Error during shutdown: {ex.Message}");
                }
            }
        }
    }
}
