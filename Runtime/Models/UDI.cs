using System;
using UnityEngine;

namespace QuantumLeap
{
    [System.Serializable]
    public class UDI
    {
        [Header("UDI Core Information")]
        public string id;
        public string ownerId;
        public string edition;
        public string linkedDvpId;
        public string type;
        public string tokenId;
        public DateTime? fusedAt;
        public string studioId;
        public string userId;
        public int sequentialId;
        public string email;

        [Header("Provenance Information")]
        public Provenance provenance;

        public UDI()
        {
            // Default constructor
        }

        public UDI(string id, string ownerId, string edition, Provenance provenance, string linkedDvpId,
                   string type, string tokenId, DateTime? fusedAt, string studioId, string userId,
                   int sequentialId, string email)
        {
            this.id = id;
            this.ownerId = ownerId;
            this.edition = edition;
            this.provenance = provenance;
            this.linkedDvpId = linkedDvpId;
            this.type = type;
            this.tokenId = tokenId;
            this.fusedAt = fusedAt;
            this.studioId = studioId;
            this.userId = userId;
            this.sequentialId = sequentialId;
            this.email = email;
        }

        /// <summary>
        /// Creates a UDI from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDI data</param>
        /// <returns>UDI object</returns>
        public static UDI FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDI>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDI from JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts UDI to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDI</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDI to JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if this UDI is fused
        /// </summary>
        /// <returns>True if fused, false otherwise</returns>
        public bool IsFused()
        {
            return fusedAt.HasValue;
        }

        /// <summary>
        /// Gets the brand from provenance
        /// </summary>
        /// <returns>Brand name or empty string</returns>
        public string GetBrand()
        {
            return provenance?.brand ?? string.Empty;
        }

        /// <summary>
        /// Gets the model from provenance
        /// </summary>
        /// <returns>Model name or empty string</returns>
        public string GetModel()
        {
            return provenance?.model ?? string.Empty;
        }

        /// <summary>
        /// Gets the creation date from provenance
        /// </summary>
        /// <returns>Creation date or null</returns>
        public DateTime? GetCreationDate()
        {
            return provenance?.createdAt;
        }
    }

    [System.Serializable]
    public class Provenance
    {
        [Header("Provenance Details")]
        public string type;
        public string brand;
        public string model;
        public IssuedTo issuedTo;
        public DateTime? createdAt;
        public string createdFor;

        public Provenance()
        {
            // Default constructor
        }

        public Provenance(string type, string brand, string model, IssuedTo issuedTo,
                         DateTime? createdAt, string createdFor)
        {
            this.type = type;
            this.brand = brand;
            this.model = model;
            this.issuedTo = issuedTo;
            this.createdAt = createdAt;
            this.createdFor = createdFor;
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"{brand} {model} ({type}) - Issued to: {issuedTo?.email}";
        }
    }

    [System.Serializable]
    public class IssuedTo
    {
        [Header("Issued To Information")]
        public string email;

        public IssuedTo()
        {
            // Default constructor
        }

        public IssuedTo(string email)
        {
            this.email = email;
        }

        /// <summary>
        /// Gets a display-friendly string representation
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"Issued to: {email}";
        }
    }

    [System.Serializable]
    public class UDIResponse
    {
        [Header("API Response")]
        public bool success;
        public UDI data;
        public string message;
        public string error;

        public UDIResponse()
        {
            // Default constructor
        }

        public UDIResponse(bool success, UDI data, string message = null, string error = null)
        {
            this.success = success;
            this.data = data;
            this.message = message;
            this.error = error;
        }

        /// <summary>
        /// Creates a UDIResponse from JSON string
        /// </summary>
        /// <param name="json">JSON string containing UDI response data</param>
        /// <returns>UDIResponse object</returns>
        public static UDIResponse FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<UDIResponse>(json);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to parse UDIResponse from JSON: {ex.Message}");
                return new UDIResponse(false, null, null, ex.Message);
            }
        }

        /// <summary>
        /// Converts UDIResponse to JSON string
        /// </summary>
        /// <returns>JSON string representation of UDIResponse</returns>
        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                QuantumLeapLogger.LogError($"Failed to convert UDIResponse to JSON: {ex.Message}");
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
    }
}
