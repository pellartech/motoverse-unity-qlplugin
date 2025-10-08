using System;
using UnityEngine;

namespace QuantumLeap
{
    [System.Serializable]
    public class UDIStats
    {
        [Header("UDI Stats Information")]
        public string brand;
        public string model;
        public int sequentialId;
        public int totalMileage;
        public int totalRaces;
        public int totalRankedRaces;
        public int totalCircuits;
        public int wins;
        public int podiumFinishes;

        public UDIStats()
        {
            // Default constructor
        }

        public UDIStats(string brand, string model, int sequentialId, int totalMileage, 
                       int totalRaces, int totalRankedRaces, int totalCircuits, 
                       int wins, int podiumFinishes)
        {
            this.brand = brand;
            this.model = model;
            this.sequentialId = sequentialId;
            this.totalMileage = totalMileage;
            this.totalRaces = totalRaces;
            this.totalRankedRaces = totalRankedRaces;
            this.totalCircuits = totalCircuits;
            this.wins = wins;
            this.podiumFinishes = podiumFinishes;
        }

        /// <summary>
        /// Creates a UDIStats from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDIStats data</param>
        /// <returns>UDIStats object</returns>
        public static UDIStats FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDIStats>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDIStats from JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts UDIStats to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDIStats</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDIStats to JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"UDI Stats [{brand} {model}] - Races: {totalRaces}, Mileage: {totalMileage}, Wins: {wins}";
        }

        /// <summary>
        /// Calculates win percentage
        /// </summary>
        /// <returns>Win percentage (0-100) or 0 if no races</returns>
        public float GetWinPercentage()
        {
            if (totalRaces == 0) return 0f;
            return (float)wins / totalRaces * 100f;
        }

        /// <summary>
        /// Calculates podium finish percentage
        /// </summary>
        /// <returns>Podium finish percentage (0-100) or 0 if no races</returns>
        public float GetPodiumPercentage()
        {
            if (totalRaces == 0) return 0f;
            return (float)podiumFinishes / totalRaces * 100f;
        }

        /// <summary>
        /// Calculates ranked race percentage
        /// </summary>
        /// <returns>Ranked race percentage (0-100) or 0 if no races</returns>
        public float GetRankedRacePercentage()
        {
            if (totalRaces == 0) return 0f;
            return (float)totalRankedRaces / totalRaces * 100f;
        }

        /// <summary>
        /// Calculates average mileage per race
        /// </summary>
        /// <returns>Average mileage per race or 0 if no races</returns>
        public float GetAverageMileagePerRace()
        {
            if (totalRaces == 0) return 0f;
            return (float)totalMileage / totalRaces;
        }

        /// <summary>
        /// Calculates average mileage per circuit
        /// </summary>
        /// <returns>Average mileage per circuit or 0 if no circuits</returns>
        public float GetAverageMileagePerCircuit()
        {
            if (totalCircuits == 0) return 0f;
            return (float)totalMileage / totalCircuits;
        }

        /// <summary>
        /// Resets all stats to zero
        /// </summary>
        public void ResetStats()
        {
            totalMileage = 0;
            totalRaces = 0;
            totalRankedRaces = 0;
            totalCircuits = 0;
            wins = 0;
            podiumFinishes = 0;
        }

        /// <summary>
        /// Creates a copy of the current stats
        /// </summary>
        /// <returns>New UDIStats instance with same values</returns>
        public UDIStats Clone()
        {
            return new UDIStats(brand, model, sequentialId, totalMileage, totalRaces, 
                               totalRankedRaces, totalCircuits, wins, podiumFinishes);
        }
    }

    [System.Serializable]
    public class UDIStatsResponse
    {
        [Header("API Response")]
        public bool success;
        public UDIStats data;
        public string message;
        public string error;

        public UDIStatsResponse()
        {
            // Default constructor
        }

        public UDIStatsResponse(bool success, UDIStats data, string message = null, string error = null)
        {
            this.success = success;
            this.data = data;
            this.message = message;
            this.error = error;
        }

        /// <summary>
        /// Creates a UDIStatsResponse from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDIStats response data</param>
        /// <returns>UDIStatsResponse object</returns>
        public static UDIStatsResponse FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDIStatsResponse>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDIStatsResponse from JSON: {ex.Message}");
                return new UDIStatsResponse(false, null, null, ex.Message);
            }
        }

        /// <summary>
        /// Converts UDIStatsResponse to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDIStatsResponse</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDIStatsResponse to JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if the response is successful and has valid data
        /// </summary>
        /// <returns>True if successful and valid, false otherwise</returns>
        public bool IsValid()
        {
            return success && data != null;
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            if (success)
            {
                return $"UDI Stats Response: Success - {data?.ToString()}";
            }
            else
            {
                return $"UDI Stats Response: Failed - {error ?? message}";
            }
        }
    }
}
