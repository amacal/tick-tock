using System.Collections.Generic;

namespace TickTock.Dashboard.Models.Jobs
{
    public class IndexModel
    {
        public List<JobModel> Jobs { get; set; }

        public class JobModel
        {
            public string Id { get; set; }
            public string Version { get; set; }
            public string Name { get; set; }
        }
    }
}