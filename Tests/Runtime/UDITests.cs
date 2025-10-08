using NUnit.Framework;
using UnityEngine;
using QuantumLeap;
using System;

namespace QuantumLeap.Tests
{
    public class UDITests
    {
        private UDI _testUDI;
        private Provenance _testProvenance;
        private IssuedTo _testIssuedTo;

        [SetUp]
        public void Setup()
        {
            // Create test data
            _testIssuedTo = new IssuedTo("test@gmail.com");
            _testProvenance = new Provenance(
                "default",
                "lamborghini-test",
                "huracan",
                _testIssuedTo,
                DateTime.Parse("2025-10-08T04:54:59.293Z"),
                "1f891b68-1010-46b0-afc6-282c626aa662"
            );
            _testUDI = new UDI(
                "2ac44520-68be-4aa4-a909-57fe109af086",
                "1f891b68-1010-46b0-afc6-282c626aa662",
                "lamborghini-test_huracan_default",
                _testProvenance,
                "1",
                "default",
                null,
                null,
                "b51e5fdb-154d-4f31-a144-d0909f345408",
                null,
                9,
                "test@gmail.com"
            );
        }

        [Test]
        public void Test_UDI_Initialization()
        {
            // Assert
            Assert.AreEqual("2ac44520-68be-4aa4-a909-57fe109af086", _testUDI.id);
            Assert.AreEqual("1f891b68-1010-46b0-afc6-282c626aa662", _testUDI.ownerId);
            Assert.AreEqual("lamborghini-test_huracan_default", _testUDI.edition);
            Assert.AreEqual("1", _testUDI.linkedDvpId);
            Assert.AreEqual("default", _testUDI.type);
            Assert.IsNull(_testUDI.tokenId);
            Assert.IsNull(_testUDI.fusedAt);
            Assert.AreEqual("b51e5fdb-154d-4f31-a144-d0909f345408", _testUDI.studioId);
            Assert.IsNull(_testUDI.userId);
            Assert.AreEqual(9, _testUDI.sequentialId);
            Assert.AreEqual("test@gmail.com", _testUDI.email);
        }

        [Test]
        public void Test_UDI_IsFused_WhenNotFused()
        {
            // Act & Assert
            Assert.IsFalse(_testUDI.IsFused());
        }

        [Test]
        public void Test_UDI_IsFused_WhenFused()
        {
            // Arrange
            _testUDI.fusedAt = DateTime.Now;

            // Act & Assert
            Assert.IsTrue(_testUDI.IsFused());
        }

        [Test]
        public void Test_UDI_GetBrand()
        {
            // Act & Assert
            Assert.AreEqual("lamborghini-test", _testUDI.GetBrand());
        }

        [Test]
        public void Test_UDI_GetModel()
        {
            // Act & Assert
            Assert.AreEqual("huracan", _testUDI.GetModel());
        }

        [Test]
        public void Test_UDI_GetCreationDate()
        {
            // Act
            var creationDate = _testUDI.GetCreationDate();

            // Assert
            Assert.IsNotNull(creationDate);
            Assert.AreEqual(DateTime.Parse("2025-10-08T04:54:59.293Z"), creationDate);
        }

        [Test]
        public void Test_UDI_ToString()
        {
            // Act
            var result = _testUDI.ToString();

            // Assert
            Assert.IsTrue(result.Contains("2ac44520-68be-4aa4-a909-57fe109af086"));
            Assert.IsTrue(result.Contains("lamborghini-test_huracan_default"));
            Assert.IsTrue(result.Contains("1f891b68-1010-46b0-afc6-282c626aa662"));
            Assert.IsTrue(result.Contains("b51e5fdb-154d-4f31-a144-d0909f345408"));
        }

        [Test]
        public void Test_UDI_ToJson()
        {
            // Act
            var json = _testUDI.ToJson();

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("2ac44520-68be-4aa4-a909-57fe109af086"));
            Assert.IsTrue(json.Contains("lamborghini-test_huracan_default"));
        }

        [Test]
        public void Test_UDI_FromJson()
        {
            // Arrange
            var json = _testUDI.ToJson();

            // Act
            var deserializedUDI = UDI.FromJson(json);

            // Assert
            Assert.IsNotNull(deserializedUDI);
            Assert.AreEqual(_testUDI.id, deserializedUDI.id);
            Assert.AreEqual(_testUDI.ownerId, deserializedUDI.ownerId);
            Assert.AreEqual(_testUDI.edition, deserializedUDI.edition);
            Assert.AreEqual(_testUDI.studioId, deserializedUDI.studioId);
        }

