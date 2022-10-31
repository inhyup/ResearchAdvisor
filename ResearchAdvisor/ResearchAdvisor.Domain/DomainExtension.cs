using Microsoft.Extensions.DependencyInjection;
using ResearchAdvisor.DomainApi.Port;

namespace ResearchAdvisor.Domain
{
    public static class DomainExtension
    {
        public static void AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IRequestResearchPaper<>), typeof(ResearchPaperDomain<>));
            serviceCollection.AddTransient(typeof(IRequestUser<>), typeof(UserDomain<>));
        }
    }
}
