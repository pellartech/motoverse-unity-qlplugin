using System;
using System.Collections;
using UnityEngine;

namespace QuantumLeap
{
    public class UDIFuseComponent : QuantumLeapComponent
    {
        private UDIFuseResponse _currentFuseResponse = null;

        public UDIFuseResponse CurrentFuseResponse => _currentFuseResponse;

        public event Action<UDIFuseResponse> OnUDIFuseReceived;
        public event Action<string> OnUDIFuseError;

        public override void Initialize()
        {
            base.Initialize();

            OnComponentInitialized += OnUDIFuseInitialized;

            OnDataReceived += OnUDIFuseDataReceived;

            OnErrorOccurred += OnUDIFuseDataError;
        }

        /// <summary>
        /// Fuses a UDI with NFT details
        /// </summary>
        /// <param name="udiId">UDI ID to fuse</param>
        /// <param name="tokenId">Token ID</param>
        /// <param name="contractAddress">NFT contract address</param>
        /// <param name="ownerAddress">NFT owner address</param>
        /// <param name="rarity">NFT rarity</param>
        /// <param name="attributes">NFT attributes</param>
        /// <returns>Coroutine for the API call</returns>
        public Coroutine FuseUDI(string udiId, string tokenId, string contractAddress, 
                                string ownerAddress, string rarity = "common", 
                                UDIAttribute[] attributes = null)
        {
            if (string.IsNullOrEmpty(udiId))
            {
                OnUDIFuseError?.Invoke("UDI ID is required");
                return null;
            }

            if (string.IsNullOrEmpty(tokenId))
            {
                OnUDIFuseError?.Invoke("Token ID is required");
                return null;
            }

            if (string.IsNullOrEmpty(contractAddress))
            {
                OnUDIFuseError?.Invoke("Contract address is required");
                return null;
            }

            if (string.IsNullOrEmpty(ownerAddress))
            {
                OnUDIFuseError?.Invoke("Owner address is required");
                return null;
            }

            if (!IsValidEthereumAddress(contractAddress))
            {
                OnUDIFuseError?.Invoke("Invalid contract address format");
                return null;
            }

            if (!IsValidEthereumAddress(ownerAddress))
            {
                OnUDIFuseError?.Invoke("Invalid owner address format");
                return null;
            }

            var fuseRequest = new UDIFuseRequest
            {
                udiId = udiId,
                tokenId = tokenId,
                nftDetails = new UDINFTDetails
                {
                    contractAddress = contractAddress,
                    ownerAddress = ownerAddress,
                    metadata = new UDIMetadata
                    {
                        rarity = rarity ?? "common",
                        attributes = attributes ?? new UDIAttribute[0]
                    }
                }
            };

            string jsonData = fuseRequest.ToJson();
            string endpoint = $"{ApiUrl}/udis/fuse";

            return StartCoroutine(PostDataCoroutine(endpoint, jsonData));
        }

        /// <summary>
        /// Fuses a UDI with NFT details using UDIFuseRequest object
        /// </summary>
        /// <param name="fuseRequest">UDIFuseRequest object containing all fuse data</param>
        /// <returns>Coroutine for the API call</returns>
        public Coroutine FuseUDI(UDIFuseRequest fuseRequest)
        {
            if (fuseRequest == null)
            {
                OnUDIFuseError?.Invoke("Fuse request is required");
                return null;
            }

            if (string.IsNullOrEmpty(fuseRequest.udiId))
            {
                OnUDIFuseError?.Invoke("UDI ID is required");
                return null;
            }

            if (string.IsNullOrEmpty(fuseRequest.tokenId))
            {
                OnUDIFuseError?.Invoke("Token ID is required");
                return null;
            }

            if (fuseRequest.nftDetails == null)
            {
                OnUDIFuseError?.Invoke("NFT details are required");
                return null;
            }

            if (string.IsNullOrEmpty(fuseRequest.nftDetails.contractAddress))
            {
                OnUDIFuseError?.Invoke("Contract address is required");
                return null;
            }

            if (string.IsNullOrEmpty(fuseRequest.nftDetails.ownerAddress))
            {
                OnUDIFuseError?.Invoke("Owner address is required");
                return null;
            }

            if (!IsValidEthereumAddress(fuseRequest.nftDetails.contractAddress))
            {
                OnUDIFuseError?.Invoke("Invalid contract address format");
                return null;
            }

            if (!IsUDIFuseRequestValid(fuseRequest))
            {
                OnUDIFuseError?.Invoke("Invalid fuse request data");
                return null;
            }

            string jsonData = fuseRequest.ToJson();
            string endpoint = $"{ApiUrl}/udis/fuse";

            return StartCoroutine(PostDataCoroutine(endpoint, jsonData));
        }

