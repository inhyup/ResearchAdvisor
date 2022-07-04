namespace ResearchAdvisor.DomainApi.Model
{
    public class Research : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
