using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Newtonsoft.Json;
using ResearchAdvisor.DomainApi.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;


namespace ResearchAdvisor.Persistence.Adapter.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public IDataView LoadDB(MLContext mlContext)
        {
            JsonSerializer serializer = new JsonSerializer();
            List<ResearchPaper> o = new List<ResearchPaper>();
            using (FileStream s = File.Open("../ResearchAdvisor.Persistence.Adapter/Context/arxiv-metadata.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (!sr.EndOfStream)
                {
                    o = serializer.Deserialize<List<ResearchPaper>>(reader);
                }
                IDataView data = mlContext.Data.LoadFromEnumerable<ResearchPaper>(o);

                return data;
            }
        }

        public List<ResearchPaper> Load()
        {
            JsonSerializer serializer = new JsonSerializer();
            List<ResearchPaper> o = new List<ResearchPaper>();
            using (FileStream s = File.Open("../ResearchAdvisor.Persistence.Adapter/Context/arxiv-metadata.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (!sr.EndOfStream)
                {
                    o = serializer.Deserialize<List<ResearchPaper>>(reader);
                }
                return o;
            }
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
