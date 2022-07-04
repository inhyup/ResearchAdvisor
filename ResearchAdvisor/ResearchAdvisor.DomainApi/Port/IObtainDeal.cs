using ResearchAdvisor.DomainApi.Model;
using System.Collections.Generic;

namespace ResearchAdvisor.DomainApi.Port
{
    public interface IObtainDeal<T>
    {
        List<Deal> GetDeals();
        Deal GetDeal(T id);
    }
}
