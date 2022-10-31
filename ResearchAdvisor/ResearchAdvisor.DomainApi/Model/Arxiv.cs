using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ResearchAdvisor.DomainApi.Model
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Arxiv
    {
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("updated")]
        public DateTime Updated { get; set; }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("entry")]
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public DateTime updated { get; set; }
        public DateTime published { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        [XmlElement("author")]
        public List<Author> author { get; set; }
        [XmlElement("category")]
        public List<Category> cateory { get; set; }
        public bool isLiked { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
    }
    public class Category
    {
        [XmlAttribute]
        public string term { get; set; }
    }
}
