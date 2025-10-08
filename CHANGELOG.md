# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial plugin structure
- API integration components
- Logging system
- Event tracking
- Studio management

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [1.0.0] - 2025-08-06

### Added
- Initial release of Motoverse Quantum Leap Plugin
- QuantumLeapComponent for API integration
- GameEventLogComponent for event logging
- GameStudioComponent for studio management
- QuantumLeapManager singleton for coordination
- QuantumLeapLogger for configurable logging
- GameEventLog and GameStudio data models
- Async/await support for network operations
- Unity component integration
- Configurable logging levels and outputs
- HTTP request handling with retry logic
- Error handling and exception management

# [1.1.0] - 2025-10-08
### Added
- UDI (Unique Digital Item) model and management system
- UDIComponent for UDI creation and retrieval operations
- UDIStats model for statistical data management
- UDIStatsComponent for statistics API integration
- UDIFuseComponent for UDI fusion operations with NFT integration
- Comprehensive documentation and usage examples

#### UDI System Components
- **UDI Model**: Complete data structure with Provenance, IssuedTo, and validation
- **UDIComponent**: API integration for UDI creation (`generateToken2049Udis`) and retrieval (`getUdisById`)
- **UDICreateRequest**: Request model for UDI creation with email, brand, model parameters
- **UDIResponse**: Response wrapper with success/error handling

#### UDI Statistics System
- **UDIStats Model**: Statistical data with win rates, podium percentages, performance ratings
- **UDIStatsComponent**: API integration for statistics retrieval (`getDefaultUdisStats`)
- **UDIStatsResponse**: Response wrapper for statistics data
- **Performance Analysis**: Win percentage, podium percentage, performance rating algorithms
- **Driver Analysis**: Success and activity assessment methods

#### UDI Fusion System
- **UDIFuseComponent**: NFT fusion operations (`generateUdisFuse`)
- **UDIFuseRequest**: Request model with NFT details, contract addresses, metadata
- **UDIFuseResponse**: Response with fusion results, timestamps, fusion IDs
- **NFT Integration**: Contract address validation, metadata handling, attribute management
- **Ethereum Validation**: Secure address format validation and hex string checking

#### API Integration
- **UDI Creation**: `POST /udis/default/issue/token2049/{email}/{brand}/{model}`
- **UDI Retrieval**: `GET /udis/{udiId}`
- **UDI Statistics**: `GET /udis/default/{brand}/{model}/{sequentialId}/stats`
- **UDI Fusion**: `POST /udis/fuse`
- **Authentication**: API key-based authentication for all endpoints
- **Error Handling**: Comprehensive error response handling and user feedback
- **Request Validation**: Pre-flight validation for all API parameters
- **Response Parsing**: Automatic JSON parsing with type safety and error recovery
