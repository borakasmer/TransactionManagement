using System.Collections.Generic;
using productConsumer.DB;

public class ProductConsumer : Products
{
    public int UserId { get; set; }
    public List<TransactionHistory> TransactionList { get; set; }
}