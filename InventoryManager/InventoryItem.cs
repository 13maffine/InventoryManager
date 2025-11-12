using System;

namespace InventoryManagerNmspc
{
    public class InventoryItem
    {
        private string name;
        private int quantity;
        private decimal price;
        private string category;
        public string Name
        {
            get { return name; }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Название не должно быть пустым!");
                else name = value;
            }
        }
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value < 0) throw new ArgumentException("Количество не может быть отрицательным!");
                else quantity = value;
            }
        }
        public decimal Price
        {
            get { return price; }
            set
            {
                if (value < 0) throw new ArgumentException("Цены не может быть отрицательной!");
                else price = value;
            }
        }
        public string Category
        {
            get { return category; }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Категория не должна быть пустой!");
                else category = value;
            }
        }
        public InventoryItem(string name, int quantity, decimal price, string category)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Category = category;
        }
    }
}
