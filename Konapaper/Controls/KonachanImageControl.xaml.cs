using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Konapaper
{
    public partial class KonachanImageControl : UserControl
    {
        public KonachanImage KonImage { get; private set; }

        public KonachanImageControl(KonachanImage konImage)
        {
            InitializeComponent();
            KonImage = konImage;
        }

        async void loaded(object sender, RoutedEventArgs e)
        {
            await reloadKonImage();
        }

        public async Task UpdateKonImage(KonachanImage konImage)
        {
            KonImage = konImage;
            await reloadKonImage();
        }

        async Task reloadKonImage()
        {
            image.Source = await KonImage.GetPreview();
            if (image.Source == null)
            {
                title.Text = "Could not download " + KonImage.ID;
            }
            else
            {
                title.Text = KonImage.GetName();
                id.Text = KonImage.ID.ToString();
                tagCount.Text = KonImage.Tags.Length + " tags";
                tagCount.ToolTip = "Tags: " + string.Join(", ", KonImage.Tags);
            }
        }

        async void downloadClick(object sender, RoutedEventArgs e)
        {
            contextMenu.IsOpen = false;

            await saveImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                KonImage.GetFileName()));
        }

        async void downloadToClick(object sender, RoutedEventArgs e)
        {
            contextMenu.IsOpen = false;

            var sfd = new SaveFileDialog { FileName =
                 KonImage.GetFileName(), Filter = "Jpeg image|*.jpg" };

            if (sfd.ShowDialog() ?? false)
                await saveImage(sfd.FileName);
        }

        async Task saveImage(string path)
        {
            id.Text = "Downloading and saving image...";
            try
            {
                var encoder = new JpegBitmapEncoder();
                var image = await KonImage.GetImage();

                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var fs = new FileStream(path, FileMode.Create))
                    encoder.Save(fs);
            }
            catch
            {
                MessageBox.Show("Could not save the file",
                    "Saving failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            id.Text = KonImage.ID.ToString();
        }

        void viewInKonachanClick(object sender, RoutedEventArgs e)
        {
            contextMenu.IsOpen = false;
            // TODO CHECK TODOOO!!
            Process.Start(KonImage.JpegURL);
        }

        async void setWallpaperClick(object sender, RoutedEventArgs e)
        {
            contextMenu.IsOpen = false;

            using (var wc = new WebClient())
            {
                var file = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".jpg");
                await wc.DownloadFileTaskAsync(KonImage.JpegURL, file);
                Wallpaper.Set(file, Wallpaper.Style.Stretched);
            }
        }
    }
}
