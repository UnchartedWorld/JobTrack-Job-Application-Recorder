using System;
using System.Collections.Generic;

public class JobApplication
{
    public Guid ID { get; set; }
    public JobInfo JobInformation { get; set; }
    public List<Interview> Interviews { get; set; }
    public JobNotes Notes { get; set; }

}