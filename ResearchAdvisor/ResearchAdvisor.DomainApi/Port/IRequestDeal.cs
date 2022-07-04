using System.Collections.Generic;

namespace ResearchAdvisor.DomainApi.Port
{
    public interface IRequestDeal<T>
    {
        List<T> GetDeals();
        T GetDeal(int id);
    }
}
