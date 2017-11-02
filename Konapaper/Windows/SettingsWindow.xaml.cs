using System.Windows;
using System.Windows.Input;

namespace Konapaper
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            loadSettings();
        }

        void loadSettings()
        {
            Settings.SetSettingDisabled = true;
            
            fadeSeen.IsChecked = Settings.GetValue<bool>("fadeSeen");
            switch (Settings.GetValue<int>("notifyPriority"))
            {
                default:
                case 0:
                    notifyNever.IsChecked = true;
                    break;

                case 1:
                    notifyStart.IsChecked = true;
                    break;

                case 2:
                    notifyAlways.IsChecked = true;
                    break;
            }
            showNotificationsPreview.IsChecked = Settings.GetValue<bool>("showNotificationsPreview");

            Settings.SetSettingDisabled = false;
        }

        #region Title bar

        // move
        void titleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try { DragMove(); }
                catch { }
            }
        }

        #endregion

        void acceptMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // fade seen
        void fadeSeen_CheckedChanged(object sender, RoutedEventArgs e)
            => Settings.SetValue("fadeSeen", fadeSeen.IsChecked.Value);

        // notify priority
        void notifyNever_Checked(object sender, RoutedEventArgs e) => updateNotifyPriority(0);
        void notifyStart_Checked(object sender, RoutedEventArgs e) => updateNotifyPriority(1);
        void notifyAlways_Checked(object sender, RoutedEventArgs e) => updateNotifyPriority(2);

        void updateNotifyPriority(int value)
        {
            if (Settings.SetSettingDisabled)
                return;

            bool result;
            if (value > 0)
            {
                var args = value == 2 ? "--min" : "--singlecheck";
                result = WindowsStartup.EnableRunAtStartUp(args);
            }
            else
                result = WindowsStartup.DisableRunAtStartUp();

            if (result)
                Settings.SetValue("notifyPriority", value);
            else
                notifyCouldNotChange();
        }

        void notifyCouldNotChange()
        {
            MessageBox.Show("Could not update this setting. Try running the application as administrator before trying again",
                "Could not update the setting", MessageBoxButton.OK, MessageBoxImage.Error);
            loadSettings();
        }

        // show notificiations preview
        void showNotificationsPreview_CheckedChanged(object sender, RoutedEventArgs e)
            => Settings.SetValue("showNotificationsPreview", showNotificationsPreview.IsChecked.Value);

        const string delayText = "Delay between notifications (in minutes): {0}";
        const string postPerPageText = "Show {0} posts per page";

        void notificationDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Settings.SetValue("notificationDelay", (int)delaySlider.Value);
            //delayBlock.Text = string.Format(delayText, (int)delaySlider.Value);
        }

        private void postPerPage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Settings.SetValue("postsPerPage", (int)postPerPageSlider.Value);
            //postPerPageBlock.Text = string.Format(postPerPageText, (int)postPerPageSlider.Value);
        }
    }
}
