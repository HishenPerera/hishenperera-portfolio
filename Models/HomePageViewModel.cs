using System.Collections.Generic;

namespace hishenperera_portfolio.Models
{
    public class HomePageViewModel
    {
        public List<Project> Projects { get; set; } = new();
        public ContactViewModel Contact { get; set; } = new();
    }
}
