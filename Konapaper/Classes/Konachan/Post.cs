using System;
using System.IO;
using System.Xml.Serialization;

namespace Konapaper
{
    [Serializable, XmlType(AnonymousType = true)]
    public class Post
    {
        #region Preview

        [XmlAttribute("preview_url")]
        public string PreviewURL { get; set; }

        [XmlAttribute("actual_preview_height")]
        public int ActualPreviewHeight { get; set; }

        [XmlAttribute("actual_preview_width")]
        public int ActualPreviewWidth { get; set; }

        [XmlAttribute("preview_height")]
        public int PreviewHeight { get; set; }

        [XmlAttribute("preview_width")]
        public int PreviewWidth { get; set; }

        #endregion

        #region Sample

        [XmlAttribute("sample_file_size")]
        public int SampleFileSize { get; set; }

        [XmlAttribute("sample_height")]
        public int SampleHeight { get; set; }

        [XmlAttribute("sample_url")]
        public string SampleURL { get; set; }

        [XmlAttribute("sample_width")]
        public int SampleWidth { get; set; }

        #endregion

        #region Image

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        #endregion

        #region Author

        [XmlAttribute("author")]
        public string Author { get; set; }

        [XmlAttribute("creator_id")]
        public int CreatorID { get; set; }

        #endregion

        #region Frames

        [XmlAttribute("frames")]
        public int Frames { get; set; }


        [XmlAttribute("frames_string")]
        public string FramesString { get; set; }


        [XmlAttribute("frames_pending")]
        public int FramesPending { get; set; }


        [XmlAttribute("frames_pending_string")]
        public string FramesPendingString { get; set; }

        #endregion

        #region Dates

        [XmlAttribute("created_at")]
        public long _CreatedAt { get; set; }

        public DateTime CreatedAt
        {
            get { return UnixDate.UnixTimeToDateTime(_CreatedAt); }
            set { _CreatedAt = UnixDate.DateTimeToUnixTime(value); }
        }

        #endregion

        #region Image information

        [XmlAttribute("id")]
        public int ID { get; set; }


        // TODO not public
        [XmlAttribute("rating")]
        public string _Rating { get; set; }
        public Rating Rating
        {
            get
            {
                if (string.IsNullOrEmpty(_Rating))
                    return Rating.Questionable;

                switch (_Rating[0])
                {
                    case 's': return Rating.Safe;
                    default:
                    case 'q': return Rating.Questionable;
                    case 'e': return Rating.Explicit;
                }
            }
            set
            {
                switch (value)
                {
                    case Rating.Safe: _Rating = "s"; break;
                    default:
                    case Rating.Questionable: _Rating = "q"; break;
                    case Rating.Explicit: _Rating = "e"; break;
                }
            }
        }

        [XmlAttribute("score")]
        public int Score { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("tags")]
        public string _Tags { get; set; }
        public string[] Tags
        {
            get
            {
                return string.IsNullOrEmpty(_Tags) ?
                    null :
                    _Tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                if (value == null)
                    _Tags = null;

                else
                    _Tags = string.Join(" ", value);
            }
        }

        [XmlAttribute("change")]
        public int Change { get; set; } // TODO wut?

        [XmlAttribute("md5")]
        public string MD5 { get; set; }

        [XmlAttribute("has_children")]
        public bool HasChildren { get; set; }

        [XmlAttribute("is_held")]
        public bool IsHeld { get; set; }

        [XmlAttribute("is_shown_in_index")]
        public bool IsShownInIndex { get; set; }

        #endregion

        #region Jpeg

        [XmlAttribute("jpeg_file_size")]
        public int JpegFileSize { get; set; }

        [XmlAttribute("jpeg_url")]
        public string JpegURL { get; set; }

        [XmlAttribute("jpeg_height")]
        public int JpegHeight { get; set; }

        [XmlAttribute("jpeg_width")]
        public int JpegWidth { get; set; }

        #endregion

        const string baseName = "Konachan.com - {0} {1}";
        public string GetName() => string.Format(baseName, ID, _Tags);

        public string GetFileName()
        {
            var fileName = GetName() + ".jpg";

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalid in invalidChars)
                if (fileName.IndexOf(invalid) > -1)
                    fileName = fileName.Replace(invalid, '_');

            return fileName;
        }
    }
}
