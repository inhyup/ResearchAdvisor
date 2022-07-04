using System.Collections.Generic;

namespace ResearchAdvisor.DomainApi.Port
{
    public interface IRequestResearch<T>
    {
        List<T> GetResearches();
        T GetResearch(int id);
        void PostResearch(T t);
    }
}
