using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public  class Notifications
    {
        private static Notifications instance;
        public static Notifications GetNotifications() 
        {
            if (instance == null)
            {
                instance = new Notifications();
                return instance;
            }
            else { return instance; }
        }
        public delegate void CommonLog(string message, NotificationEvents notificationEvents);
        public event CommonLog OnCommonPushpin;
        public enum NotificationEvents
        {
            ConnectionPort,
            NotConnectionPort,
            CalibrateMax,
            CalibrateMin,
            Success,
            PositionProcessor
          
        }
        public void InvokeCommonStatus(string msg,NotificationEvents notificationEvents)
        {
            OnCommonPushpin?.Invoke(msg,notificationEvents);
        }
    }
}
