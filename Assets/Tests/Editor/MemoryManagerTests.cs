using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MemoryManagerTests
    {
        [Test]
        public void FormatBytes_ZeroBytes_ReturnsZeroB()
        {
            // Arrange
            long byteCount = 0;

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("0 B", result);
        }

        [Test]
        public void FormatBytes_1024Bytes_Returns1KB()
        {
            // Arrange
            long byteCount = 1024;

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1 KB", result);
        }

        [Test]
        public void FormatBytes_1048576Bytes_Returns1MB()
        {
            // Arrange
            long byteCount = 1048576; // 1024 * 1024

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1 MB", result);
        }

        [Test]
        public void FormatBytes_1073741824Bytes_Returns1GB()
        {
            // Arrange
            long byteCount = 1073741824; // 1024 * 1024 * 1024

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1 GB", result);
        }

        [Test]
        public void FormatBytes_NegativeValue_ReturnsNegativeFormatted()
        {
            // Arrange
            long byteCount = -1024;

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("-1 KB", result);
        }

        [Test]
        public void FormatBytes_SmallValue_ReturnsBytes()
        {
            // Arrange
            long byteCount = 512;

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("512 B", result);
        }

        [Test]
        public void FormatBytes_LargeValue_ReturnsTB()
        {
            // Arrange
            long byteCount = 1099511627776; // 1024^4 = 1 TB

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1 TB", result);
        }

        [Test]
        public void FormatBytes_DecimalValue_RoundsToOneDecimalPlace()
        {
            // Arrange
            long byteCount = 1536; // 1.5 KB

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1.5 KB", result);
        }

        [Test]
        public void FormatBytes_VeryLargeValue_ReturnsPB()
        {
            // Arrange
            long byteCount = 1125899906842624; // 1024^5 = 1 PB

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("1 PB", result);
        }

        [Test]
        public void FormatBytes_ComplexValue_FormatsCorrectly()
        {
            // Arrange
            long byteCount = 2684354560; // ~2.5 GB

            // Act
            string result = MemoryManager.FormatBytes(byteCount);

            // Assert
            Assert.AreEqual("2.5 GB", result);
        }
    }
}
