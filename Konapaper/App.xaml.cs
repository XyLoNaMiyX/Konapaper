using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace Konapaper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SelectCulture(Thread.CurrentThread.CurrentUICulture.ToString());

            Settings.Init("LonamiWebs\\Konapaper", new Dictionary<string, dynamic>
            {
                { "notificationDelay", 20 },
                { "allowedRating", (int)Rating.Safe },
                { "postsPerPage", 20 },
                { "fadeSeen", false },
                { "notifyPriority", (int)StartMode.Normal },
                { "showNotificationsPreview", false },
            });
        }
        
        public static void SelectCulture(string culture)
        {
            // List all our resources      
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                dictionaryList.Add(dictionary);

            // We want our specific culture      
            string requestedCulture = string.Format("StringResources.{0}.xaml", culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault
                (d => d.Source.OriginalString == requestedCulture);
            if (resourceDictionary == null)
            {
                requestedCulture = "StringResources.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault
                    (d => d.Source.OriginalString == requestedCulture);
            }

            // If we have the requested resource, remove it from the list and place at the end
            // Then this language will be our string table to use.     
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            // Inform the threads of the new culture      
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        public static BitmapSource GetUserImage()
        {
            var file = Settings.GetValue<string>("userImage");
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return null;

            return loadImage(file);
        }

        static BitmapSource loadImage(string file)
        {
            var image = new BitmapImage();
            using (FileStream stream = File.OpenRead(file))
            {
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit(); // load the image from the stream
            } // close the stream
            return image;
        }
    }
}

