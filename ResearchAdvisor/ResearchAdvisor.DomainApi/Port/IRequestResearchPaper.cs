using System.Collections.Generic;
using System.Threading.Tasks;
using ResearchAdvisor.DomainApi.Model;


namespace ResearchAdvisor.DomainApi.Port
{
    public interface IRequestResearchPaper<T>
    {
        Arxiv Get(string id);
    }
}
