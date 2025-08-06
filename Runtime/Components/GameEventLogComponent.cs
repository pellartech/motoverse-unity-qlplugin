using System;
using System.Collections;
using UnityEngine;

namespace QuantumLeap
{
    public class GameEventLogComponent : QuantumLeapComponent
    {
        private GameEventLog _gameEventLog = null;

        public GameEventLog GameEventLog => _gameEventLog;

        public event Action<GameEventLog> OnGameEventLogReceived;
        public event Action<string> OnGameEventLogError;

        public override void Initialize()
        {
            base.Initialize();

            OnComponentInitialized += OnGameEventLogInitialized;

            OnDataReceived += OnGameEventLogDataReceived;

            OnErrorOccurred += OnGameEventLogDataError;
        }

        public Coroutine LogGameEvent(string eventType, string category, int categoryNumber, string brand, string model, string tokenId, GameEventData eventData)
        {
            var jsonData = GameEventLog.GenerateGameEventLogInput(eventType, category, categoryNumber, brand, model, tokenId, eventData);
            return StartCoroutine(PostDataCoroutine($"{ApiUrl}/game-studios/events", jsonData));
        }

        private void OnGameEventLogDataReceived(string data)
        {
            _gameEventLog = GameEventLog.FromJson(data ?? "");
            
            if (_gameEventLog != null)
            {
                OnGameEventLogReceived?.Invoke(_gameEventLog);
            }
            else
            {
                Debug.LogError("Failed to parse GameEventLog from response data");
                OnGameEventLogDataError("Failed to parse GameEventLog data");
            }
        }

        private void OnGameEventLogInitialized()
        {
            OnDataReceived -= OnGameEventLogDataReceived;
            OnErrorOccurred -= OnGameEventLogDataError;
        }

        private void OnGameEventLogDataError(string error)
        {
            Debug.LogError($"GameEventLogComponent: OnGameEventLogError: {error}");
            OnGameEventLogError?.Invoke(error);
        }
    }
} 