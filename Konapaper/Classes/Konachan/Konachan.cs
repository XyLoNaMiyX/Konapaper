using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        /// Determines the page count
        /// </summary>
        public int PageCount => PostCount / PostsPerPage;

        /// <summary>
        /// Determines the post count
        /// </summary>
        public int PostCount { get; private set; }

        /// <summary>
        /// The maximum allowed rating for the images. Lower ratings will be valid
        /// </summary>
        public Rating AllowedRating { get; set; }

        /// <summary>
        /// The amount of posts per page
        /// </summary>
        public int PostsPerPage { get; set; } = 20;

        /// <summary>
        /// The current URL for the selected page with the selected filters
        /// </summary>
        public string PageUrl
        {
            get
            {
                if (Tags == null || Tags.Count == 0)
                    return string.Format(basePostsUrl, PostsPerPage, Page, string.Empty);

                return string.Format(basePostsUrl, PostsPerPage, Page, string.Join(" ", Tags));
            }
        }

        /// <summary>
        /// The current search tags
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        #endregion

        #region Constant fields

        // posts api request base url
        const string basePostsUrl = "http://konachan.com/post.xml/?limit={0}&page={1}&tags={2}";

        // used to clear empty attributes
        static readonly Regex removeEmptyAttributesRegex = new Regex(@"\w+=""""", RegexOptions.Compiled);

        #endregion

        #region Public methods

        public async Task<List<KonachanImage>> GetImages()
        {
            var result = new List<KonachanImage>();

            using (var wc = new WebClient())
            {
                var posts = SerializerXML.DeserializeFromString<PostsList>(
                    removeEmptyAttributesRegex.Replace(await wc.DownloadStringTaskAsync(PageUrl), string.Empty));

                foreach (var post in posts.Posts)
                    if (post.Rating <= AllowedRating)
                        result.Add(new KonachanImage(post));
            }

            return result;
        }

        #endregion
    }
}