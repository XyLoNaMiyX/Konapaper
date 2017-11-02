using System;
using System.Xml.Serialization;

namespace Konapaper
{
    [Serializable, XmlRoot("posts")]
    public class PostsList
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("offset")]
        public int Offset { get; set; }

        [XmlElement("post")]
        public Post[] Posts;
    }
}
