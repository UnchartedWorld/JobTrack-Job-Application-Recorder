using System.Collections.Generic;

public class JobApplication
{
    public int ID { get; set; }
    public JobInfo JobInformation { get; set; }
    public List<Interview> Interviews { get; set; }
    public JobNotes Notes { get; set; }

}