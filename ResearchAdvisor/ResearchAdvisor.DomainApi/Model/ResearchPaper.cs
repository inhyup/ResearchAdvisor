using System.Collections.Generic;
using System;
using Microsoft.ML.Data;

namespace ResearchAdvisor.DomainApi.Model
{
    public class ResearchPaper
    {
        public string Id { get; set; }
        public string Submitter { get; set; }
        public string Authors { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Journal { get; set; }
        public string Doi { get; set; }
        public string Report { get; set; }
        public string Categories { get; set; }
        public string Update_date { get; set; }
    }
}
