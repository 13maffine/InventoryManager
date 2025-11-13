using System;
using System.Collections.Generic;
using System.IO;

namespace InventoryManagerNmspc
{
    public class NotificationManager
    {
        private int _notificationThreshold = 10;
        private const string ThresholdFileName = "notification_threshold.txt";

        public int NotificationThreshold
        {
            get => _notificationThreshold;
            set
            {
                if (value >= 0)
                {
                    _notificationThreshold = value;
                }
            }
        }

        public NotificationManager()
        {
            LoadThreshold();
        }

        private void LoadThreshold()
        {
            try
            {
                if (File.Exists(ThresholdFileName))
                {
                    string content = File.ReadAllText(ThresholdFileName);
                    if (int.TryParse(content, out int savedThreshold) && savedThreshold >= 0)
                    {
                        _notificationThreshold = savedThreshold;
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationThreshold = 10;
                throw new Exception($"Ошибка загрузки порога: {ex.Message}");
            }
        }

        public void SaveThreshold()
        {
            try
            {
                File.WriteAllText(ThresholdFileName, _notificationThreshold.ToString());
            }
            catch (Exception ex)
            {
               throw new Exception($"Ошибка сохранения порога: {ex.Message}");
            }
        }

        public List<string> CheckInventoryLevels(List<InventoryItem> items)
        {
            if (items == null) throw new ArgumentNullException("Неинициалищированный список товаро!");

            List<string> notifications = new List<string>();

            foreach (var item in items)
            {
                if (item.Quantity < _notificationThreshold)
                {
                    notifications.Add($"Низкий запас: {item.Name}. Текущее количество: {item.Quantity} (порог: {_notificationThreshold})");
                }
            }

            return notifications;
        }
    }
}