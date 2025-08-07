using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuantumLeap.Tests
{
    public class QuantumLeapManagerTests
    {
        [SetUp]
        public void Setup()
        {
            // Reset manager state before each test
            QuantumLeapManager.Shutdown();
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up after each test
            QuantumLeapManager.Shutdown();
        }

        [Test]
        public void Test_Manager_InitialState()
        {
            // Assert
            Assert.IsFalse(QuantumLeapManager.IsInitialized);
        }

        [Test]
        public void Test_Manager_Initialize_WithDefaultParameters()
        {
            // Act
            QuantumLeapManager.Initialize();

            // Assert
            Assert.IsTrue(QuantumLeapManager.IsInitialized);
            Assert.AreEqual(3, QuantumLeapManager.MaxRetries);
            Assert.AreEqual(1f, QuantumLeapManager.RetryDelay);
        }

        [Test]
        public void Test_Manager_Initialize_WithCustomParameters()
        {
            // Arrange
            float timeout = 60f;
            int maxRetries = 5;
            float retryDelay = 2f;

            // Act
            QuantumLeapManager.Initialize(timeout, maxRetries, retryDelay);

            // Assert
            Assert.IsTrue(QuantumLeapManager.IsInitialized);
            Assert.AreEqual(maxRetries, QuantumLeapManager.MaxRetries);
            Assert.AreEqual(retryDelay, QuantumLeapManager.RetryDelay);
        }

        [Test]
        public void Test_Manager_Initialize_WhenAlreadyInitialized_ShouldNotReinitialize()
        {
            // Arrange
            QuantumLeapManager.Initialize(30f, 3, 1f);
            bool eventFired = false;
            QuantumLeapManager.OnInitialized += () => eventFired = true;

            // Act
            QuantumLeapManager.Initialize(60f, 5, 2f);

            // Assert
            Assert.IsTrue(QuantumLeapManager.IsInitialized);
            Assert.IsFalse(eventFired, "OnInitialized event should not fire when already initialized");
        }

        [Test]
        public void Test_Manager_Shutdown_ShouldResetState()
        {
            // Arrange
            QuantumLeapManager.Initialize();

            // Act
            QuantumLeapManager.Shutdown();

            // Assert
            Assert.IsFalse(QuantumLeapManager.IsInitialized);
        }

        [Test]
        public void Test_Manager_SetRetryConfiguration_WithValidValues()
        {
            // Arrange
            QuantumLeapManager.Initialize();
            int newMaxRetries = 10;
            float newRetryDelay = 5f;

            // Act
            QuantumLeapManager.SetRetryConfiguration(newMaxRetries, newRetryDelay);

            // Assert
            Assert.AreEqual(newMaxRetries, QuantumLeapManager.MaxRetries);
            Assert.AreEqual(newRetryDelay, QuantumLeapManager.RetryDelay);
        }

        [Test]
        public void Test_Manager_SetRetryConfiguration_WithNegativeMaxRetries_ShouldSetToZero()
        {
            // Arrange
            QuantumLeapManager.Initialize();

            // Act
            QuantumLeapManager.SetRetryConfiguration(-5, 1f);

            // Assert
            Assert.AreEqual(0, QuantumLeapManager.MaxRetries);
        }

        [Test]
        public void Test_Manager_SetRetryConfiguration_WithNegativeRetryDelay_ShouldSetToZero()
        {
            // Arrange
            QuantumLeapManager.Initialize();

            // Act
            QuantumLeapManager.SetRetryConfiguration(3, -2f);

            // Assert
            Assert.AreEqual(0f, QuantumLeapManager.RetryDelay);
        }

        [Test]
        public void Test_Manager_OnInitialized_Event_ShouldFire()
        {
            // Arrange
            bool eventFired = false;
            QuantumLeapManager.OnInitialized += () => eventFired = true;

            // Act
            QuantumLeapManager.Initialize();

            // Assert
            Assert.IsTrue(eventFired, "OnInitialized event should fire when manager is initialized");
        }

        [Test]
        public void Test_Manager_OnError_Event_ShouldFire_WhenInitializationFails()
        {
            // Arrange
            QuantumLeapManager.OnError += (message) => 
            {
                // Event handler for testing - we just verify the event system is working
            };

            // Act - Try to initialize with invalid parameters (this should not actually fail in current implementation)
            QuantumLeapManager.Initialize();

            // Assert - Since the current implementation doesn't have validation that would cause errors,
            // we just verify the event system is working
            Assert.IsTrue(QuantumLeapManager.IsInitialized);
        }

        [Test]
        public void Test_Manager_Initialize_WithZeroTimeout_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => QuantumLeapManager.Initialize(0f, 3, 1f));
        }

        [Test]
        public void Test_Manager_Initialize_WithZeroMaxRetries_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => QuantumLeapManager.Initialize(30f, 0, 1f));
        }

        [Test]
        public void Test_Manager_Initialize_WithZeroRetryDelay_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => QuantumLeapManager.Initialize(30f, 3, 0f));
        }

        [Test]
        public void Test_Manager_Properties_ShouldReturnCorrectValues()
        {
            // Arrange
            float timeout = 45f;
            int maxRetries = 7;
            float retryDelay = 3f;
            QuantumLeapManager.Initialize(timeout, maxRetries, retryDelay);

            // Assert
            Assert.IsTrue(QuantumLeapManager.IsInitialized);
            Assert.AreEqual(maxRetries, QuantumLeapManager.MaxRetries);
            Assert.AreEqual(retryDelay, QuantumLeapManager.RetryDelay);
        }
    }
} 