using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Konapaper
{
    public partial class MainWindow : Window
    {
        #region Private fields

        // notify icon
        NotifyIcon nicon;

        // konachan handler
        Konachan konachan;

        // determines the application start mode
        StartMode startMode;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            nicon = new NotifyIcon(new Uri("pack://application:,,,/Res/app.ico", UriKind.Absolute), "Konapaper");
            nicon.MouseDown += Nicon_MouseDown;

            konachan = new Konachan();
            loadSettings();
        }

        #endregion

        #region Loading

        #endregion

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "--min":

                        if (!checkInternet(false))
                            return;

                        else
                        {
                            startMode = StartMode.Minimized;
                            Reminder.Init(Reminder_Remember, true);
                            hideWindow(true);
                        }
                        break;
                    case "--singlecheck":

                        if (!checkInternet(false))
                            return;

                        else
                        {
                            startMode = StartMode.SingleCheck;
                            Reminder.Init(Reminder_Remember, false);
                            hideWindow(false);
                        }
                        break;
                    default:
                        
                        if (!checkInternet(true))
                            return;
                        else
                            startMode = StartMode.Normal;

                        break;
                }
            }
            else
            {
                if (!checkInternet(true))
                    return;

                startMode = StartMode.Normal;
                await loadNextPage();
            }
        }

        async void Nicon_MouseDown(object sender, MouseButton button)
        {
            showWindow();
            if (konachan.Page == 1)
                await loadNextPage();
        }

        bool checkInternet(bool parameterlessInit)
        {
            if (InternetChecker.IsConnected())
                return true;

            if (parameterlessInit)
                MessageBox.Show("Konapaper requires an internet connection to work. Please connect to the internet and try again",
                    "No internet connection", MessageBoxButton.OK, MessageBoxImage.Error);

            Application.Current.Shutdown();
            return false;
        }

        async void Reminder_Remember()
        {
            await checkNewImages();
        }

        void hideWindow(bool showInTrayBar)
        {
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
            nicon.Visible = showInTrayBar;
        }

        // TODO IF NOT SHOWN LOAD
        void showWindow()
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            nicon.Visible = false;
        }

        async Task checkNewImages()
        {
            switch (NotificationWindow.Notify(await ViewChecker.GetLatestUnviewedImages()))
            {
                case NotificationAction.Dismiss:
                    if (startMode == StartMode.SingleCheck)
                        Application.Current.Shutdown();
                    break;

                case NotificationAction.RemindLater:
                    if (startMode == StartMode.SingleCheck)
                        hideWindow(true);
                    Reminder.RemindLater();
                    break;

                case NotificationAction.Show:
                    showWindow();
                    break;
            }
        }

        async void oh()
        {
            await checkNewImages();
        }

        bool loadingPage;
        async Task loadNextPage()
        {
            if (loadingPage) return;
            loadingPage = true;

            foreach (var img in await konachan.GetImages())
            {
                totalPages.Text = images.Children.Count + "/" + konachan.PostCount;
                images.Children.Add(new KonachanImageControl(img));
            }
            ++konachan.Page;

            loadingPage = false;
        }

        // TODO create class or something D;
        #region Title bar

        // move
        void titleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    DragMove();
                }
                catch { }
            }
        }
        
        void maximizeEvent(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }
        
        // minimize
        void minimizeEvent(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; }
        // close
        void closeEvent(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }

        #endregion

        #region Visibility options

        bool IsMaximized => WindowState == WindowState.Maximized;

        void ToggleMaximize()
        {
            if (IsMaximized) Restore();
            else Maximize();
        }

        Rect restoreLocation;
        void Maximize()
        {
            restoreLocation = new Rect { Width = Width, Height = Height, X = Left, Y = Top };
            
            System.Windows.Forms.Screen currentScreen
                = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Height = currentScreen.WorkingArea.Height;
            Width = currentScreen.WorkingArea.Width;
            Left = currentScreen.WorkingArea.X;
            Top = currentScreen.WorkingArea.Y;
        }

        void Restore()
        {
            WindowState = WindowState.Normal;
            Height = Math.Max(100, restoreLocation.Height);
            Width = Math.Max(100, restoreLocation.Width);
            Left = Math.Max(100, restoreLocation.X);
            Top = Math.Max(100, restoreLocation.Y);
        }

        #endregion

        async void scrollChanged(object sender, ScrollChangedEventArgs e)
        {
            foreach (KonachanImageControl image in images.Children)
            {
                if (isUserVisible(image, imagesScroller))
                {
                    // TODO
                    //currentPage.Text = image.KonImage.Page.ToString();
                    break;
                }
            }

            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
            {
                await loadNextPage();
            }
        }

        static bool isUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container)
                .TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }
        
        void settingsMouseDown(object sender, MouseButtonEventArgs e)
        {
            new SettingsWindow().ShowDialog();
            loadSettings();
        }

        void minimizeMouseDown(object sender, MouseButtonEventArgs e)
        {
            hideWindow(true);
        }

        void exitMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void loadSettings()
        {
            konachan.AllowedRating = (Rating)Settings.GetValue<int>("allowedRating");
            konachan.PostsPerPage = Settings.GetValue<int>("postsPerPage");
        }
    }
}