        /// <summary>
        /// Validates Ethereum address format
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        private bool IsValidEthereumAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;

            // Basic Ethereum address validation (0x + 40 hex characters)
            return address.StartsWith("0x") && address.Length == 42 && IsHexString(address.Substring(2));
        }

        /// <summary>
        /// Checks if string contains only hexadecimal characters
        /// </summary>
        /// <param name="hex">String to check</param>
        /// <returns>True if hex string, false otherwise</returns>
        private bool IsHexString(string hex)
        {
            foreach (char c in hex)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates UDIFuseRequest data
        /// </summary>
        /// <param name="request">Request to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        private bool IsUDIFuseRequestValid(UDIFuseRequest request)
        {
            if (request.nftDetails?.metadata == null)
                return false;

            if (string.IsNullOrEmpty(request.nftDetails.metadata.rarity))
                return false;

            return true;
        }

        private void OnUDIFuseDataReceived(string data)
        {
            try
            {
                // Try to parse as UDIFuseResponse
                var fuseResponse = UDIFuseResponse.FromJson(data ?? "");
                
                if (fuseResponse != null && fuseResponse.success)
                {
                    _currentFuseResponse = fuseResponse;
                    OnUDIFuseReceived?.Invoke(fuseResponse);
                    QuantumLeapLogger.Log($"UDI Fuse successful: {fuseResponse.data?.udiId}");
                }
                else
                {
                    QuantumLeapLogger.LogError("Failed to parse UDI Fuse response data");
                    OnUDIFuseError?.Invoke("Failed to parse UDI Fuse data");
                }
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Error parsing UDI Fuse data: {ex.Message}");
                OnUDIFuseError?.Invoke($"Error parsing UDI Fuse data: {ex.Message}");
            }
        }

        private void OnUDIFuseInitialized()
        {
            OnDataReceived -= OnUDIFuseDataReceived;
            OnErrorOccurred -= OnUDIFuseDataError;
        }

        private void OnUDIFuseDataError(string error)
        {
            QuantumLeapLogger.LogError($"UDIFuseComponent: OnUDIFuseError: {error}");
            OnUDIFuseError?.Invoke(error);
        }

        /// <summary>
        /// Clears the current fuse response data
        /// </summary>
        public void ClearFuseResponse()
        {
            _currentFuseResponse = null;
        }

        /// <summary>
        /// Checks if a fuse response is currently loaded
        /// </summary>
        /// <returns>True if fuse response is loaded, false otherwise</returns>
        public bool HasFuseResponse()
        {
            return _currentFuseResponse != null;
        }

        /// <summary>
        /// Gets the current fuse response UDI ID
        /// </summary>
        /// <returns>UDI ID or null if no response</returns>
        public string GetCurrentFuseUDIId()
        {
            return _currentFuseResponse?.data?.udiId;
        }

        /// <summary>
        /// Gets the current fuse response token ID
        /// </summary>
        /// <returns>Token ID or null if no response</returns>
        public string GetCurrentFuseTokenId()
        {
            return _currentFuseResponse?.data?.tokenId;
        }

        /// <summary>
        /// Gets the current fuse response type
        /// </summary>
        /// <returns>Type or null if no response</returns>
        public string GetCurrentFuseType()
        {
            return _currentFuseResponse?.data?.type;
        }

        /// <summary>
        /// Gets the current fuse response fused date
        /// </summary>
        /// <returns>Fused date string or null if no response</returns>
        public string GetCurrentFuseDate()
        {
            return _currentFuseResponse?.data?.fusedAt;
        }

        /// <summary>
        /// Gets the current fuse response fusion ID
        /// </summary>
        /// <returns>Fusion ID or null if no response</returns>
        public string GetCurrentFusionId()
        {
            return _currentFuseResponse?.data?.fusionId;
        }

        /// <summary>
        /// Gets the current fuse response fused date as DateTime
        /// </summary>
        /// <returns>DateTime or null if no response or parsing fails</returns>
        public DateTime? GetCurrentFuseDateTime()
        {
            return _currentFuseResponse?.data?.GetFusedDateTime();
        }

        /// <summary>
        /// Checks if the current fuse response indicates a fused UDI
        /// </summary>
        /// <returns>True if fused, false otherwise</returns>
        public bool IsCurrentUDIFused()
        {
            return _currentFuseResponse?.data?.IsFused() ?? false;
        }

        /// <summary>
        /// Gets the current fuse response type display name
        /// </summary>
        /// <returns>Formatted type string or empty string if no response</returns>
        public string GetCurrentFuseTypeDisplayName()
        {
            return _currentFuseResponse?.data?.GetTypeDisplayName() ?? string.Empty;
        }

        /// <summary>
        /// Gets the current fuse response summary
        /// </summary>
        /// <returns>Formatted summary string or empty string if no response</returns>
        public string GetCurrentFuseSummary()
        {
            if (_currentFuseResponse?.data == null)
                return string.Empty;

            return _currentFuseResponse.data.ToString();
        }
    }

