using System;
using UnityEngine;

namespace QuantumLeap
{
    [System.Serializable]
    public class GameEventLogResponse
    {
        public bool success;
        public GameEventLog data;
    }

    [System.Serializable]
    public class GameEventLogInput
    {
        public string eventType;
        public string category;
        public int categoryNumber;
        public string brand;
        public string model;
        public string tokenId;
        public GameEventData eventData;
        public string timestamp;
    }

    [System.Serializable]
    public class GameEventData
    {
        public int mileage;
        public string circuit;
        public bool rankedRace;
        public bool raceWon;
        public bool podiumFinish;
        
        // Add any additional fields as needed
        public string customField1;
        public int customField2;
        public bool customField3;
    }

    [System.Serializable]
    public class GameEventLog
    {
        public string id;
        public int count;
        public string studioId;
        public string eventType;
        public string category;
        public int categoryNumber;
        public string brand;
        public string model;
        public string tokenId;
        public GameEventData eventData;
        public DateTime updated_at;
        public DateTime created_at;
        public string platform;
        public string context;
        public string walletAddress;

        public GameEventLog(string id, int count, string studioId, string eventType, string category, int categoryNumber, string brand, string model, string tokenId, GameEventData eventData, DateTime updated_at, DateTime created_at, string platform, string context, string walletAddress)
        {
            this.id = id;
            this.count = count;
            this.studioId = studioId;
            this.eventType = eventType;
            this.category = category;
            this.categoryNumber = categoryNumber;
            this.brand = brand;
            this.model = model;
            this.tokenId = tokenId;
            this.eventData = eventData;
            this.updated_at = updated_at;
            this.created_at = created_at;
            this.platform = platform;
            this.context = context;
            this.walletAddress = walletAddress;
        }

        public static string GenerateGameEventLogInput(string eventType, string category, int categoryNumber, string brand, string model, string tokenId, GameEventData eventData)
        {
            var input = new GameEventLogInput
            {
                eventType = eventType,
                category = category,
                categoryNumber = categoryNumber,
                brand = brand,
                model = model,
                tokenId = tokenId,
                eventData = eventData,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            return JsonUtility.ToJson(input);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static GameEventLog FromJson(string json)
        {
            try
            {
                // First try to parse as a wrapped response
                var response = JsonUtility.FromJson<GameEventLogResponse>(json);
                if (response != null && response.success && response.data != null)
                {
                    return response.data;
                }
                
                // If that fails, try to parse directly as GameEventLog
                return JsonUtility.FromJson<GameEventLog>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse GameEventLog from JSON: {ex.Message}");
                Debug.LogError($"JSON content: {json}");
                return null;
            }
        }

        public static GameEventLog[] FromJsonArray(string json)
        {
            return JsonUtility.FromJson<GameEventLog[]>(json);
        }
    }
}