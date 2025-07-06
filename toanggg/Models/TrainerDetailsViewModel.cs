namespace toanggg.Models
{
    public class TrainerDetailsViewModel
    {
        public string TrainerFullName { get; set; }
        public string TrainerAvatar { get; set; }
        public string TrainerEmail { get; set; }
        public string TrainerPhone { get; set; }
        public string TrainerAddress { get; set; }
        public List<ClassViewModel> Classes { get; set; }
    }
}
