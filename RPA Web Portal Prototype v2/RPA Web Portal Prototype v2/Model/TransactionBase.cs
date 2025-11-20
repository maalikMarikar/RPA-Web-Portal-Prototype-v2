namespace RPA_Web_Portal_Prototype_v2.Model;

public abstract class TransactionBase
{
    public int Id { get; set; }
    public required string SubmittedUser { get; set; }
    public required DateTime DateTimeSubmitted { get; set; }
    
    public required string BranchName { get; set; }
    public required int BranchId { get; set; }
    
}