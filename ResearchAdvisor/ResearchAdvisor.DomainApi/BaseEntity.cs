using System.ComponentModel.DataAnnotations;

namespace ResearchAdvisor.DomainApi
{
    public class BaseEntity<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
