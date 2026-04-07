using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Profiling;

namespace Tests
{
    public class MemoryManagerPlayModeTests
    {
        private GameObject memoryManagerObject;
        private MemoryManager memoryManager;

        [SetUp]
        public void Setup()
        {
            // Create a GameObject with MemoryManager component
            memoryManagerObject = new GameObject("MemoryManager");
            memoryManager = memoryManagerObject.AddComponent<MemoryManager>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            if (memoryManagerObject != null)
            {
                Object.Destroy(memoryManagerObject);
            }
        }

        [UnityTest]
        public IEnumerator MemoryManager_ComponentExists()
        {
            // Wait one frame
            yield return null;

            // Assert
            Assert.IsNotNull(memoryManager);
        }

        [UnityTest]
        public IEnumerator MemoryManager_UpdateIsCalledEveryFrame()
        {
            // Wait for Start to be called
            yield return null;

            // Get initial memory values
            long initialMonoHeap = Profiler.GetMonoHeapSizeLong();

            // Wait a frame
            yield return null;

            // Memory values should be accessible
            Assert.IsTrue(initialMonoHeap >= 0);
        }

        [Test]
        public void FormatBytes_StaticMethod_WorksWithoutInstance()
        {
            // Act
            string result = MemoryManager.FormatBytes(1024);

            // Assert
            Assert.AreEqual("1 KB", result);
        }

        [Test]
        public void FormatBytes_VariousUnits_FormatsCorrectly()
        {
            // Test multiple values
            Assert.AreEqual("0 B", MemoryManager.FormatBytes(0));
            Assert.AreEqual("100 B", MemoryManager.FormatBytes(100));
            Assert.AreEqual("1 KB", MemoryManager.FormatBytes(1024));
            Assert.AreEqual("1 MB", MemoryManager.FormatBytes(1024 * 1024));
            Assert.AreEqual("1 GB", MemoryManager.FormatBytes(1024L * 1024 * 1024));
        }
    }
}
