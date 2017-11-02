using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Konapaper
{
    public class KonachanImage
    {
        #region Private fields

        // store the images locally after downloaded to save bandwidth
        BitmapImage preview, small, large;

        #endregion

        #region Properties

        /// <summary>
        /// The URL of the preview image
        /// </summary>
        public string PreviewUrl { get; private set; }
        /// <summary>
        /// The URL of the "small" sized version of the image
        /// </summary>
        public string SmallUrl { get; private set; }
        /// <summary>
        /// The URL of the large sized version of the image
        /// </summary>
        public string LargeUrl { get; private set; }

        /// <summary>
        /// The Konachan URL
        /// </summary>
        public string KonachanUrl { get; set; }

        /// <summary>
        /// The full name of this image
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// The ID of this image
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The page in which the image was found
        /// </summary>
        public int Page { get; internal set; }

        /// <summary>
        /// The tags of the image
        /// </summary>
        public string[] Tags { get {
                var result = new string[_Tags.Length];
                Array.Copy(_Tags, result, _Tags.Length);
                return result;
            } }
        string[] _Tags;

        #endregion

        const int idIndex = 15; // "Konachan.com - ".Length

        #region Constructors

        public KonachanImage(string previewUrl, string smallUrl, string largeUrl)
        {
            PreviewUrl = previewUrl;
            SmallUrl = smallUrl;
            LargeUrl = largeUrl;

            if (SmallUrl != null || LargeUrl != null)
            {
                // use the non-null url
                var okUrl = SmallUrl == null ? LargeUrl : SmallUrl;

                FullName = okUrl.Substring(okUrl.LastIndexOf('/') + 1); // + 1 to skip the /)
                ID = int.Parse(FullName.Substring(idIndex, FullName.IndexOf(' ', idIndex) - idIndex));

                //                 Konac...- ID.................. +' '
                var tagsPosition = idIndex + ID.ToString().Length + 1;
                _Tags = FullName.Substring(tagsPosition, // specify length to trim ".ext" part
                    FullName.LastIndexOf('.') - (idIndex + ID.ToString().Length + 1)).Split(' ');

                KonachanUrl = (okUrl.Contains("konachan.com") ?
                    "https://konachan.com/post/show/" :
                    "https://konachan.net/post/show/") + ID;
            }
            else
            {
                FullName = "Unknown";
                ID = 0;
            }
        }

        #endregion

        #region Get image

        /// <summary>
        /// Gets the preview of this image. If it's unavailable, returns null
        /// </summary>
        /// <returns>The preview</returns>
        public async Task<BitmapImage> GetPreview()
        {
            if (PreviewUrl == null) return null;
            if (preview == null) preview = await downloadImage(PreviewUrl);
            return preview;
        }

        /// <summary>
        /// Gets the "small" sized version of this image. If it's unavailable, returns null
        /// </summary>
        /// <returns>The image</returns>
        public async Task<BitmapImage> GetSmall()
        {
            if (SmallUrl == null) return null;
            if (small == null) small = await downloadImage(SmallUrl);
            return small;
        }

        /// <summary>
        /// Gets the large sized version of this image. If it's unavailable, returns null
        /// </summary>
        /// <returns>The image</returns>
        public async Task<BitmapImage> GetLarge()
        {
            if (LargeUrl == null) return null;
            if (large == null) large = await downloadImage(LargeUrl);
            return large;
        }

        #endregion

        #region Private methods

        // download the image from the given url
        static async Task<BitmapImage> downloadImage(string url)
        {
            const int maxTries = 3;
            int currentTry = 0;

            while (currentTry < maxTries)
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        var ms = new MemoryStream(await wc.DownloadDataTaskAsync(url));
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.EndInit();
                        return image;
                    }
                }
                catch
                {
                    await Task.Factory.StartNew(() => Thread.Sleep(1000));
                }
            }

            return null;
        }

        #endregion
    }
}
