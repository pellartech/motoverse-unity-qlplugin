using System;
using System.Collections;
using UnityEngine;

namespace QuantumLeap
{
    public class UDIComponent : QuantumLeapComponent
    {
        private UDI _currentUDI = null;
        public UDI CurrentUDI => _currentUDI;

        public event Action<string, UDI> OnUDIReceived;
        public event Action<string> OnUDIError;

        public readonly string ACTION_CREATE_UDI = "POST:CreateUDI";
        public readonly string ACTION_GET_UDI_BY_ID = "GET:GetUDIById";

        public override void Initialize()
        {
            base.Initialize();

            OnComponentInitialized += OnUDIInitialized;

            OnDataReceived += OnUDIDataReceived;

            OnErrorOccurred += OnUDIDataError;
        }

        /// <summary>
        /// Creates a UDI with the specified email, brand, and model
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="brand">Brand name</param>
        /// <param name="model">Model name</param>
        /// <returns>Coroutine for the API call</returns>
        public Coroutine CreateUDI(string email, string brand, string model)
        {
            if (string.IsNullOrEmpty(email))
            {
                OnUDIError?.Invoke("Email is required");
                return null;
            }

            if (!IsValidEmail(email))
            {
                OnUDIError?.Invoke("Invalid email format");
                return null;
            }

            if (string.IsNullOrEmpty(brand))
            {
                OnUDIError?.Invoke("Brand is required");
                return null;
            }

            if (string.IsNullOrEmpty(model))
            {
                OnUDIError?.Invoke("Model is required");
                return null;
            }

            var requestData = new UDICreateRequest
            {
                email = email,
                brand = brand,
                model = model
            };

            string jsonData = JsonUtility.ToJson(requestData);
            string endpoint = $"{ApiUrl}/udis/default/issue/{email}/{brand}/{model}";

            return StartCoroutine(PostDataCoroutine(ACTION_CREATE_UDI, endpoint, jsonData));
        }

        /// <summary>
        /// Gets UDI details by ID
        /// </summary>
        /// <param name="udiId">UDI ID</param>
        /// <returns>Coroutine for the API call</returns>
        public Coroutine GetUDIById(string udiId)
        {
            if (string.IsNullOrEmpty(udiId))
            {
                OnUDIError?.Invoke("UDI ID is required");
                return null;
            }

            string endpoint = $"{ApiUrl}/udis/{udiId}";
            return StartCoroutine(FetchDataCoroutine(ACTION_GET_UDI_BY_ID, endpoint));
        }

        /// <summary>
        /// Validates email format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return email.Contains("@") && email.Contains(".") && email.Length > 5;
        }

        private void OnUDIDataReceived(string action, string data)
        {
            try
            {
                // Try to parse as UDIResponse first
                var udiResponse = UDIResponse.FromJson(data ?? "");

                if (udiResponse != null && udiResponse.success && udiResponse.data != null)
                {
                    _currentUDI = udiResponse.data;
                    OnUDIReceived?.Invoke(action, _currentUDI);
                    QuantumLeapLogger.Log($"UDI received successfully: {_currentUDI.id}");
                    return;
                }

                // Try to parse as direct UDI
                _currentUDI = UDI.FromJson(data ?? "");

                if (_currentUDI != null && !string.IsNullOrEmpty(_currentUDI.id))
                {
                    OnUDIReceived?.Invoke(action, _currentUDI);
                    QuantumLeapLogger.Log($"UDI received successfully: {_currentUDI.id}");
                    return;
                }

                QuantumLeapLogger.LogError("Failed to parse UDI from response data");
                OnUDIError?.Invoke("Failed to parse UDI data");
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Error parsing UDI data: {ex.Message}");
                OnUDIError?.Invoke($"Error parsing UDI data: {ex.Message}");
            }
        }

        private void OnUDIInitialized()
        {
            OnDataReceived -= OnUDIDataReceived;
            OnErrorOccurred -= OnUDIDataError;
        }

        private void OnUDIDataError(string error)
        {
            QuantumLeapLogger.LogError($"UDIComponent: OnUDIError: {error}");
            OnUDIError?.Invoke(error);
        }

        /// <summary>
        /// Clears the current UDI data
        /// </summary>
        public void ClearUDI()
        {
            _currentUDI = null;
        }

        /// <summary>
        /// Gets the current UDI ID
        /// </summary>
        /// <returns>UDI ID or null if no UDI is loaded</returns>
        public string GetCurrentUDIId()
        {
            return _currentUDI?.id;
        }

        /// <summary>
        /// Checks if a UDI is currently loaded
        /// </summary>
        /// <returns>True if UDI is loaded, false otherwise</returns>
        public bool HasUDI()
        {
            return _currentUDI != null;
        }



        /// <summary>
        /// Gets the current UDI's brand
        /// </summary>
        /// <returns>Brand name or empty string</returns>
        public string GetCurrentBrand()
        {
            return _currentUDI?.GetBrand() ?? string.Empty;
        }

        /// <summary>
        /// Gets the current UDI's model
        /// </summary>
        /// <returns>Model name or empty string</returns>
        public string GetCurrentModel()
        {
            return _currentUDI?.GetModel() ?? string.Empty;
        }

        /// <summary>
        /// Gets the current UDI's sequential ID
        /// </summary>
        /// <returns>Sequential ID or 0</returns>
        public int GetCurrentSequentialId()
        {
            return _currentUDI?.GetSequentialId() ?? 0;
        }

        /// <summary>
        /// Gets the current UDI's email
        /// </summary>
        /// <returns>Email or empty string</returns>
        public string GetCurrentEmail()
        {
            return _currentUDI?.email ?? string.Empty;
        }

        /// <summary>
        /// Checks if the current UDI is fused
        /// </summary>
        /// <returns>True if fused, false otherwise</returns>
        public bool IsCurrentUDIFused()
        {
            return _currentUDI?.IsFused() ?? false;
        }
    }

    /// <summary>
    /// Request model for creating UDI
    /// </summary>
    [System.Serializable]
    public class UDICreateRequest
    {
        public string email;
        public string brand;
        public string model;

        public UDICreateRequest()
        {
            // Default constructor
        }

        public UDICreateRequest(string email, string brand, string model)
        {
            this.email = email;
            this.brand = brand;
            this.model = model;
        }

        /// <summary>
        /// Validates the request data
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(email) &&
                   !string.IsNullOrEmpty(brand) &&
                   !string.IsNullOrEmpty(model) &&
                   email.Contains("@") &&
                   email.Contains(".");
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"UDI Create Request: {email} - {brand} {model}";
        }
    }
}
