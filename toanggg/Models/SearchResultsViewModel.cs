namespace toanggg.Models
{
    public class SearchResultsViewModel
    {
        public List<SearchResult> Gyms { get; set; } = new List<SearchResult>();
        public List<SearchResult> Trainers { get; set; } = new List<SearchResult>();
        public List<SearchResult> GymTypes { get; set; } = new List<SearchResult>();
        public List<SearchResult> MembershipTypes { get; set; } = new List<SearchResult>();
    }
}
