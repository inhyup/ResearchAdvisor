using System.Collections.Generic;

namespace ResearchAdvisor.DomainApi.Model
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string LikedCategory { get; set; }
        public Dictionary<string, int> LikedKeywords { get; set; }
        public List<Entry> LikedPapers { get; set; }
    }
}