    /// <summary>
    /// Request model for fusing UDI
    /// </summary>
    [System.Serializable]
    public class UDIFuseRequest
    {
        [Header("UDI Fuse Information")]
        public string udiId;
        public string tokenId;
        public UDINFTDetails nftDetails;

        public UDIFuseRequest()
        {
            // Default constructor
        }

        public UDIFuseRequest(string udiId, string tokenId, UDINFTDetails nftDetails)
        {
            this.udiId = udiId;
            this.tokenId = tokenId;
            this.nftDetails = nftDetails;
        }

        /// <summary>
        /// Creates a UDIFuseRequest from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDI fuse request data</param>
        /// <returns>UDIFuseRequest object</returns>
        public static UDIFuseRequest FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDIFuseRequest>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDIFuseRequest from JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts UDIFuseRequest to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDIFuseRequest</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDIFuseRequest to JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates if the UDIFuseRequest has all required fields
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(udiId) &&
                   !string.IsNullOrEmpty(tokenId) &&
                   nftDetails != null &&
                   !string.IsNullOrEmpty(nftDetails.contractAddress) &&
                   !string.IsNullOrEmpty(nftDetails.ownerAddress) &&
                   nftDetails.metadata != null &&
                   !string.IsNullOrEmpty(nftDetails.metadata.rarity);
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"UDI Fuse Request - UDI ID: {udiId}, Token ID: {tokenId}, " +
                   $"Contract: {nftDetails?.contractAddress}, Rarity: {nftDetails?.metadata?.rarity}";
        }
    }

    /// <summary>
    /// NFT details for UDI fuse
    /// </summary>
    [System.Serializable]
    public class UDINFTDetails
    {
        [Header("NFT Information")]
        public string contractAddress;
        public string ownerAddress;
        public UDIMetadata metadata;

        public UDINFTDetails()
        {
            // Default constructor
        }

        public UDINFTDetails(string contractAddress, string ownerAddress, UDIMetadata metadata)
        {
            this.contractAddress = contractAddress;
            this.ownerAddress = ownerAddress;
            this.metadata = metadata;
        }
    }

    /// <summary>
    /// NFT metadata for UDI fuse
    /// </summary>
    [System.Serializable]
    public class UDIMetadata
    {
        [Header("Metadata Information")]
        public string rarity;
        public UDIAttribute[] attributes;

        public UDIMetadata()
        {
            // Default constructor
        }

        public UDIMetadata(string rarity, UDIAttribute[] attributes = null)
        {
            this.rarity = rarity;
            this.attributes = attributes ?? new UDIAttribute[0];
        }
    }

    /// <summary>
    /// NFT attribute for UDI fuse
    /// </summary>
    [System.Serializable]
    public class UDIAttribute
    {
        [Header("Attribute Information")]
        public string trait_type;
        public string value;

        public UDIAttribute()
        {
            // Default constructor
        }

        public UDIAttribute(string traitType, string value)
        {
            this.trait_type = traitType;
            this.value = value;
        }
    }

    /// <summary>
    /// Response model for UDI fuse
    /// </summary>
    [System.Serializable]
    public class UDIFuseResponse
    {
        [Header("API Response")]
        public bool success;
        public UDIFuseData data;
        public string message;
        public string error;

        public UDIFuseResponse()
        {
            // Default constructor
        }

        public UDIFuseResponse(bool success, UDIFuseData data, string message = null, string error = null)
        {
            this.success = success;
            this.data = data;
            this.message = message;
            this.error = error;
        }

        /// <summary>
        /// Creates a UDIFuseResponse from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDI fuse response data</param>
        /// <returns>UDIFuseResponse object</returns>
        public static UDIFuseResponse FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDIFuseResponse>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDIFuseResponse from JSON: {ex.Message}");
                return new UDIFuseResponse(false, null, null, ex.Message);
            }
        }

        /// <summary>
        /// Converts UDIFuseResponse to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDIFuseResponse</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDIFuseResponse to JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if the response is successful and has valid data
        /// </summary>
        /// <returns>True if successful and valid, false otherwise</returns>
        public bool IsValid()
        {
            return success && data != null;
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            if (success)
            {
                return $"UDI Fuse Response: Success - {data?.ToString()}";
            }
            else
            {
                return $"UDI Fuse Response: Failed - {error ?? message}";
            }
        }
    }

    /// <summary>
    /// UDI fuse data response
    /// </summary>
    [System.Serializable]
    public class UDIFuseData
    {
        [Header("Fuse Data")]
        public string udiId;
        public string tokenId;
        public string type;
        public string fusedAt;
        public string fusionId;

        public UDIFuseData()
        {
            // Default constructor
        }

        public UDIFuseData(string udiId, string tokenId, string type, string fusedAt, string fusionId)
        {
            this.udiId = udiId;
            this.tokenId = tokenId;
            this.type = type;
            this.fusedAt = fusedAt;
            this.fusionId = fusionId;
        }

        /// <summary>
        /// Gets the fused date as DateTime
        /// </summary>
        /// <returns>DateTime or null if parsing fails</returns>
        public DateTime? GetFusedDateTime()
        {
            if (string.IsNullOrEmpty(fusedAt))
                return null;

            try
            {
                return DateTime.Parse(fusedAt);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse fusedAt date: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if the UDI is fused
        /// </summary>
        /// <returns>True if fused, false otherwise</returns>
        public bool IsFused()
        {
            return !string.IsNullOrEmpty(fusedAt) && !string.IsNullOrEmpty(fusionId);
        }

        /// <summary>
        /// Gets the fusion type display name
        /// </summary>
        /// <returns>Formatted type string</returns>
        public string GetTypeDisplayName()
        {
            if (string.IsNullOrEmpty(type))
                return "Unknown";

            return type.ToLower() switch
            {
                "premium" => "Premium",
                "standard" => "Standard",
                "basic" => "Basic",
                _ => char.ToUpper(type[0]) + type.Substring(1).ToLower()
            };
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"UDI ID: {udiId}, Token ID: {tokenId}, Type: {GetTypeDisplayName()}, " +
                   $"Fused At: {fusedAt}, Fusion ID: {fusionId}";
        }
    }
}
