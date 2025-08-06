using System;
using UnityEngine;

namespace QuantumLeap
{
    public class GameStudioComponent : QuantumLeapComponent
    {
        private GameStudio _gameStudio = null;

        public GameStudio GameStudio => _gameStudio;

        public event Action<GameStudio> OnGameStudioReceived;

        public override void Initialize()
        {
            base.Initialize();

            OnComponentInitialized += OnGameStudioInitialized;

            OnDataReceived += OnGameStudioDataReceived;

            OnErrorOccurred += OnGameStudioDataError;
        }

        public Coroutine GetGameStudioById(string studioId)
        {
            return StartCoroutine(FetchDataCoroutine($"{ApiUrl}/game-studios/{studioId}"));
        }

        private void OnGameStudioDataReceived(string data)
        {
            _gameStudio = GameStudio.FromJson(data ?? "");
            
            if (_gameStudio != null)
            {
                OnGameStudioReceived?.Invoke(_gameStudio);
            }
            else
            {
                Debug.LogError("Failed to parse GameStudio from response data");
                OnGameStudioDataError("Failed to parse GameStudio data");
            }
        }

        private void OnGameStudioInitialized()
        {
            OnDataReceived -= OnGameStudioDataReceived;
            OnErrorOccurred -= OnGameStudioDataError;
        }

        private void OnGameStudioDataError(string error)
        {
            Debug.LogError($"GameStudioComponent: OnGameStudioError: {error}");
        }
    }
}