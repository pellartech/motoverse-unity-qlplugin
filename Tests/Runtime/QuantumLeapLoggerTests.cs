using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;

namespace QuantumLeap.Tests
{
    public class QuantumLeapLoggerTests
    {
        [SetUp]
        public void Setup()
        {
            // Reset logger state before each test
            QuantumLeapLogger.CurrentLogLevel = QuantumLeapLogger.LogLevel.Info;
            QuantumLeapLogger.IncludeTimestamps = true;
            QuantumLeapLogger.IncludeLogLevel = true;
        }

        [Test]
        public void Test_Logger_InitialState()
        {
            // Assert
            Assert.AreEqual(QuantumLeapLogger.LogLevel.Info, QuantumLeapLogger.CurrentLogLevel);
            Assert.IsTrue(QuantumLeapLogger.IncludeTimestamps);
            Assert.IsTrue(QuantumLeapLogger.IncludeLogLevel);
        }

        [Test]
        public void Test_Logger_DebugLevel_WhenCurrentLevelIsInfo_ShouldNotLog()
        {
            // Arrange
            QuantumLeapLogger.CurrentLogLevel = QuantumLeapLogger.LogLevel.Info;
            bool logMessageFired = false;
            QuantumLeapLogger.OnLogMessage += (level, message) => logMessageFired = true;

            // Act
            QuantumLeapLogger.LogDebug("Debug message");

            // Assert
            Assert.IsFalse(logMessageFired, "Debug message should not be logged when current level is Info");
        }

        [Test]
        public void Test_Logger_DebugLevel_WhenCurrentLevelIsDebug_ShouldLog()
        {
            // Arrange
            QuantumLeapLogger.CurrentLogLevel = QuantumLeapLogger.LogLevel.Debug;
            bool logMessageFired = false;
            QuantumLeapLogger.OnLogMessage += (level, message) => logMessageFired = true;

            // Act
            QuantumLeapLogger.LogDebug("Debug message");

            // Assert
            Assert.IsTrue(logMessageFired, "Debug message should be logged when current level is Debug");
        }

        [Test]
        public void Test_Logger_InfoLevel_ShouldLog()
        {
            // Arrange
            bool logMessageFired = false;
            QuantumLeapLogger.OnLogMessage += (level, message) => logMessageFired = true;

            // Act
            QuantumLeapLogger.Log("Info message");

            // Assert
            Assert.IsTrue(logMessageFired, "Info message should be logged");
        }

        [Test]
        public void Test_Logger_WarningLevel_ShouldLog()
        {
            // Arrange
            bool logMessageFired = false;
            QuantumLeapLogger.OnLogMessage += (level, message) => logMessageFired = true;

            // Act
            QuantumLeapLogger.LogWarning("Warning message");

            // Assert
            Assert.IsTrue(logMessageFired, "Warning message should be logged");
        }

        [Test]
        public void Test_Logger_ErrorLevel_ShouldLog()
        {
            // Arrange
            bool logMessageFired = false;
            QuantumLeapLogger.OnLogMessage += (level, message) => logMessageFired = true;

            // Act
            QuantumLeapLogger.LogError("Error message");

            // Assert
            Assert.IsTrue(logMessageFired, "Error message should be logged");
        }

        [Test]
        public void Test_Logger_IncludeTimestamps_WhenEnabled_ShouldIncludeTimestamp()
        {
            // Arrange
            QuantumLeapLogger.IncludeTimestamps = true;
            string loggedMessage = "";
            QuantumLeapLogger.OnLogMessage += (level, message) => loggedMessage = message;

            // Act
            QuantumLeapLogger.Log("Test message");

            // Assert
            Assert.IsTrue(loggedMessage.Contains(DateTime.Now.ToString("yyyy-MM-dd")), "Message should include timestamp");
        }

        [Test]
        public void Test_Logger_IncludeTimestamps_WhenDisabled_ShouldNotIncludeTimestamp()
        {
            // Arrange
            QuantumLeapLogger.IncludeTimestamps = false;
            string loggedMessage = "";
            QuantumLeapLogger.OnLogMessage += (level, message) => loggedMessage = message;

            // Act
            QuantumLeapLogger.Log("Test message");

            // Assert
            Assert.IsFalse(loggedMessage.Contains(DateTime.Now.ToString("yyyy-MM-dd")), "Message should not include timestamp");
        }

        [Test]
        public void Test_Logger_IncludeLogLevel_WhenEnabled_ShouldIncludeLogLevel()
        {
            // Arrange
            QuantumLeapLogger.IncludeLogLevel = true;
            string loggedMessage = "";
            QuantumLeapLogger.OnLogMessage += (level, message) => loggedMessage = message;

            // Act
            QuantumLeapLogger.LogWarning("Test message");

            // Assert
            Assert.IsTrue(loggedMessage.Contains("[Warning]"), "Message should include log level");
        }

        [Test]
        public void Test_Logger_IncludeLogLevel_WhenDisabled_ShouldNotIncludeLogLevel()
        {
            // Arrange
            QuantumLeapLogger.IncludeLogLevel = false;
            string loggedMessage = "";
            QuantumLeapLogger.OnLogMessage += (level, message) => loggedMessage = message;

            // Act
            QuantumLeapLogger.LogWarning("Test message");

            // Assert
            Assert.IsFalse(loggedMessage.Contains("[Warning]"), "Message should not include log level");
        }

        [Test]
        public void Test_Logger_Context_WhenProvided_ShouldIncludeContext()
        {
            // Arrange
            string loggedMessage = "";
            QuantumLeapLogger.OnLogMessage += (level, message) => loggedMessage = message;
            var context = new { TestProperty = "TestValue" };

            // Act
            QuantumLeapLogger.Log("Test message", context);

            // Assert
            Assert.IsTrue(loggedMessage.Contains("Context:"), "Message should include context");
            Assert.IsTrue(loggedMessage.Contains("TestValue"), "Message should include context value");
        }

        [Test]
        public void Test_Logger_ClearLogs_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => QuantumLeapLogger.ClearLogs(), "ClearLogs should not throw an exception");
        }

        [Test]
        public void Test_Logger_LogLevels_Ordering()
        {
            // Assert
            Assert.Less(QuantumLeapLogger.LogLevel.Debug, QuantumLeapLogger.LogLevel.Info);
            Assert.Less(QuantumLeapLogger.LogLevel.Info, QuantumLeapLogger.LogLevel.Warning);
            Assert.Less(QuantumLeapLogger.LogLevel.Warning, QuantumLeapLogger.LogLevel.Error);
        }
    }
} 