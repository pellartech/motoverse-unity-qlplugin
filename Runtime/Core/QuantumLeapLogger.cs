using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuantumLeap
{
    public static class QuantumLeapLogger
    {
        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        public static LogLevel CurrentLogLevel = LogLevel.Info;

        public static bool IncludeTimestamps = true;

        public static bool IncludeLogLevel = true;

        public static event Action<LogLevel, string> OnLogMessage;

        public static void LogDebug(string message, object context = null)
        {
            Log(LogLevel.Debug, message, context);
        }

        public static void Log(string message, object context = null)
        {
            Log(LogLevel.Info, message, context);
        }

        public static void LogWarning(string message, object context = null)
        {
            Log(LogLevel.Warning, message, context);
        }

        public static void LogError(string message, object context = null)
        {
            Log(LogLevel.Error, message, context);
        }

        public static void Log(LogLevel level, string message, object context = null)
        {
            if (level < CurrentLogLevel) return;

            var formattedMessage = FormatMessage(level, message, context);

            // Log to Unity console
            switch (level)
            {
                case LogLevel.Debug:
                    Debug.Log($"[QuantumLeap Debug] {formattedMessage}");
                    break;
                case LogLevel.Info:
                    Debug.Log($"[QuantumLeap] {formattedMessage}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[QuantumLeap] {formattedMessage}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[QuantumLeap] {formattedMessage}");
                    break;
            }

            // Fire the event
            OnLogMessage?.Invoke(level, formattedMessage);
        }

        private static string FormatMessage(LogLevel level, string message, object context)
        {
            var parts = new List<string>();

            // Add timestamp if enabled
            if (IncludeTimestamps)
            {
                parts.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]");
            }

            // Add log level if enabled
            if (IncludeLogLevel)
            {
                parts.Add($"[{level}]");
            }

            // Add the main message
            parts.Add(message);

            // Add context if provided
            if (context != null)
            {
                parts.Add($"Context: {context}");
            }

            return string.Join(" ", parts);
        }

        public static void ClearLogs()
        {
            Debug.Log("[QuantumLeap] Logs cleared");
        }
    }
}