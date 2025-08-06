using System;
using UnityEngine;

namespace QuantumLeap
{
    [Serializable]
    public class GameStudioResponse
    {
        public bool success;
        public GameStudio data;
    }

    [Serializable]
    public class GameStudio
    {
        public string id;
        public string studioName;
        public int tierLimit;
        public string tier;
        public int requestsMade;
        public string contactEmail;
        public bool isActive;
        public DateTime created_at;
        public DateTime updated_at;

        public GameStudio(string id, string studioName, string tier, int tierLimit, int requestsMade, string contactEmail, bool isActive, DateTime created_at, DateTime updated_at)
        {
            this.id = id;
            this.studioName = studioName;
            this.tier = tier;
            this.tierLimit = tierLimit;
            this.requestsMade = requestsMade;
            this.contactEmail = contactEmail;
            this.isActive = isActive;
            this.created_at = created_at;
            this.updated_at = updated_at;
        }

        public string ToJson()
        {
            if (this == null)
            {
                return "{}";
            }
            return JsonUtility.ToJson(this);
        }

        public static GameStudio FromJson(string json)
        {
            try
            {
                // First try to parse as a wrapped response
                var response = JsonUtility.FromJson<GameStudioResponse>(json);
                if (response != null && response.success && response.data != null)
                {
                    return response.data;
                }
                
                // If that fails, try to parse directly as GameStudio
                return JsonUtility.FromJson<GameStudio>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse GameStudio from JSON: {ex.Message}");
                Debug.LogError($"JSON content: {json}");
                return null;
            }
        }

        public static GameStudio[] FromJsonArray(string json)
        {
            return JsonUtility.FromJson<GameStudio[]>(json);
        }
    }
}