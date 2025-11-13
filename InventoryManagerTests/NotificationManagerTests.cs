using InventoryManagerNmspc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InventoryManagerTests
{
    [TestClass]
    public class NotificationManagerTests
    {
        private NotificationManager _notificationManager;
        private List<InventoryItem> _testItems;
        private string _testThresholdFile = "test_threshold.txt";

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(_testThresholdFile))
            {
                File.Delete(_testThresholdFile);
            }

            _notificationManager = new NotificationManager();
            _testItems = new List<InventoryItem>
            {
                new InventoryItem("Молоко", 15, 80, "Молочные продукты"),
                new InventoryItem("Хлеб", 5, 40, "Выпечка"),
                new InventoryItem("Сыр", 25, 300, "Молочные продукты"),
                new InventoryItem("Яблоки", 8, 50, "Фрукты")
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(_testThresholdFile))
            {
                File.Delete(_testThresholdFile);
            }
        }

        [TestMethod]
        public void Constructor_DefaultValues_CreatesObject()
        {
            // Arrange & Act
            var manager = new NotificationManager();

            // Assert
            Assert.IsNotNull(manager);
            Assert.AreEqual(10, manager.NotificationThreshold); // значение по умолчанию
        }

        [TestMethod]
        public void NotificationThreshold_SetValidValue_UpdatesProperty()
        {
            // Arrange
            int newThreshold = 15;

            // Act
            _notificationManager.NotificationThreshold = newThreshold;

            // Assert
            Assert.AreEqual(newThreshold, _notificationManager.NotificationThreshold);
        }

        [TestMethod]
        public void NotificationThreshold_SetNegativeValue_UsesDefault()
        {
            // Arrange
            int originalThreshold = _notificationManager.NotificationThreshold;
            int negativeThreshold = -5;

            // Act
            _notificationManager.NotificationThreshold = negativeThreshold;

            // Assert - значение не должно измениться на отрицательное
            Assert.AreEqual(originalThreshold, _notificationManager.NotificationThreshold);
        }

        [TestMethod]
        public void NotificationThreshold_SetZeroValue_UpdatesProperty()
        {
            // Arrange
            int zeroThreshold = 0;

            // Act
            _notificationManager.NotificationThreshold = zeroThreshold;

            // Assert
            Assert.AreEqual(zeroThreshold, _notificationManager.NotificationThreshold);
        }

        [TestMethod]
        public void CheckInventoryLevels_ItemsAboveThreshold_NoNotifications()
        {
            // Arrange
            _notificationManager.NotificationThreshold = 5;
            var items = new List<InventoryItem>
            {
                new InventoryItem("Товар1", 10, 100, "Категория1"),
                new InventoryItem("Товар2", 6, 200, "Категория2")
            };

            // Act
            var notifications = _notificationManager.CheckInventoryLevels(items);

            // Assert
            Assert.AreEqual(0, notifications.Count);
        }

        [TestMethod]
        public void CheckInventoryLevels_ItemsBelowThreshold_GeneratesNotifications()
        {
            // Arrange
            _notificationManager.NotificationThreshold = 10;
            var items = new List<InventoryItem>
            {
                new InventoryItem("Молоко", 5, 80, "Молочные"),    // ниже порога
                new InventoryItem("Хлеб", 15, 40, "Выпечка"),      // выше порога
                new InventoryItem("Сыр", 3, 300, "Молочные")       // ниже порога
            };

            // Act
            var notifications = _notificationManager.CheckInventoryLevels(items);

            // Assert
            Assert.AreEqual(2, notifications.Count);
            Assert.IsTrue(notifications.Any(n => n.Contains("Молоко")));
            Assert.IsTrue(notifications.Any(n => n.Contains("Сыр")));
            Assert.IsFalse(notifications.Any(n => n.Contains("Хлеб")));
        }

        [TestMethod]
        public void CheckInventoryLevels_ItemsAtThreshold_NoNotifications()
        {
            // Arrange
            _notificationManager.NotificationThreshold = 10;
            var items = new List<InventoryItem>
            {
                new InventoryItem("Товар1", 10, 100, "Категория1"), // точно на пороге
                new InventoryItem("Товар2", 10, 200, "Категория2")  // точно на пороге
            };

            // Act
            var notifications = _notificationManager.CheckInventoryLevels(items);

            // Assert
            Assert.AreEqual(0, notifications.Count);
        }

        [TestMethod]
        public void CheckInventoryLevels_EmptyList_NoNotifications()
        {
            // Arrange
            var emptyList = new List<InventoryItem>();

            // Act
            var notifications = _notificationManager.CheckInventoryLevels(emptyList);

            // Assert
            Assert.AreEqual(0, notifications.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckInventoryLevels_NullList_NoNotifications()
        {
            // Arrange
            List<InventoryItem> nullList = null;

            // Act
            var notifications = _notificationManager.CheckInventoryLevels(nullList);
        }
    }
}