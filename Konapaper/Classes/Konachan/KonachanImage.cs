using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Konapaper
{
    public class KonachanImage : Post
    {
        #region Private fields

        // used to cancel an image from being downloaded
        static CancellationTokenSource cts;

        // cache the images
        BitmapImage preview;
        BitmapImage image;

        #endregion

        #region Constructors

        public KonachanImage(Post post)
        {
            PreviewURL = post.PreviewURL;
            ActualPreviewHeight = post.ActualPreviewHeight;
            ActualPreviewWidth = post.ActualPreviewWidth;
            PreviewHeight = post.PreviewHeight;
            PreviewWidth = post.PreviewWidth;
            SampleFileSize = post.SampleFileSize;
            SampleHeight = post.SampleHeight;
            SampleURL = post.SampleURL;
            SampleWidth = post.SampleWidth;
            Height = post.Height;
            Width = post.Width;
            Author = post.Author;
            CreatorID = post.CreatorID;
            Frames = post.Frames;
            FramesString = post.FramesString;
            FramesPending = post.FramesPending;
            FramesPendingString = post.FramesPendingString;
            CreatedAt = post.CreatedAt;
            ID = post.ID;
            Rating = post.Rating;
            Score = post.Score;
            Source = post.Source;
            Status = post.Status;
            Tags = post.Tags;
            Change = post.Change;
            MD5 = post.MD5;
            HasChildren = post.HasChildren;
            IsHeld = post.IsHeld;
            IsShownInIndex = post.IsShownInIndex;
            JpegFileSize = post.JpegFileSize;
            JpegURL = post.JpegURL;
            JpegHeight = post.JpegHeight;
            JpegWidth = post.JpegWidth;
        }

        #endregion

        #region Get image

        /// <summary>
        /// Gets the preview of this image. If it's unavailable, returns null
        /// </summary>
        /// <returns>The preview</returns>
        public async Task<BitmapImage> GetPreview()
        {
            if (preview == null)
                preview = await downloadImage(PreviewURL);
            return preview;
        }

        /// <summary>
        /// Gets the real sized version of this image. If it's unavailable, returns null
        /// </summary>
        /// <returns>The image</returns>
        public async Task<BitmapImage> GetImage()
        {
            if (image == null)
                image = await downloadImage(JpegURL);
            return image;
        }

        /// <summary>
        /// Cancels any running get image task
        /// </summary>
        public void CancelGetImage()
        {
            if (cts != null)
                cts.Cancel();
        }

        #endregion

        #region Private methods

        // download the image from the given url
        static async Task<BitmapImage> downloadImage(string url)
        {
            try
            {
                cts = new CancellationTokenSource();
                cts.Token.ThrowIfCancellationRequested();

                using (var wc = new WebClient())
                {
                    using (cts.Token.Register(wc.CancelAsync))
                    {
                        var ms = new MemoryStream(await wc.DownloadDataTaskAsync(url));
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
            }
            catch { return null; }
            finally { cts = null; }
        }

        #endregion
    }
}
