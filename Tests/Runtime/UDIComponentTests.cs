using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;
using System.Collections;

namespace QuantumLeap.Tests
{
    public class UDIComponentTests
    {
        private GameObject _testGameObject;
        private UDIComponent _component;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject and component
            _testGameObject = new GameObject("TestUDIComponent");
            _component = _testGameObject.AddComponent<UDIComponent>();

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
        public void Test_UDIComponent_InitialState()
        {
            // Assert
            Assert.IsFalse(_component.IsInitialized);
            Assert.IsNull(_component.CurrentUDI);
        }

        [Test]
        public void Test_UDIComponent_Initialize_ShouldCallBaseInitialize()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");

            // Act
            _component.Initialize();

            // Assert
            Assert.IsTrue(_component.IsInitialized);
        }

        [Test]
        public void Test_UDIComponent_CreateUDI_WithValidData_ShouldReturnCoroutine()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var coroutine = _component.CreateUDI("test@example.com", "test-brand", "test-model");

            // Assert
            Assert.IsNotNull(coroutine, "CreateUDI should return a Coroutine");
        }

        [Test]
        public void Test_UDIComponent_CreateUDI_WithEmptyEmail_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            var coroutine = _component.CreateUDI("", "test-brand", "test-model");

            // Assert
            Assert.IsNull(coroutine, "CreateUDI should return null for empty email");
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire");
            Assert.AreEqual("Email is required", errorMessage);
        }

        [Test]
        public void Test_UDIComponent_CreateUDI_WithEmptyBrand_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            var coroutine = _component.CreateUDI("test@example.com", "", "test-model");

            // Assert
            Assert.IsNull(coroutine, "CreateUDI should return null for empty brand");
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire");
            Assert.AreEqual("Brand is required", errorMessage);
        }

        [Test]
        public void Test_UDIComponent_CreateUDI_WithEmptyModel_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            var coroutine = _component.CreateUDI("test@example.com", "test-brand", "");

            // Assert
            Assert.IsNull(coroutine, "CreateUDI should return null for empty model");
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire");
            Assert.AreEqual("Model is required", errorMessage);
        }

        [Test]
        public void Test_UDIComponent_GetUDIById_WithValidId_ShouldReturnCoroutine()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var coroutine = _component.GetUDIById("test-udi-id");

            // Assert
            Assert.IsNotNull(coroutine, "GetUDIById should return a Coroutine");
        }

        [Test]
        public void Test_UDIComponent_GetUDIById_WithEmptyId_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            var coroutine = _component.GetUDIById("");

            // Assert
            Assert.IsNull(coroutine, "GetUDIById should return null for empty ID");
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire");
            Assert.AreEqual("UDI ID is required", errorMessage);
        }

        [Test]
        public void Test_UDIComponent_GetUDIById_WithNullId_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
            {
                errorEventFired = true;
                errorMessage = message;
            };

            // Act
            var coroutine = _component.GetUDIById(null);

            // Assert
            Assert.IsNull(coroutine, "GetUDIById should return null for null ID");
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire");
            Assert.AreEqual("UDI ID is required", errorMessage);
        }

        [Test]
        public void Test_UDIComponent_OnUDIReceived_Event_ShouldFire()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool eventFired = false;
            UDI receivedUDI = null;
            _component.OnUDIReceived += (action, udi) =>
            {
                eventFired = true;
                receivedUDI = udi;
            };

            // Act - Simulate receiving UDI data
            string testJson = @"{
                ""success"": true,
                ""data"": {
                    ""id"": ""test-udi-001"",
                    ""ownerId"": ""test-owner"",
                    ""edition"": ""test-edition"",
                    ""linkedDvpId"": ""1"",
                    ""type"": ""default"",
                    ""tokenId"": null,
                    ""fusedAt"": null,
                    ""studioId"": ""test-studio"",
                    ""userId"": null,
                    ""sequentialId"": 1,
                    ""email"": ""test@example.com"",
                    ""provenance"": {
                        ""type"": ""default"",
                        ""brand"": ""test-brand"",
                        ""model"": ""test-model"",
                        ""issuedTo"": {
                            ""email"": ""test@example.com""
                        },
                        ""createdAt"": ""2023-01-01T12:00:00.000Z"",
                        ""createdFor"": ""test-user""
                    }
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
            Assert.IsTrue(eventFired, "OnUDIReceived event should fire when data is received");
            Assert.IsNotNull(receivedUDI, "UDI should not be null");
            Assert.AreEqual("test-udi-001", receivedUDI.id);
        }

        [Test]
        public void Test_UDIComponent_OnUDIError_Event_ShouldFire()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();
            bool errorEventFired = false;
            string errorMessage = "";
            _component.OnUDIError += (message) =>
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
            Assert.IsTrue(errorEventFired, "OnUDIError event should fire when invalid data is received");
            Assert.IsTrue(errorMessage.Contains("Error parsing UDI data"), "Error message should indicate parsing failure");
        }

        [Test]
        public void Test_UDIComponent_ClearUDI_ShouldResetCurrentUDI()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Set a mock UDI
            var mockUDI = new UDI(
                "test-id",
                "test-owner",
                "test-edition",
                new Provenance("default", "test-brand", "test-model", new IssuedTo("test@example.com"), DateTime.Now, "test-user"),
                "1",
                "default",
                null,
                null,
                "test-studio",
                null,
                1,
                "test@example.com"
            );

            // Use reflection to set the private field
            var field = typeof(UDIComponent).GetField("_currentUDI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_component, mockUDI);

            // Act
            _component.ClearUDI();

            // Assert
            Assert.IsNull(_component.CurrentUDI, "CurrentUDI should be null after clearing");
        }

        [Test]
        public void Test_UDIComponent_GetCurrentUDIId_WithNoUDI_ShouldReturnNull()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.GetCurrentUDIId();

            // Assert
            Assert.IsNull(result, "GetCurrentUDIId should return null when no UDI is loaded");
        }

        [Test]
        public void Test_UDIComponent_HasUDI_WithNoUDI_ShouldReturnFalse()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.HasUDI();

            // Assert
            Assert.IsFalse(result, "HasUDI should return false when no UDI is loaded");
        }

        [Test]
        public void Test_UDIComponent_GetCurrentBrand_WithNoUDI_ShouldReturnEmptyString()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.GetCurrentBrand();

            // Assert
            Assert.AreEqual(string.Empty, result, "GetCurrentBrand should return empty string when no UDI is loaded");
        }

        [Test]
        public void Test_UDIComponent_GetCurrentModel_WithNoUDI_ShouldReturnEmptyString()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.GetCurrentModel();

            // Assert
            Assert.AreEqual(string.Empty, result, "GetCurrentModel should return empty string when no UDI is loaded");
        }

        [Test]
        public void Test_UDIComponent_GetCurrentEmail_WithNoUDI_ShouldReturnEmptyString()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.GetCurrentEmail();

            // Assert
            Assert.AreEqual(string.Empty, result, "GetCurrentEmail should return empty string when no UDI is loaded");
        }

        [Test]
        public void Test_UDIComponent_IsCurrentUDIFused_WithNoUDI_ShouldReturnFalse()
        {
            // Arrange
            _component.SetApiUrl("https://api.example.com");
            _component.Initialize();

            // Act
            var result = _component.IsCurrentUDIFused();

            // Assert
            Assert.IsFalse(result, "IsCurrentUDIFused should return false when no UDI is loaded");
        }

        [Test]
        public void Test_UDICreateRequest_Initialization()
        {
            // Arrange
            var request = new UDICreateRequest("test@example.com", "test-brand", "test-model");

            // Assert
            Assert.AreEqual("test@example.com", request.email);
            Assert.AreEqual("test-brand", request.brand);
            Assert.AreEqual("test-model", request.model);
        }

        [Test]
        public void Test_UDICreateRequest_IsValid_WithValidData()
        {
            // Arrange
            var request = new UDICreateRequest("test@example.com", "test-brand", "test-model");

            // Act & Assert
            Assert.IsTrue(request.IsValid());
        }

        [Test]
        public void Test_UDICreateRequest_IsValid_WithInvalidEmail()
        {
            // Arrange
            var request = new UDICreateRequest("invalid-email", "test-brand", "test-model");

            // Act & Assert
            Assert.IsFalse(request.IsValid());
        }

        [Test]
        public void Test_UDICreateRequest_IsValid_WithEmptyBrand()
        {
            // Arrange
            var request = new UDICreateRequest("test@example.com", "", "test-model");

            // Act & Assert
            Assert.IsFalse(request.IsValid());
        }

        [Test]
        public void Test_UDICreateRequest_IsValid_WithEmptyModel()
        {
            // Arrange
            var request = new UDICreateRequest("test@example.com", "test-brand", "");

            // Act & Assert
            Assert.IsFalse(request.IsValid());
        }

        [Test]
        public void Test_UDICreateRequest_ToString()
        {
            // Arrange
            var request = new UDICreateRequest("test@example.com", "test-brand", "test-model");

            // Act
            var result = request.ToString();

            // Assert
            Assert.IsTrue(result.Contains("test@example.com"));
            Assert.IsTrue(result.Contains("test-brand"));
            Assert.IsTrue(result.Contains("test-model"));
        }
    }
}
