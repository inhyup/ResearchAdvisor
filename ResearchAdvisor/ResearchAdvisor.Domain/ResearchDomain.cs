using Microsoft.EntityFrameworkCore;
using ResearchAdvisor.DomainApi.Port;
using ResearchAdvisor.Persistence.Adapter.Context;
using System.Collections.Generic;
using System.Linq;

namespace ResearchAdvisor.Domain
{
    public class ResearchDomain<T>: IRequestResearch<T> where T : class
    {
        private readonly DbSet<T> table;

        public ResearchDomain(ApplicationDbContext dbContext)
        {
            ApplicationDbContext _dbContext;
            _dbContext = dbContext;
            table = _dbContext.Set<T>();
        }
        public T GetResearch(int id)
        {
            return table.Find(id);
        }

        public List<T> GetResearches()
        {
            return table.ToList();
        }

        public void PostResearch(T t)
        {
            table.Add(t);
        }
    }
}
