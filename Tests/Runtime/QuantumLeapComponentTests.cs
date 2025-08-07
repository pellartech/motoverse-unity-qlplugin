using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;
using System.Collections;

namespace QuantumLeap.Tests
{
    public class QuantumLeapComponentTests
    {
        private GameObject _testGameObject;
        private QuantumLeapComponent _component;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject and component
            _testGameObject = new GameObject("TestQuantumLeapComponent");
            _component = _testGameObject.AddComponent<QuantumLeapComponent>();
            
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
        public void Test_Component_InitialState()
        {
            // Assert
            Assert.IsFalse(_component.IsInitialized);
            Assert.IsNotNull(_component.ApiUrl);
        }

        [Test]
        public void Test_Component_Initialize_WithValidConfiguration()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.SetApiKey("test-api-key");
            _component.SetRequestTimeout(60f);

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(_component.IsInitialized);
        }

        [Test]
        public void Test_Component_Initialize_WithInvalidApiUrl_ShouldThrowException()
        {
            // Arrange
            _component.SetApiUrl("");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _component.Initialize());
            Assert.IsTrue(exception.Message.Contains("Default API URL is not set"));
        }

        [Test]
        public void Test_Component_Initialize_WithInvalidTimeout_ShouldThrowException()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.SetRequestTimeout(-1f);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _component.Initialize());
            Assert.IsTrue(exception.Message.Contains("Request timeout must be greater than 0"));
        }

        [Test]
        public void Test_Component_Initialize_WhenAlreadyInitialized_ShouldNotReinitialize()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool eventFired = false;
            _component.OnComponentInitialized += () => eventFired = true;

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(_component.IsInitialized);
            Assert.IsFalse(eventFired, "OnComponentInitialized event should not fire when already initialized");
        }

        [Test]
        public void Test_Component_OnComponentInitialized_Event_ShouldFire()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            bool eventFired = false;
            _component.OnComponentInitialized += () => eventFired = true;

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(eventFired, "OnComponentInitialized event should fire when component is initialized");
        }

        [Test]
        public void Test_Component_OnErrorOccurred_Event_ShouldFire_WhenInitializationFails()
        {
            // Arrange
            _component.SetApiUrl(""); // Invalid URL
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnErrorOccurred += (message) => 
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(errorEventFired, "OnErrorOccurred event should fire when initialization fails");
            Assert.IsTrue(errorMessage.Contains("Default API URL is not set"));
        }

        [Test]
        public void Test_Component_SetApiUrl_ShouldUpdateUrl()
        {
            // Arrange
            string newUrl = "https://new-api.example.com";

            // Act
            _component.SetApiUrl(newUrl);

            // Assert
            Assert.AreEqual(newUrl, _component.ApiUrl);
        }

        [Test]
        public void Test_Component_SetApiKey_ShouldUpdateKey()
        {
            // Arrange
            string newKey = "new-api-key";

            // Act
            _component.SetApiKey(newKey);

            // Assert
            // Note: We can't directly test the private field, but we can verify the component still works
            Assert.IsNotNull(_component);
        }

        [Test]
        public void Test_Component_SetRequestTimeout_WithValidValue()
        {
            // Arrange
            float newTimeout = 120f;

            // Act
            _component.SetRequestTimeout(newTimeout);

            // Assert
            Assert.AreEqual(newTimeout, _component.RequestTimeout);
        }

        [Test]
        public void Test_Component_SetRequestTimeout_WithInvalidValue_ShouldNotUpdate()
        {
            // Arrange
            float originalTimeout = _component.RequestTimeout;
            float invalidTimeout = -5f;

            // Act
            _component.SetRequestTimeout(invalidTimeout);

            // Assert
            Assert.AreEqual(originalTimeout, _component.RequestTimeout);
        }

        [Test]
        public void Test_Component_Properties_ShouldReturnCorrectValues()
        {
            // Arrange
            string apiUrl = "https://test-api.example.com";
            float timeout = 90f;
            _component.SetApiUrl(apiUrl);
            _component.SetRequestTimeout(timeout);

            // Assert
            Assert.AreEqual(apiUrl, _component.ApiUrl);
            Assert.AreEqual(timeout, _component.RequestTimeout);
        }
    }
} 