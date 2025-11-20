namespace RPA_Web_Portal_Prototype_v2.Model;

public class RegularSettlement : TransactionBase
{
    
    public required string Cif { get; set; }
    public required string DealNo { get; set; }
    public required string ProductType { get; set; }
    public required string FinalAmount { get; set; }
    
    public required string Rcf1 { get; set; }
    public required string Rcf2 { get; set; }
    public required string Rcf3 { get; set; }
    public required string Rcf4 { get; set; }
    public required string Rcf5 { get; set; }
}