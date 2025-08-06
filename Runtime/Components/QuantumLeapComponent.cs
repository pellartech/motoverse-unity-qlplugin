using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuantumLeap
{
    public class QuantumLeapComponent : MonoBehaviour
    {
        [Header("Quantum Leap Settings")]
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private bool _logToConsole = true;
        [SerializeField] private QuantumLeapLogger.LogLevel _logLevel = QuantumLeapLogger.LogLevel.Info;

        [Header("API Configuration")]
        [SerializeField] private string _apiUrl;

        [SerializeField] private string _apiKey;
        [SerializeField] private float _requestTimeout = 30f;

        [SerializeField] private int _maxRetries = 3;
        [SerializeField] private float _retryDelay = 1f;

        public event Action OnComponentInitialized;

        public event Action<string> OnDataReceived;

        public event Action<string> OnErrorOccurred;

        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            if (_autoInitialize)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (!_autoInitialize)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void ValidateInput()
        {
            if (string.IsNullOrEmpty(_apiUrl))
            {
                throw new Exception("Default API URL is not set");
            }

            if (_requestTimeout <= 0)
            {
                throw new Exception("Request timeout must be greater than 0");
            }
        }

        protected Dictionary<string, string> GetHeaders()
        {
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            };

            if (!string.IsNullOrEmpty(_apiKey))
            {
                headers["x-api-key"] = $"{_apiKey}";
            }

            return headers;
        }

        public virtual void Initialize()
        {
            ValidateInput();

            if (_isInitialized) return;

            try
            {
                QuantumLeapLogger.CurrentLogLevel = _logLevel;
                QuantumLeapLogger.IncludeTimestamps = true;
                QuantumLeapLogger.IncludeLogLevel = true;

                QuantumLeapManager.OnInitialized += OnManagerInitialized;
                QuantumLeapManager.OnApiResponseReceived += OnApiResponseReceived;
                QuantumLeapManager.OnError += OnManagerError;

                QuantumLeapManager.Initialize(_requestTimeout, _maxRetries, _retryDelay);

                _isInitialized = true;
                OnComponentInitialized?.Invoke();

                if (_logToConsole)
                {
                    QuantumLeapLogger.Log($"QuantumLeapComponent initialized on {gameObject.name} with timeout: {_requestTimeout}s");
                }
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to initialize QuantumLeapComponent: {ex.Message}");
                OnErrorOccurred?.Invoke($"Initialization failed: {ex.Message}");
            }
        }

        protected IEnumerator FetchDataCoroutine(string url)
        {
            if (!_isInitialized)
            {
                QuantumLeapLogger.LogError("Component not initialized. Call Initialize() first.");
                yield break;
            }

            var headers = GetHeaders();

            var task = QuantumLeapManager.FetchDataAsync(url, headers);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Exception != null)
            {
                QuantumLeapLogger.LogError($"Fetching data from {url} - exception: {task.Exception.Message}");
                var errorMessage = $"Failed to fetch data from {url}: {task.Exception.Message}";
                QuantumLeapLogger.LogError(errorMessage);
                OnErrorOccurred?.Invoke(errorMessage);
                yield break;
            }

            try
            {
                var result = task.Result;
                OnDataReceived?.Invoke(result);

                if (_logToConsole)
                {
                    QuantumLeapLogger.Log($"Data fetched successfully from {url}: {result}");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to fetch data from {url}: {ex.Message}";
                QuantumLeapLogger.LogError(errorMessage);
                OnErrorOccurred?.Invoke(errorMessage);
            }
        }

        protected IEnumerator PostDataCoroutine(string url, string data)
        {
            if (!_isInitialized)
            {
                QuantumLeapLogger.LogError("Component not initialized. Call Initialize() first.");
                yield break;
            }

            var headers = GetHeaders();

            var task = QuantumLeapManager.PostDataAsync(url, data, headers);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Exception != null)
            {
                var errorMessage = $"Failed to post data to {url}: {task.Exception.Message}";
                QuantumLeapLogger.LogError(errorMessage);
                OnErrorOccurred?.Invoke(errorMessage);
                yield break;
            }

            try
            {
                var result = task.Result;
                OnDataReceived?.Invoke(result);

                if (_logToConsole)
                {
                    QuantumLeapLogger.Log($"Data posted successfully to {url}: {result}");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to post data to {url}: {ex.Message}";
                QuantumLeapLogger.LogError(errorMessage);
                OnErrorOccurred?.Invoke(errorMessage);
            }
        }

        protected void OnManagerInitialized()
        {
            if (_logToConsole)
            {
                QuantumLeapLogger.Log("QuantumLeapManager initialized successfully");
            }
        }

        protected virtual void OnApiResponseReceived(string url, object data)
        {
            if (_logToConsole)
            {
                QuantumLeapLogger.Log($"API response received from {url}: {data}");
            }
        }

        protected void OnManagerError(string error)
        {
            if (_logToConsole)
            {
                QuantumLeapLogger.LogError($"Manager error: {error}");
            }
            OnErrorOccurred?.Invoke(error);
        }

        protected void Cleanup()
        {
            QuantumLeapManager.OnInitialized -= OnManagerInitialized;
            QuantumLeapManager.OnApiResponseReceived -= OnApiResponseReceived;
            QuantumLeapManager.OnError -= OnManagerError;

            _isInitialized = false;
        }

        public string ApiUrl => _apiUrl;

        public float RequestTimeout => _requestTimeout;

        public void SetApiUrl(string url)
        {
            _apiUrl = url;
        }

        public void SetApiKey(string key)
        {
            _apiKey = key;
        }

        public void SetRequestTimeout(float timeoutSeconds)
        {
            if (timeoutSeconds <= 0)
            {
                QuantumLeapLogger.LogWarning("Timeout must be greater than 0 seconds");
                return;
            }

            _requestTimeout = timeoutSeconds;
        }
    }
}