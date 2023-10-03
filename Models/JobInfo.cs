using System;

public class JobInfo
{
    public string CompanyName { get; set; }
    public string JobTitle { get; set; }
    public Salary InputSalary { get; set; }
    public string JobLink { get; set; }
    public string JobLocation { get; set; }
    public string JobFlexibility { get; set; }
    public DateTimeOffset DateOfApplying { get; set; }
    public JobOfferStatus OfferStatus { get; set; }

    public enum JobOfferStatus
    {
        Yes,
        No,
        Unknown
    }
}