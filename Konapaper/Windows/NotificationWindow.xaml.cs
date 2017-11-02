using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Konapaper
{
    public partial class NotificationWindow : Window
    {
        const string singleImageString = "There is a new available image";
        const string multipleImagesString = "There are {0} new images available";

        List<KonachanImage> images;

        DispatcherTimer slideshowTimer;
        const int slideshowDuration = 5000;
        const int fadeDuration = 300;

        NotificationAction result;

        public static NotificationAction Notify(List<KonachanImage> images)
        {
            var window = new NotificationWindow(images);
            window.ShowDialog();
            return window.result;
        }


        NotificationWindow(List<KonachanImage> images)
        {
            InitializeComponent();

            this.images = images;
            if (images.Count == 1)
                notificationText.Text = singleImageString;

            else
            {
                notificationText.Text = string.Format(multipleImagesString, images.Count);
                if (Settings.GetValue<bool>("showNotificationsPreview"))
                {
                    slideshowTimer = new DispatcherTimer(
                        TimeSpan.FromMilliseconds(slideshowDuration),
                        DispatcherPriority.Background,
                        slideshowTick,
                        Dispatcher);
                }
            }
        }

        async void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.GetValue<bool>("showNotificationsPreview"))
                await showNextImage();
        }

        int _currentImage;
        int currentImage
        {
            get { return _currentImage; }
            set { _currentImage = value % images.Count; }
        }
        async Task showNextImage()
        {
            slideshowTimer.IsEnabled = false;

            // load image...
            BitmapSource newImage = await images[currentImage++].GetPreview();

            // animate and set image
            await startPreviewAnimation(0);
            preview.Source = newImage;
            await startPreviewAnimation(1);

            slideshowTimer.IsEnabled = true;
        }

        async void slideshowTick(object s, EventArgs e)
        {
            await showNextImage();
        }

        async Task startPreviewAnimation(double toOpacity)
        {
            preview.BeginAnimation(Image.OpacityProperty, new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(fadeDuration),
            });
            await Task.Delay(fadeDuration);
        }

        #region Buttons

        void dismiss_Click(object sender, RoutedEventArgs e) => closeNotification(NotificationAction.Dismiss);
        void remind_Click(object sender, RoutedEventArgs e) => closeNotification(NotificationAction.RemindLater);
        void show_Click(object sender, RoutedEventArgs e) => closeNotification(NotificationAction.Show);

        #endregion

        void closeNotification(NotificationAction finalAction)
        {
            result = finalAction;

            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            Close();
        }
    }
}
