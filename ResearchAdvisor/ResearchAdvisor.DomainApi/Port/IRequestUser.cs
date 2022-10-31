using System.Collections.Generic;
using System.Threading.Tasks;
using ResearchAdvisor.DomainApi.Model;

namespace ResearchAdvisor.DomainApi.Port
{
    public interface IRequestUser<T>
    {
        Task<List<T>> GetUserByEmail(string email);
        Task Save(T t);
        Task<T> Load(string email, string password);
        Task Delete(T t);
        Task<Arxiv> SearchAsync(string searchWord, User user);
        Task<Arxiv> LoadArxiv(User user);
        ResearchPapers LoadPapers(User user);
        Task<ResearchPapers> SearchPapers(string searchWord, User user);
    }
}
