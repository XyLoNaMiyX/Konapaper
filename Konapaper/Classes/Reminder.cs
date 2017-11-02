using System;
using System.Windows.Threading;

namespace Konapaper
{
    public delegate void RememberEventHandler();

    public static class Reminder
    {
        #region Public events

        public static event RememberEventHandler Remember;

        #endregion

        #region Private fields

        static DispatcherTimer timer;
        static bool checkAlways;

        #endregion

        #region Initialize

        public static void Init(RememberEventHandler rememberEvent, bool checkAlways)
        {
            Remember += rememberEvent;

            Reminder.checkAlways = checkAlways;
            if (checkAlways)
                initTimer();
        }

        #endregion

        #region Public methods

        public static void RemindLater()
        {
            if (timer == null)
                initTimer();
        }

        #endregion

        #region Private methods

        static void initTimer()
        {
            timer = new DispatcherTimer(TimeSpan.FromMinutes(Settings.GetValue<int>("notificationDelay")),
                DispatcherPriority.Background, timerTick, App.Current.Dispatcher);
        }

        static void timerTick(object sender, EventArgs e)
        {
            if (!checkAlways)
            {
                timer.Stop();
                timer = null;
            }
            Remember();
        }

        #endregion
    }
}