        [Test]
        public void Test_UDI_FromJson_WithInvalidJson()
        {
            // Arrange
            var invalidJson = "invalid json";

            // Act
            var result = UDI.FromJson(invalidJson);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Test_Provenance_Initialization()
        {
            // Assert
            Assert.AreEqual("default", _testProvenance.type);
            Assert.AreEqual("lamborghini-test", _testProvenance.brand);
            Assert.AreEqual("huracan", _testProvenance.model);
            Assert.IsNotNull(_testProvenance.issuedTo);
            Assert.AreEqual(DateTime.Parse("2025-10-08T04:54:59.293Z"), _testProvenance.createdAt);
            Assert.AreEqual("1f891b68-1010-46b0-afc6-282c626aa662", _testProvenance.createdFor);
        }

        [Test]
        public void Test_Provenance_ToString()
        {
            // Act
            var result = _testProvenance.ToString();

            // Assert
            Assert.IsTrue(result.Contains("lamborghini-test"));
            Assert.IsTrue(result.Contains("huracan"));
            Assert.IsTrue(result.Contains("default"));
            Assert.IsTrue(result.Contains("test@gmail.com"));
        }

        [Test]
        public void Test_IssuedTo_Initialization()
        {
            // Assert
            Assert.AreEqual("test@gmail.com", _testIssuedTo.email);
        }

        [Test]
        public void Test_IssuedTo_ToString()
        {
            // Act
            var result = _testIssuedTo.ToString();

            // Assert
            Assert.IsTrue(result.Contains("test@gmail.com"));
        }

        [Test]
        public void Test_UDIResponse_Initialization()
        {
            // Arrange
            var response = new UDIResponse(true, _testUDI, "Success");

            // Assert
            Assert.IsTrue(response.success);
            Assert.AreEqual(_testUDI, response.data);
            Assert.AreEqual("Success", response.message);
            Assert.IsNull(response.error);
        }

        [Test]
        public void Test_UDIResponse_IsValid_WithValidData()
        {
            // Arrange
            var response = new UDIResponse(true, _testUDI);

            // Act & Assert
            Assert.IsTrue(response.IsValid());
        }

        [Test]
        public void Test_UDIResponse_IsValid_WithFailedResponse()
        {
            // Arrange
            var response = new UDIResponse(false, null, null, "Error occurred");

            // Act & Assert
            Assert.IsFalse(response.IsValid());
        }

        [Test]
        public void Test_UDIResponse_IsValid_WithNullData()
        {
            // Arrange
            var response = new UDIResponse(true, null);

            // Act & Assert
            Assert.IsFalse(response.IsValid());
        }

        [Test]
        public void Test_UDIResponse_ToString_WithSuccess()
        {
            // Arrange
            var response = new UDIResponse(true, _testUDI);

            // Act
            var result = response.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Success"));
            Assert.IsTrue(result.Contains("2ac44520-68be-4aa4-a909-57fe109af086"));
        }

        [Test]
        public void Test_UDIResponse_ToString_WithFailure()
        {
            // Arrange
            var response = new UDIResponse(false, null, null, "Test error");

            // Act
            var result = response.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Failed"));
            Assert.IsTrue(result.Contains("Test error"));
        }

        [Test]
        public void Test_UDIResponse_ToJson()
        {
            // Arrange
            var response = new UDIResponse(true, _testUDI, "Success");

            // Act
            var json = response.ToJson();

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("success"));
            Assert.IsTrue(json.Contains("2ac44520-68be-4aa4-a909-57fe109af086"));
        }

        [Test]
        public void Test_UDIResponse_FromJson()
        {
            // Arrange
            var response = new UDIResponse(true, _testUDI, "Success");
            var json = response.ToJson();

            // Act
            var deserializedResponse = UDIResponse.FromJson(json);

            // Assert
            Assert.IsNotNull(deserializedResponse);
            Assert.AreEqual(response.success, deserializedResponse.success);
            Assert.AreEqual(response.message, deserializedResponse.message);
        }

        [Test]
        public void Test_UDIResponse_FromJson_WithInvalidJson()
        {
            // Arrange
            var invalidJson = "invalid json";

            // Act
            var result = UDIResponse.FromJson(invalidJson);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.success);
            Assert.IsNotNull(result.error);
        }
    }
}
