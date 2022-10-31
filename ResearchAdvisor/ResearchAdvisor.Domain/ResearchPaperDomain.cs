using Amazon.DynamoDBv2.DataModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ResearchAdvisor.DomainApi.Port;
using ResearchAdvisor.Persistence.Adapter.Context;
using ResearchAdvisor.DomainApi.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using System.Xml.Serialization;
using System.Net;
using System.IO;

namespace ResearchAdvisor.Domain
{
    public class ResearchPaperDomain<T> : IRequestResearchPaper<T> where T : class
    {
        public ResearchPaperDomain()
        {
        }

        public Arxiv Get(string id)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Arxiv));
            Arxiv arxiv;
            string url = $"http://export.arxiv.org/api/query?id_list={id}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                arxiv = (Arxiv)serializer.Deserialize(reader);
                return arxiv;
            }
        }
    }
}
