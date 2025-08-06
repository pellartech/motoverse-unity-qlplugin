# Motoverse Quantum Leap Plugin

A Unity plugin for fetching data from APIs and logging it with configurable logging system and easy integration.

## Features

- **API Integration**: Easy HTTP requests and data fetching
- **Configurable Logging**: Flexible logging system with multiple output formats
- **Event System**: Game event logging and tracking
- **Studio Management**: Game studio data management
- **Async Support**: Full async/await support for network operations
- **Unity Integration**: Seamless integration with Unity's component system

## Installation

### Via GitHub (Recommended)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL**
5. Enter: `https://github.com/pellartech/motoverse-unity-qlplugin.git`

### Via Unity Package Manager

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL**
5. Enter: `com.motoverse.qlplugin`

## Quick Start

### 1. Add the Component

```csharp
using QuantumLeap;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private QuantumLeapComponent quantumLeap;
    
    void Start()
    {
        // Initialize the plugin
        quantumLeap.Initialize();
    }
}
```

### 2. Configure Logging

```csharp
// Configure the logger
QuantumLeapLogger.Instance.SetLogLevel(LogLevel.Info);
QuantumLeapLogger.Instance.EnableConsoleOutput(true);
QuantumLeapLogger.Instance.EnableFileOutput(true, "logs/");
```

### 3. Make API Calls

```csharp
// Example API call
var response = await quantumLeap.FetchDataAsync("https://api.example.com/data");
if (response.IsSuccess)
{
    Debug.Log($"Data received: {response.Data}");
}
```

## Components

### QuantumLeapComponent
Main component for API integration and data management.

**Properties:**
- `ApiBaseUrl`: Base URL for API calls
- `Timeout`: Request timeout in seconds
- `RetryAttempts`: Number of retry attempts for failed requests

### GameEventLogComponent
Component for logging game events.

**Methods:**
- `LogEvent(string eventName, Dictionary<string, object> data)`
- `LogError(string error, Exception exception = null)`

### GameStudioComponent
Component for managing game studio data.

**Properties:**
- `StudioId`: Unique studio identifier
- `StudioName`: Studio name
- `ApiKey`: API key for studio operations

## Models

### GameEventLog
Represents a game event log entry with timestamp, event type, and data.

### GameStudio
Represents a game studio with ID, name, and configuration.

## Core Classes

### QuantumLeapManager
Singleton manager for coordinating all plugin operations.

### QuantumLeapLogger
Configurable logging system with multiple output formats.

## Examples

### Basic Usage

```csharp
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using QuantumLeap;
using UnityEngine.Networking;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class TestQL : MonoBehaviour
{
    public GameStudioComponent ql;
    public GameEventLogComponent gameEventLog;

    private void Start()
    {
        Debug.Log("Starting");
        Debug.Log($"QL: {ql.GetInstanceID()}");
        if (ql != null)
        {
            ql.OnGameStudioReceived += HandleGameStudioReceived;
            ql.GetGameStudioById("b51e5fdb-154d-4f31-a144-d0909f345408");
        }
        else
        {
            Debug.LogWarning("Can not find QL");
        }

        // Test game event logging
        if (gameEventLog != null)
        {
            gameEventLog.OnGameEventLogReceived += HandleGameEventLogReceived;
            gameEventLog.OnGameEventLogError += HandleGameEventLogError;
            
            // Example: Log a car history event
            var eventData = new GameEventData
            {
                mileage = 100,
                circuit = "hockenheimring",
                rankedRace = true,
                raceWon = true,
                podiumFinish = true
            };
            
            gameEventLog.LogGameEvent(
                eventType: "car_history",
                category: "gameplay",
                categoryNumber: 1,
                brand: "lamborghini-test",
                model: "revuelto",
                tokenId: "60",
                eventData: eventData
            );
        }
        else
        {
            Debug.LogWarning("GameEventLogComponent not found");
        }
    }

    private void HandleGameStudioReceived(GameStudio gameStudio)
    {
        Debug.Log("QL DATA: " + gameStudio.ToJson());
    }

    private void HandleGameEventLogReceived(GameEventLog gameEventLog)
    {
        Debug.Log("Game Event Log Received: " + gameEventLog.ToJson());
    }

    private void HandleGameEventLogError(string error)
    {
        Debug.LogError("Game Event Log Error: " + error);
    }
}

```

## Requirements

- Unity 2021.3 or higher
- .NET 4.x or higher
- Internet connection for API calls

## Support

- **Documentation**: [GitHub Wiki](https://github.com/pellartech/motoverse-unity-qlplugin/wiki)
- **Issues**: [GitHub Issues](https://github.com/pellartech/motoverse-unity-qlplugin/issues)
- **Email**: hello@pellartech.com

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Changelog

### Version 1.0.0
- Initial release
- Basic API integration
- Configurable logging system
- Event logging components
- Studio management 