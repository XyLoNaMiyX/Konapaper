using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Konapaper
{
    class Konachan
    {
        #region Properties
        
        /// <summary>
        /// The current selected page number
        /// </summary>
        public int Page
        {
            get { return _Page; }
            set { _Page = value > 0 ? value : 1; }
        }
        int _Page = 1;

        /// <summary>
        /// Total pages since last analysis
        /// </summary>
        public int TotalPages
        {
            get { return _PageCount; }
            private set
            {
                if (_PageCount != value)
                {
                    _PageCount = value;
                    onTotalPages(value);
                }
            }
        }
        int _PageCount = 0;

        /// <summary>
        /// Is safe mode enabled?
        /// </summary>
        public bool Safe { get; set; }

        #endregion

        #region Constant strings

        // safe urls
        const string sUrl = "http://konachan.net/post?page=";
        // unsafe url
        const string uUrl = "http://konachan.com/post?page=";

        #endregion


        // splits
        const string sPostsStart = "<ul id=\"post-list-posts\">";
        const string sPostsClose = "</ul>";

        const string sPrevi = "<img src=\""; // preview image
        const string sSmall = "<a class=\"directlink smallimg\" href=\"";
        const string sLarge = "<a class=\"directlink largeimg\" href=\"";

        const string sPreviStart = "<img src=\"";

        // last page regex
        static readonly Regex lpRegex =
            new Regex(@"<a href=""\/post\?page=\d+&amp;tags=.*?"">(\d+)<\/a>", RegexOptions.Compiled);

        public async Task<List<KonachanImage>> GetImages()
        {
            var result = new List<KonachanImage>(0);
            // download
            string html = string.Empty;
            using (var wc = new WebClient())
                html = await wc.DownloadStringTaskAsync(PageUrl);

            var matches = lpRegex.Matches(html);
            if (matches.Count > 0)
                // get the last match (the last page number)             1 = second group (capture)
                TotalPages = int.Parse(matches[matches.Count - 1].Groups[1].Value);

            var postsIdx = html.IndexOf(sPostsStart);
            if (postsIdx < 0) return result; // no posts here

            // set the html to be only the posts part
            html = html.Substring(postsIdx, html.IndexOf(sPostsClose, postsIdx) - postsIdx);
            var images = html.Split(sPrevi);
            
            // skip the first split, before the posts part
            for (int i = 1; i < images.Length; i++)
            {
                var image = images[i];

                // get the preview url (since the start of this split until the next ", the end of img src=""
                string previUrl = image.Substring(0, image.IndexOf('"')).SanitizeUrl();

                // small image start (right after img src=")
                int sis = image.IndexOf(sSmall) + sSmall.Length;
                // large image start (right after img src=")
                int lis = image.IndexOf(sLarge) + sLarge.Length;

                // if the indices are less than the length, the result was not found; else, get the url, substring between img src=""
                string smallUrl = sis < sSmall.Length ? null : image.Substring(sis, image.IndexOf('"', sis) - sis).SanitizeUrl();
                string largeUrl = lis < sLarge.Length ? null : image.Substring(lis, image.IndexOf('"', lis) - lis).SanitizeUrl();

                // add that image
                result.Add(new KonachanImage(previUrl, smallUrl, largeUrl) { Page = _Page });
            }

            return result;
        }

        public async Task<KonachanImage> GetImage(int id)
        {
            // http://konachan.com/post/show/<id>
            await Task.Factory.StartNew(() => { });
            return null;
        }


        /// <summary>
        /// The current URL for the selected page with the selected filters
        /// </summary>
        public string PageUrl {
            get {
                if (Safe)
                    return filters.Count > 0 ? sUrl + Page + "&tags=" + string.Join("+", filters) : sUrl + Page;
                else
                    return filters.Count > 0 ? uUrl + Page + "&tags=" + string.Join("+", filters) : uUrl + Page;
            }
        }

        public delegate void TotalPagesEventHandler(int count);
        public event TotalPagesEventHandler TotalPagesChanged;
        void onTotalPages(int count)
        { if (TotalPagesChanged != null) TotalPagesChanged(count); }

        readonly List<string> filters = new List<string>();

        public void AddFilter(string filter)    => filters.Add(filter);
        public bool DeleteFilter(string filter) => filters.Remove(filter);
        public void ClearFilters()              => filters.Clear();
        public List<string> GetFilters()        => new List<string>(filters);


    }
}