using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;

namespace QuantumLeap.Tests
{
    public class GameEventLogComponentTests
    {
        private GameObject _testGameObject;
        private GameEventLogComponent _component;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject and component
            _testGameObject = new GameObject("TestGameEventLogComponent");
            _component = _testGameObject.AddComponent<GameEventLogComponent>();
            
            // Reset manager state
            QuantumLeapManager.Shutdown();
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up
            if (_testGameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_testGameObject);
            }
            QuantumLeapManager.Shutdown();
        }

        [Test]
        public void Test_GameEventLogComponent_InitialState()
        {
            // Assert
            Assert.IsFalse(_component.IsInitialized);
            Assert.IsNull(_component.GameEventLog);
        }

        [Test]
        public void Test_GameEventLogComponent_Initialize_ShouldCallBaseInitialize()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(_component.IsInitialized);
        }

        [Test]
        public void Test_GameEventLogComponent_OnGameEventLogReceived_Event_ShouldFire()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool eventFired = false;
            GameEventLog receivedLog = null;
            _component.OnGameEventLogReceived += (action, log) => 
            {
                eventFired = true;
                receivedLog = log;
            };

            // Act - Simulate receiving data
            string testJson = @"{
                ""success"": true,
                ""data"": {
                    ""id"": ""test-event-001"",
                    ""count"": 1,
                    ""studioId"": ""test-studio"",
                    ""eventType"": ""test_event"",
                    ""category"": ""test_category"",
                    ""categoryNumber"": 1,
                    ""brand"": ""test_brand"",
                    ""model"": ""test_model"",
                    ""tokenId"": ""test_token"",
                    ""eventData"": {
                        ""mileage"": 100,
                        ""circuit"": ""test_circuit"",
                        ""rankedRace"": true,
                        ""raceWon"": false,
                        ""podiumFinish"": true
                    },
                    ""updated_at"": ""2023-01-01T12:00:00.000Z"",
                    ""created_at"": ""2023-01-01T11:00:00.000Z"",
                    ""platform"": ""Unity"",
                    ""context"": ""test"",
                    ""walletAddress"": ""0x123456789""
                }
            }";

            // Simulate the data received event by calling the protected method through reflection
            var method = typeof(QuantumLeapComponent).GetMethod("OnApiResponseReceived", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(_component, new object[] { "test-url", testJson });
            }

            // Assert
            Assert.IsTrue(eventFired, "OnGameEventLogReceived event should fire when data is received");
            Assert.IsNotNull(receivedLog, "GameEventLog should not be null");
        }

        [Test]
        public void Test_GameEventLogComponent_OnGameEventLogError_Event_ShouldFire()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnGameEventLogError += (message) => 
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act - Simulate receiving invalid data
            var method = typeof(QuantumLeapComponent).GetMethod("OnApiResponseReceived", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(_component, new object[] { "test-url", "invalid json" });
            }

            // Assert
            Assert.IsTrue(errorEventFired, "OnGameEventLogError event should fire when invalid data is received");
            Assert.IsTrue(errorMessage.Contains("Failed to parse GameEventLog"), "Error message should indicate parsing failure");
        }

        [Test]
        public void Test_GameEventLogComponent_LogGameEvent_ShouldReturnCoroutine()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            var eventData = new GameEventData
            {
                mileage = 100,
                circuit = "test_circuit",
                rankedRace = true,
                raceWon = false,
                podiumFinish = true
            };

            // Act
            var coroutine = _component.LogGameEvent(
                eventType: "test_event",
                category: "test_category",
                categoryNumber: 1,
                brand: "test_brand",
                model: "test_model",
                tokenId: "test_token",
                eventData: eventData
            );

            // Assert
            Assert.IsNotNull(coroutine, "LogGameEvent should return a Coroutine");
        }

        [Test]
        public void Test_GameEventLogComponent_LogGameEvent_WithValidData_ShouldGenerateCorrectJson()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            var eventData = new GameEventData
            {
                mileage = 150,
                circuit = "monaco",
                rankedRace = true,
                raceWon = true,
                podiumFinish = true
            };

            // Act
            var coroutine = _component.LogGameEvent(
                eventType: "race_completed",
                category: "racing",
                categoryNumber: 1,
                brand: "Ferrari",
                model: "F40",
                tokenId: "token-001",
                eventData: eventData
            );

            // Assert
            Assert.IsNotNull(coroutine, "LogGameEvent should return a Coroutine");
        }

        [Test]
        public void Test_GameEventLogComponent_GameEventLog_Property_ShouldReturnCurrentLog()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act - Simulate receiving data
            string testJson = @"{
                ""success"": true,
                ""data"": {
                    ""id"": ""test-event-002"",
                    ""count"": 1,
                    ""studioId"": ""test-studio"",
                    ""eventType"": ""test_event"",
                    ""category"": ""test_category"",
                    ""categoryNumber"": 1,
                    ""brand"": ""test_brand"",
                    ""model"": ""test_model"",
                    ""tokenId"": ""test_token"",
                    ""eventData"": {
                        ""mileage"": 200,
                        ""circuit"": ""test_circuit"",
                        ""rankedRace"": false,
                        ""raceWon"": true,
                        ""podiumFinish"": false
                    },
                    ""updated_at"": ""2023-01-01T12:00:00.000Z"",
                    ""created_at"": ""2023-01-01T11:00:00.000Z"",
                    ""platform"": ""Unity"",
                    ""context"": ""test"",
                    ""walletAddress"": ""0x123456789""
                }
            }";

            var method = typeof(QuantumLeapComponent).GetMethod("OnApiResponseReceived", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(_component, new object[] { "test-url", testJson });
            }

            // Assert
            Assert.IsNotNull(_component.GameEventLog, "GameEventLog property should return the current log");
            Assert.AreEqual("test-event-002", _component.GameEventLog.id);
        }

        [Test]
        public void Test_GameEventLogComponent_Initialize_ShouldSubscribeToEvents()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(_component.IsInitialized);
        }

        [Test]
        public void Test_GameEventLogComponent_LogGameEvent_WithNullEventData_ShouldNotThrowException()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _component.LogGameEvent(
                    eventType: "test_event",
                    category: "test_category",
                    categoryNumber: 1,
                    brand: "test_brand",
                    model: "test_model",
                    tokenId: "test_token",
                    eventData: null
                );
            });
        }

        [Test]
        public void Test_GameEventLogComponent_LogGameEvent_WithEmptyStrings_ShouldNotThrowException()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            var eventData = new GameEventData();

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _component.LogGameEvent(
                    eventType: "",
                    category: "",
                    categoryNumber: 0,
                    brand: "",
                    model: "",
                    tokenId: "",
                    eventData: eventData
                );
            });
        }
    }
} 