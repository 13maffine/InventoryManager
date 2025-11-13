using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagerNmspc
{
    public partial class InventoryForm : Form
    {
        private InventoryManager inventoryManager;
        private NotificationManager notificationManager;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label quantityLabel;
        private TextBox quantityTextBox;
        private Label priceLabel;
        private TextBox priceTextBox;
        private Label categoryLabel;
        private TextBox categoryTextBox;
        private Button addItemButton;
        private Button removeItemButton;
        private Button updateQuantityButton;
        private Label itemsLabel;
        private ListBox itemsListBox;

        private Label thresholdLabel;
        private TextBox thresholdTextBox;
        private Button noticationButton;
        private Timer notificationTimer;

        private bool allowNotifications = false;

        public InventoryForm()
        {
            inventoryManager = new InventoryManager();
            notificationManager = new NotificationManager();

            this.Text = "Управление инвентарём";
            this.Width = 500;
            this.Height = 400;
            nameLabel = new Label
            {
                Location = new System.Drawing.Point(10, 10),
                Text = "Название",
            };
            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 150,
            };
            quantityLabel = new Label
            {
                Location = new System.Drawing.Point(170, 10),
                Text = "Количество",
                Width = 80,
            };
            quantityTextBox = new TextBox
            {
                Location = new System.Drawing.Point(170, 40),
                Width = 80,
            };
            priceLabel = new Label
            {
                Location = new System.Drawing.Point(260, 10),
                Text = " Цена",
            };
            priceTextBox = new TextBox
            {
                Location = new System.Drawing.Point(260, 40),
                Width = 100,
            };
            categoryLabel = new Label
            {
                Location = new System.Drawing.Point(370, 10),
                Text = "Категория",
            };
            categoryTextBox = new TextBox
            {
                Location = new System.Drawing.Point(370, 40),
                Width = 100,
            };
            addItemButton = new Button
            {
                Location = new System.Drawing.Point(10, 70),
                Text = "Добавить",
                Width = 100
            };
            addItemButton.Click += AddItemButton_Click;
            removeItemButton = new Button
            {
                Location = new System.Drawing.Point(120, 70),
                Text = "Удалить",
                Width = 100
            };
            removeItemButton.Click += RemoveItemButton_Click;

            updateQuantityButton = new Button
            {
                Location = new System.Drawing.Point(220, 70),
                Text = "Обновить кол-во товара",
                Width = 150,
            };
            updateQuantityButton.Click += UpdateQuantityButton_Click;

            itemsLabel = new Label
            {
                Location = new System.Drawing.Point(10, 100),
                Text = "Список товаров",
            };
            itemsListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 130),
                Width = 460,
                Height = 150
            };
            thresholdLabel = new Label
            {
                Location = new Point(10, 290),
                Text = "Порог уведомлений:",
                Width = 120
            };
            thresholdTextBox = new TextBox
            {
                Location = new Point(130, 290),
                Width = 80,
                Text = notificationManager.NotificationThreshold.ToString(),
            };
            noticationButton = new Button
            {
                Location = new Point(220, 290),
                Width = 150,
                Height = 40,
                Text = "Обновить порог количества товара"
            };

            thresholdTextBox.KeyPress += ThresholdTextBox_KeyPress;
            noticationButton.Click += NotificationButton_Click;

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(quantityLabel);
            this.Controls.Add(quantityTextBox);
            this.Controls.Add(priceLabel);
            this.Controls.Add(priceTextBox);
            this.Controls.Add(categoryLabel);
            this.Controls.Add(categoryTextBox);
            this.Controls.Add(addItemButton);
            this.Controls.Add(removeItemButton);
            this.Controls.Add(updateQuantityButton);
            this.Controls.Add(itemsLabel);
            this.Controls.Add(itemsListBox);
            this.Controls.Add(thresholdLabel);
            this.Controls.Add(thresholdTextBox);
            this.Controls.Add(noticationButton);

            UpdateItemsList();
            this.Shown += (s, e) => StartNotificationTimer();
        }

        private async void StartNotificationTimer()
        {
            await Task.Delay(5000);

            allowNotifications = true;

            notificationTimer = new Timer();
            notificationTimer.Interval = 10000;
            notificationTimer.Tick += (s, e) => CheckAndUpdateNotifications();
            notificationTimer.Start();

            CheckAndUpdateNotifications();
        }

        private void CheckAndUpdateNotifications()
        {
            var newNotifications = notificationManager.CheckInventoryLevels(inventoryManager.Items);

            foreach (var notification in newNotifications)
            {
                ShowNotificationAlert(notification);
            }
        }

        private void ShowNotificationAlert(string notification)
        {
            MessageBox.Show(notification, "Уведомление о низком запасе",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ThresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void NotificationButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(thresholdTextBox.Text, out int threshold) && threshold >= 0)
            {
                notificationManager.NotificationThreshold = threshold;

                notificationManager.SaveThreshold();

                MessageBox.Show($"Порог уведомлений установлен: {threshold}", "Порог обновлен",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Введите корректное числовое значение для порога!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateItemsList()
        {
            itemsListBox.Items.Clear();
            foreach (var item in inventoryManager.Items)
            {
                itemsListBox.Items.Add($"{item.Name} - Количество: {item.Quantity} | Цена: {item.Price} руб. | Категория: {item.Category}");
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) ||
            string.IsNullOrEmpty(quantityTextBox.Text) || string.IsNullOrEmpty(priceTextBox.Text) ||
            string.IsNullOrEmpty(categoryTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            int quantity;
            decimal price;
            if (!int.TryParse(quantityTextBox.Text, out quantity) ||
            !decimal.TryParse(priceTextBox.Text, out price))
            {
                MessageBox.Show("Неверный формат количества или цены!");
                return;
            }
            try
            {
                InventoryItem newItem = new InventoryItem(nameTextBox.Text, quantity, price, categoryTextBox.Text);
                inventoryManager.AddItem(newItem);
                nameTextBox.Clear();
                quantityTextBox.Clear();
                priceTextBox.Clear();
                categoryTextBox.Clear();
                UpdateItemsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для удаления!");
                return;
            }
            string selectedItem = itemsListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string name = parts[0].Trim();
                var itemToRemove = inventoryManager.Items.Find(i => i.Name == name);
                if (itemToRemove != null)
                {
                    try
                    {
                        inventoryManager.RemoveItem(itemToRemove);
                        UpdateItemsList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void UpdateQuantityButton_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для обновления!");
                return;
            }
            string selectedItem = itemsListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string name = parts[0].Trim();
                var itemToUpdate = inventoryManager.Items.Find(i => i.Name == name);
                if (itemToUpdate != null)
                {
                    if (string.IsNullOrEmpty(quantityTextBox.Text))
                    {
                        MessageBox.Show("Введите новое количество!");
                        return;
                    }
                    int newQuantity;
                    if (!int.TryParse(quantityTextBox.Text, out newQuantity))
                    {
                        MessageBox.Show("Неверный формат количества!");
                        return;
                    }
                    try
                    {
                        inventoryManager.UpdateItemQuantity(itemToUpdate, newQuantity);
                        UpdateItemsList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            notificationTimer?.Stop();
            base.OnFormClosed(e);
        }
    }
}