public class Salary
{
    public decimal SalaryAmount { get; set; }
    public decimal? MaximumSalaryAmount { get;set; }
    public bool IsSalaryRange { get; set; }
    public required string CurrencyType { get; set; }

    public Salary() 
    {
        IsSalaryRange = false;
        MaximumSalaryAmount = null;
    }
}