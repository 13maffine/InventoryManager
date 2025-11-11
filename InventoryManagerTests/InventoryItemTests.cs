using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InventoryManagerNmspc;

namespace InventoryManagerTests
{
    [TestClass]
    public class InventoryItemTests
    {
        [TestMethod]
        public void Constructor_ValidParameters_CreatesObject() // тест для проверки конструктора с 4 параметрами 
        {
            string name = "Булка хлеба";
            int quantity = 100;
            int price = 150;
            string category = "Еда";

            InventoryItem item = new InventoryItem(name, quantity, price, category);

            Assert.IsNotNull(item);
            Assert.AreEqual(name, item.Name);
            Assert.AreEqual(quantity, item.Quantity);
            Assert.AreEqual(price, item.Price);
            Assert.AreEqual(category, item.Category);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NegativeQuantity_ThrowsException() // тест для проверки конструктора с 4 параметрами, добавление товара с отрицательным количеством товара 
        {
            string name = "Булка хлеба";
            int quantity = -100;
            int price = 150;
            string category = "Еда";

            InventoryItem item = new InventoryItem(name, quantity, price, category);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NegativePrice_ThrowsException() // тест для проверки конструктора с 4 параметрами, добавление товара с отрицательной ценой 
        {
            string name = "Булка хлеба";
            int quantity = 100;
            int price = -150;
            string category = "Еда";

            InventoryItem item = new InventoryItem(name, quantity, price, category);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullName_ThrowsException() // тест для проверки конструктора с 4 параметрами, добавление товара с null именем 
        {
            string name = null;
            int quantity = 100;
            int price = -150;
            string category = "Еда";

            InventoryItem item = new InventoryItem(name, quantity, price, category);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullCategory_ThrowsException() // тест для проверки конструктора с 4 параметрами, добавление товара с null категорией
        {
            string name = "Булка хлеба";
            int quantity = 100;
            int price = -150;
            string category = null;

            InventoryItem item = new InventoryItem(name, quantity, price, category);
        }
    }
}
