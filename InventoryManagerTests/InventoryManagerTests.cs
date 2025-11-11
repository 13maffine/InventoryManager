using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;
using InventoryManagerNmspc;

namespace InventoryManagerTests
{
    [TestClass]
    public class InventoryManagerTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            File.WriteAllLines("inventory.txt", new string[] { "" });
        }

        [TestMethod]
        public void Constructor_ZeroParametrs_CreateObject()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();

            Assert.IsNotNull(manager);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddItem_NullItem_ThrowsException()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();

            manager.AddItem(null);
        }

        [TestMethod]
        public void AddItem_ValidItem()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            InventoryItem item = new InventoryItem("Булка хлеба", 100, 100, "Еда");
            int count_before = manager.Items.Count;

            manager.AddItem(item);

            Assert.AreEqual(count_before + 1, manager.Items.Count);
            Assert.AreEqual(manager.Items.Last().Name, "Булка хлеба");
            Assert.AreEqual(manager.Items.Last().Quantity, 100);
            Assert.AreEqual(manager.Items.Last().Price, 100);
            Assert.AreEqual(manager.Items.Last().Category, "Еда");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveItem_NullItem_ThrowsException()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();

            manager.RemoveItem(null);
        }

        [TestMethod]
        public void RemoveItem_ValidItem()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            InventoryItem item = new InventoryItem("Булка хлеба", 200, 300, "Еда");
            int count_before = manager.Items.Count;
            manager.AddItem(item);

            manager.RemoveItem(item);

            Assert.AreEqual(count_before, manager.Items.Count);
            Assert.AreEqual(-1, manager.Items.FindIndex(x => x.Name == item.Name && x.Price == item.Price && x.Quantity == item.Quantity && x.Category == item.Category));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateItemQuantity_NullItem_ThrowsException()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();

            manager.UpdateItemQuantity(null, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateItemQuantity_NegativeQuantity_ThrowsException()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            InventoryItem item = new InventoryItem("Булка хлеба", 100, 100, "Еда");
            manager.AddItem(item);

            manager.UpdateItemQuantity(item, -100);
        }

        [TestMethod]
        public void UpdateItemQuantity_ValidData()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            int oldQuantity = 300;
            InventoryItem item = new InventoryItem("Булка хлеба", oldQuantity, 100, "Еда");
            manager.AddItem(item);
            int newQuantity = 200;

            manager.UpdateItemQuantity(item, newQuantity);

            Assert.IsTrue(manager.Items.FindIndex(x => x.Name == item.Name && x.Price == item.Price && x.Quantity == newQuantity && x.Category == item.Category) >= 0);
            Assert.AreEqual(-1, manager.Items.FindIndex(x => x.Name == item.Name && x.Price == item.Price && x.Quantity == oldQuantity && x.Category == item.Category));
        }

        [TestMethod]
        public void AddItem_DuplicateItem_ShouldNotAdd()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            var item1 = new InventoryItem("Булка хлеба", 1, 10, "Еда");
            var item2 = new InventoryItem("Булка хлеба", 5, 15, "Еда");

            manager.AddItem(item1);
            int countBefore = manager.Items.Count;
            manager.AddItem(item2);
            int countAfter = manager.Items.Count;

            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public void RemoveItem_NotExistingItem_ShouldNotThrow()
        {
            InventoryManagerNmspc.InventoryManager manager = new InventoryManagerNmspc.InventoryManager();
            var item = new InventoryItem("Не существует", 1, 10, "Еда");
            int countBefore = manager.Items.Count;

            manager.RemoveItem(item);
            int countAfter = manager.Items.Count;

            Assert.AreEqual(countBefore, countAfter);
        }
    }
}
