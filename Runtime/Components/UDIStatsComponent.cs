using System;
using System.Collections;
using UnityEngine;

namespace QuantumLeap
{
    public class UDIStatsComponent : QuantumLeapComponent
    {
        private UDIStats _currentUDIStats = null;

        public UDIStats CurrentUDIStats => _currentUDIStats;

        public event Action<string, UDIStats> OnUDIStatsReceived;
        public event Action<string> OnUDIStatsError;

        public readonly string ACTION_GET_UDI_STATS = "GET:GetUDIStats";

        public override void Initialize()
        {
            base.Initialize();

            OnComponentInitialized += OnUDIInitialized;

            OnDataReceived += OnUDIStatsDataReceived;

            OnErrorOccurred += OnUDIStatsDataError;
        }


        /// <summary>
        /// Gets UDI stats by brand, model, and sequential ID
        /// </summary>
        /// <param name="brand">Brand name</param>
        /// <param name="model">Model name</param>
        /// <param name="sequentialId">Sequential ID</param>
        /// <returns>Coroutine for the API call</returns>
        public Coroutine GetUDIStats(string brand, string model, int sequentialId)
        {
            if (string.IsNullOrEmpty(brand))
            {
                OnUDIStatsError?.Invoke("Brand is required");
                return null;
            }

            if (string.IsNullOrEmpty(model))
            {
                OnUDIStatsError?.Invoke("Model is required");
                return null;
            }

            if (sequentialId < 0)
            {
                OnUDIStatsError?.Invoke("Sequential ID must be non-negative");
                return null;
            }

            string endpoint = $"{ApiUrl}/udis/default/{brand}/{model}/{sequentialId}/stats";
            return StartCoroutine(FetchDataCoroutine(ACTION_GET_UDI_STATS, endpoint));
        }

        private void OnUDIStatsDataReceived(string action, string data)
        {
            try
            {
                // Try to parse as UDIStatsResponse
                var udiStatsResponse = UDIStatsResponse.FromJson(data ?? "");

                if (udiStatsResponse != null && udiStatsResponse.success && udiStatsResponse.data != null)
                {
                    _currentUDIStats = udiStatsResponse.data;
                    OnUDIStatsReceived?.Invoke(action, _currentUDIStats);
                    QuantumLeapLogger.Log($"UDI Stats received successfully: {_currentUDIStats.brand} {_currentUDIStats.model}");
                    return;
                }

                // Try to parse as direct UDIStats
                _currentUDIStats = UDIStats.FromJson(data ?? "");

                if (_currentUDIStats != null)
                {
                    OnUDIStatsReceived?.Invoke(action, _currentUDIStats);
                    QuantumLeapLogger.Log($"UDI Stats received successfully: {_currentUDIStats.brand} {_currentUDIStats.model}");
                    return;
                }

                QuantumLeapLogger.LogError("Failed to parse UDIStats from response data");
                OnUDIStatsError?.Invoke("Failed to parse UDIStats data");
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Error parsing UDIStats data: {ex.Message}");
                OnUDIStatsError?.Invoke($"Error parsing UDIStats data: {ex.Message}");
            }
        }

        private void OnUDIInitialized()
        {
            OnDataReceived -= OnUDIStatsDataReceived;
            OnErrorOccurred -= OnUDIStatsDataError;
        }

        private void OnUDIStatsDataError(string error)
        {
            QuantumLeapLogger.LogError($"UDIStatsComponent: OnUDIStatsError: {error}");
            OnUDIStatsError?.Invoke(error);
        }

        /// <summary>
        /// Clears the current UDIStats data
        /// </summary>
        public void ClearUDIStats()
        {
            _currentUDIStats = null;
        }

        /// <summary>
        /// Checks if a UDIStats is currently loaded
        /// </summary>
        /// <returns>True if UDIStats is loaded, false otherwise</returns>
        public bool HasUDIStats()
        {
            return _currentUDIStats != null;
        }

        /// <summary>
        /// Gets the current UDIStats brand
        /// </summary>
        /// <returns>Brand name or empty string</returns>
        public string GetCurrentUDIStatsBrand()
        {
            return _currentUDIStats?.brand ?? string.Empty;
        }

        /// <summary>
        /// Gets the current UDIStats model
        /// </summary>
        /// <returns>Model name or empty string</returns>
        public string GetCurrentUDIStatsModel()
        {
            return _currentUDIStats?.model ?? string.Empty;
        }

        /// <summary>
        /// Gets the current UDIStats sequential ID
        /// </summary>
        /// <returns>Sequential ID or -1 if no stats</returns>
        public int GetCurrentUDIStatsSequentialId()
        {
            return _currentUDIStats?.sequentialId ?? -1;
        }

        /// <summary>
        /// Gets the current UDIStats total races
        /// </summary>
        /// <returns>Total races or 0 if no stats</returns>
        public int GetCurrentUDIStatsTotalRaces()
        {
            return _currentUDIStats?.totalRaces ?? 0;
        }

        /// <summary>
        /// Gets the current UDIStats total mileage
        /// </summary>
        /// <returns>Total mileage or 0 if no stats</returns>
        public int GetCurrentUDIStatsTotalMileage()
        {
            return _currentUDIStats?.totalMileage ?? 0;
        }

        /// <summary>
        /// Gets the current UDIStats wins
        /// </summary>
        /// <returns>Number of wins or 0 if no stats</returns>
        public int GetCurrentUDIStatsWins()
        {
            return _currentUDIStats?.wins ?? 0;
        }

        /// <summary>
        /// Gets the current UDIStats podium finishes
        /// </summary>
        /// <returns>Number of podium finishes or 0 if no stats</returns>
        public int GetCurrentUDIStatsPodiumFinishes()
        {
            return _currentUDIStats?.podiumFinishes ?? 0;
        }

        /// <summary>
        /// Gets the current UDIStats win percentage
        /// </summary>
        /// <returns>Win percentage or 0 if no stats</returns>
        public float GetCurrentUDIStatsWinPercentage()
        {
            return _currentUDIStats?.GetWinPercentage() ?? 0f;
        }

        /// <summary>
        /// Gets the current UDIStats podium percentage
        /// </summary>
        /// <returns>Podium percentage or 0 if no stats</returns>
        public float GetCurrentUDIStatsPodiumPercentage()
        {
            return _currentUDIStats?.GetPodiumPercentage() ?? 0f;
        }
    }

}
