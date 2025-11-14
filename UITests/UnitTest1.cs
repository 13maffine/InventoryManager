using FlaUI.UIA3;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace UITests
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private string _filePath = @"inventory.txt";

        [TestInitialize]
        public void TestInitialize()
        {
            _app = Application.Launch(@"..\..\..\InventoryManager\bin\Debug\InventoryManager.exe");
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);

            if (File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, string.Empty);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _automation?.Dispose();
            _app?.Close();

            if (File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, string.Empty);
            }
        }

        private void AddItem(string name, string quantity, string price, string category)
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();

            // Явная очистка и ввод с задержками
            //nameTextBox?.Click();
            //System.Threading.Thread.Sleep(100);
            //nameTextBox?.Enter("");
            //System.Threading.Thread.Sleep(50);
            nameTextBox?.Enter(name);
            System.Threading.Thread.Sleep(100);

            //quantityTextBox?.Click();
            //System.Threading.Thread.Sleep(100);
            //quantityTextBox?.Enter("");
            //System.Threading.Thread.Sleep(50);
            quantityTextBox?.Enter(quantity);
            System.Threading.Thread.Sleep(100);

            //priceTextBox?.Click();
            //System.Threading.Thread.Sleep(100);
            //priceTextBox?.Enter("");
            //System.Threading.Thread.Sleep(50);
            priceTextBox?.Enter(price);
            System.Threading.Thread.Sleep(100);

            //categoryTextBox?.Click();
            //System.Threading.Thread.Sleep(100);
            //categoryTextBox?.Enter("");
            //System.Threading.Thread.Sleep(50);
            categoryTextBox?.Enter(category);
            System.Threading.Thread.Sleep(100);

            addButton?.Click();
            System.Threading.Thread.Sleep(200); // Дать время для обработки
        }

        private int GetItemsCount()
        {
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            return itemsListBox?.Items.Length ?? 0;
        }

        private void SelectFirstItem()
        {
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            if (itemsListBox != null && itemsListBox?.Items?.Length > 0)
            {
                itemsListBox?.Items?.First()?.Click();
            }
        }
        private ListBox GetItemsListBox()
        {
            return _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
        }

        private string GetFirstItemText()
        {
            var itemsListBox = GetItemsListBox();
            return itemsListBox?.Items.First()?.Text ?? string.Empty;
        }
        private string GetLastItemText()
        {
            var itemsListBox = GetItemsListBox();
            return itemsListBox?.Items.Last()?.Text ?? string.Empty;
        }

        private bool WaitForNotification(int timeoutMs)
        {
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
                if (messageBox != null)
                {
                    var okButton = messageBox.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
                    okButton?.Click();
                    return true;
                }
                System.Threading.Thread.Sleep(500);
            }
            return false;
        }

        private int CountNotificationsInPeriod(int periodMs)
        {
            DateTime startTime = DateTime.Now;
            int count = 0;

            while ((DateTime.Now - startTime).TotalMilliseconds < periodMs)
            {
                var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
                if (messageBox != null)
                {
                    count++;
                    var okButton = messageBox.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
                    okButton?.Click();
                }
                System.Threading.Thread.Sleep(500);
            }

            return count;
        }

        [TestMethod]
        public void TC001_AddItemWithValidData()
        {
            // Arrange
            string name = "Булка хлеба";
            string quantity = "100";
            string price = "150";
            string category = "Еда";

            // Act
            AddItem(name, quantity, price, category);

            // Assert
            var itemsListBox = GetItemsListBox();
            var lastItem = itemsListBox?.Items.Last()?.Text;

            Assert.IsNotNull(lastItem);
            Assert.IsTrue(lastItem.Contains(name));
            Assert.IsTrue(lastItem.Contains(quantity));
            Assert.IsTrue(lastItem.Contains(price));
            Assert.IsTrue(lastItem.Contains(category));
        }

        [TestMethod]
        public void TC002_RemoveExistingItem()
        {


            // Arrange
            AddItem("Тестовый товар", "50", "200", "Тест");
            int itemsCountBefore = GetItemsCount();
            string firstItemBefore = GetFirstItemText();

            // Act
            SelectFirstItem();
            var removeButton = _mainWindow.FindFirstDescendant(x => x.ByName("removeItemButton"))?.AsButton();
            removeButton.Click();

            // Assert
            int itemsCountAfter = GetItemsCount();
            Assert.AreEqual(itemsCountBefore - 1, itemsCountAfter);
            if (itemsCountAfter > 0)
            {
                string firstItemAfter = GetFirstItemText();
                Assert.AreNotEqual(firstItemBefore, firstItemAfter);
            }
        }

        [TestMethod]
        public void TC003_UpdateItemQuantity()
        {
            // Arrange
            AddItem("Товар для обновления", "100", "300", "Тест");
            SelectFirstItem();

            string oldQuantity = "100";
            string newQuantity = "150";

            // Act
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();

            quantityTextBox?.Enter(newQuantity);
            updateButton?.Click();

            // Assert 
            string updatedItemText = GetLastItemText();

            Assert.IsTrue(updatedItemText.Contains(newQuantity));
            Assert.IsFalse(updatedItemText.Contains(oldQuantity));
        }
        [TestMethod]
        public void TC004_AddItemWithEmptyFields()
        {
            // Arrange
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            int itemsCountBefore = itemsListBox.Items.Length;

            // Act 
            addButton.Click();

            // Assert 
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2")).AsButton();
            okButton?.Click();

            // Assert
            int itemsCountAfter = itemsListBox.Items.Length;

            Assert.AreEqual(itemsCountBefore, itemsCountAfter);
            Assert.AreEqual("Заполните все поля!", message);
        }

        [TestMethod]
        public void TC005_AddItemWithInvalidQuantityFormat()
        {
            // Arrange
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            int itemsCountBefore = itemsListBox.Items.Length;

            // Act
            nameTextBox.Enter("Тестовый товар");
            quantityTextBox.Enter("не число");
            priceTextBox.Enter("100");
            categoryTextBox.Enter("Тест");
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2")).AsButton();
            okButton?.Click();

            // Assert
            int itemsCountAfter = itemsListBox.Items.Length;

            Assert.AreEqual("Неверный формат количества или цены!", message);
            Assert.AreEqual(itemsCountBefore, itemsCountAfter);
        }

        [TestMethod]
        public void TC006_AddItemWithInvalidPriceFormat()
        {
            // Arrange
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            int itemsCountBefore = itemsListBox.Items.Length;

            // Act 
            nameTextBox.Enter("Тестовый товар");
            quantityTextBox.Enter("10");
            priceTextBox.Enter("сто");
            categoryTextBox.Enter("Тест");
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2")).AsButton();
            okButton?.Click();

            // Assert
            int itemsCountAfter = itemsListBox.Items.Length;

            Assert.AreEqual("Неверный формат количества или цены!", message);
            Assert.AreEqual(itemsCountBefore, itemsCountAfter);
        }

        [TestMethod]
        public void TC007_RemoveItemWithoutSelection()
        {
            // Arrange
            var removeButton = _mainWindow.FindFirstDescendant(x => x.ByName("removeItemButton"))?.AsButton();

            // Act
            removeButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            Assert.AreEqual("Выберите товар для удаления!", message);
        }

        [TestMethod]
        public void TC008_UpdateQuantityWithoutSelection()
        {
            // Arrange
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();

            // Act
            quantityTextBox.Enter("50");
            updateButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            Assert.AreEqual("Выберите товар для обновления!", message);
        }

        [TestMethod]
        public void TC009_UpdateQuantityWithEmptyQuantityField()
        {
            // Arrange
            AddItem("Тестовый товар", "10", "100", "Тест");
            SelectFirstItem();

            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();

            // Act
            quantityTextBox.Enter("");
            updateButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            Assert.AreEqual("Введите новое количество!", message);
        }
        [TestMethod]
        public void TC010_UpdateQuantityWithInvalidFormat()
        {
            AddItem("Тестовый товар", "10", "100", "Тест");
            SelectFirstItem();

            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            string originalItemText = itemsListBox.Items.Last().Text;

            // Act 
            quantityTextBox?.Enter("не число");
            updateButton?.Click();

            // Assert 
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            // Assert 
            string currentItemText = itemsListBox?.Items?.Last()?.Text;

            Assert.AreEqual("Неверный формат количества!", message);
            Assert.AreEqual(originalItemText, currentItemText);
        }

        [TestMethod]
        public void TC011_AddItemWithZeroQuantity()
        {
            string name = "Товар с нулевым количеством";
            string quantity = "0";
            string price = "100";
            string category = "Тест";
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();

            // Act
            nameTextBox?.Enter(name);
            quantityTextBox?.Enter(quantity);
            priceTextBox?.Enter(price);
            categoryTextBox?.Enter(category);
            addButton?.Click();

            // Assert
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            string lastItem = String.Empty;
            if (itemsListBox?.Items?.Length > 0)
            {
                lastItem = itemsListBox?.Items?.Last()?.Text;
            }

            Assert.IsTrue(lastItem.Contains("Товар с нулевым количеством"));
            Assert.IsTrue(lastItem.Contains("0"));
        }

        [TestMethod]
        public void TC012_AddItemWithZeroPrice()
        {
            // Act
            AddItem("Бесплатный товар", "10", "0", "Тест");

            // Assert
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            string lastItem = itemsListBox?.Items?.Last()?.Text;

            Assert.IsTrue(lastItem.Contains("Бесплатный товар"));
            Assert.IsTrue(lastItem.Contains("0"));
        }
        [TestMethod]
        public void TC013_DataPersistanceAfterRestart()
        {
            // Arrange
            AddItem("Молоко", "10", "80", "Молочные");
            AddItem("Хлеб", "5", "40", "Выпечка");
            AddItem("Сыр", "3", "300", "Молочные");

            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            int itemsCountBeforeRestart = itemsListBox.Items.Length;

            // Act 
            _app.Close();
            _app = Application.Launch(@"..\..\..\InventoryManager\bin\Debug\InventoryManager.exe");
            _mainWindow = _app.GetMainWindow(_automation);

            // Assert 
            itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            int itemsCountAfterRestart = itemsListBox.Items.Length;

            Assert.AreEqual(itemsCountBeforeRestart, itemsCountAfterRestart);
        }

        [TestMethod]
        public void TC015_AddItemWithNegativeQuantity_ShouldShowErrorMessage()
        {
            // Arrange
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            int itemsCountBefore = itemsListBox.Items.Length;

            // Act
            nameTextBox.Enter("Товар с отрицательным количеством");
            quantityTextBox.Enter("-10");
            priceTextBox.Enter("100");
            categoryTextBox.Enter("Тест");
            addButton.Click();

            // Assert 
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            // Assert
            int itemsCountAfter = itemsListBox.Items.Length;

            Assert.IsTrue(message.Contains("отрицательным") || message.Contains("количеств"));
            Assert.AreEqual(itemsCountBefore, itemsCountAfter);
        }

        [TestMethod]
        public void TC016_AddItemWithNegativePrice_ShouldShowErrorMessage()
        {
            // Arrange
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            int itemsCountBefore = itemsListBox.Items.Length;

            // Act
            nameTextBox.Enter("Товар с отрицательной ценой");
            quantityTextBox.Enter("10");
            priceTextBox.Enter("-50");
            categoryTextBox.Enter("Тест");
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            // Assert
            int itemsCountAfter = itemsListBox.Items.Length;

            Assert.IsTrue(message.Contains("отрицательной"));
            Assert.AreEqual(itemsCountBefore, itemsCountAfter);
        }
        [TestMethod]
        public void TC017_UpdateQuantityToNegativeValue_ShouldShowErrorMessage()
        {
            // Arrange 

            string name = "Товар для обновления";
            string quantity = "50";
            string price = "100";
            string category = "Тест";
            var nameTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("nameTextBox"))?.AsTextBox();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var priceTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("priceTextBox"))?.AsTextBox();
            var categoryTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("categoryTextBox"))?.AsTextBox();
            var addButton = _mainWindow.FindFirstDescendant(x => x.ByName("addItemButton"))?.AsButton();

            // Act
            nameTextBox?.Enter(name);
            quantityTextBox?.Enter(quantity);
            priceTextBox?.Enter(price);
            categoryTextBox?.Enter(category);
            addButton?.Click();
            SelectFirstItem();

            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();

            string originalItemText = String.Empty;
            if (itemsListBox?.Items?.Length > 0)
            {
                originalItemText = itemsListBox?.Items?.Last()?.Text;
            }

            // Act 
            quantityTextBox.Enter("-5");
            updateButton.Click();

            // Assert 
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();
            string currentItemText = itemsListBox?.Items?.Last()?.Text;
            Assert.IsTrue(message.Contains("отрицательным") || message.Contains("количеств"));
            Assert.AreEqual(originalItemText, currentItemText);
        }

        [TestMethod]
        public void TC018_AddTwoItemsWithSameName_ShouldAddOnlyOne()
        {
            // Arrange
            var itemsListBox = _mainWindow.FindFirstDescendant(x => x.ByName("itemsListBox"))?.AsListBox();
            int initialCount = itemsListBox.Items.Length;

            // Act 
            AddItem("Товар", "100", "100", "Тест");
            AddItem("Товар", "50", "150", "Тест");

            // Assert
            int finalCount = itemsListBox.Items.Length;
            Assert.AreEqual(initialCount + 1, finalCount);
            int itemsWithSameName = itemsListBox.Items.Count(item => item.Text.Contains("Товар"));
            Assert.AreEqual(1, itemsWithSameName);
        }
        [TestMethod]
        public void TC022_SetNotificationThreshold()
        {
            // Arrange
            AddItem("Низкий запас товар", "3", "100", "Тест");

            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            var updateThresholdButton = _mainWindow.FindFirstDescendant(x => x.ByName("noticationButton"))?.AsButton();

            // Act
            thresholdTextBox.Enter("5");
            updateThresholdButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            // Assert
            bool notificationShown = WaitForNotification(10000);
            Assert.IsTrue(notificationShown);
        }

        [TestMethod]
        public void TC023_LowStockNotification()
        {
            // Arrange
            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            var updateThresholdButton = _mainWindow.FindFirstDescendant(x => x.ByName("noticationButton"))?.AsButton();

            thresholdTextBox.Enter("10");
            updateThresholdButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            // Act
            AddItem("Молоко", "8", "80", "Молочные");

            bool notificationShown = WaitForNotification(10000);

            // Assert 
            Assert.IsTrue(notificationShown);
        }

        [TestMethod]
        public void TC024_RepeatingNotifications()
        {
            // Arrange
            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            var updateThresholdButton = _mainWindow.FindFirstDescendant(x => x.ByName("noticationButton"))?.AsButton();

            thresholdTextBox.Enter("10");
            updateThresholdButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            AddItem("Товар с низким запасом", "5", "100", "Тест");

            // Act
            int notificationCount = CountNotificationsInPeriod(20000);

            // Assert
            Assert.IsTrue(notificationCount >= 2);
        }

        [TestMethod]
        public void TC025_NotificationDisappearsAfterStockIncrease()
        {
            // Arrange
            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            var updateThresholdButton = _mainWindow.FindFirstDescendant(x => x.ByName("noticationButton"))?.AsButton();

            thresholdTextBox.Enter("10");
            updateThresholdButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox?.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string message = messageText?.Name;
            var okButton = messageBox?.FindFirstDescendant(x => x.ByAutomationId("2"))?.AsButton();
            okButton?.Click();

            AddItem("Товар для пополнения", "5", "100", "Тест");
            WaitForNotification(5000);

            // Act
            SelectFirstItem();
            var quantityTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("quantityTextBox"))?.AsTextBox();
            var updateButton = _mainWindow.FindFirstDescendant(x => x.ByName("updateQuantityButton"))?.AsButton();

            quantityTextBox.Enter("15");
            updateButton.Click();

            // Assert
            bool notificationAppeared = WaitForNotification(15000);
            Assert.IsFalse(notificationAppeared);
        }

        [TestMethod]
        public void TC026_NotificationThresholdValidation()
        {
            // Arrange
            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();

            // Act
            thresholdTextBox.Enter("abc");
            string textAfterLetters = thresholdTextBox.Text;
            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(textAfterLetters) || textAfterLetters.All(char.IsDigit));

            // Act
            thresholdTextBox.Enter("-100");
            string textAfterNegative = thresholdTextBox.Text;
            // Assert
            bool isNonNegative = string.IsNullOrEmpty(textAfterNegative) ||
                                (int.TryParse(textAfterNegative, out int number) && number >= 0);
            Assert.IsTrue(isNonNegative);
        }

        [TestMethod]
        public void TC027_NotificationThresholdPersistence()
        {
            // Arrange
            var thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            var updateThresholdButton = _mainWindow.FindFirstDescendant(x => x.ByName("noticationButton"))?.AsButton();
            int currentThreshold = int.TryParse(thresholdTextBox.Text, out int current) ? current : 0;
            int newThreshold = currentThreshold + 120;

            // Act
            thresholdTextBox.Enter(newThreshold.ToString());
            updateThresholdButton.Click();
            _app.Close();
            _app = Application.Launch(@"..\..\..\InventoryManager\bin\Debug\InventoryManager.exe");
            _mainWindow = _app.GetMainWindow(_automation);

            // Assert
            thresholdTextBox = _mainWindow.FindFirstDescendant(x => x.ByName("thresholdTextBox"))?.AsTextBox();
            string persistedThreshold = thresholdTextBox.Text;
            Assert.AreEqual(newThreshold.ToString(), persistedThreshold);
        }
    }
}