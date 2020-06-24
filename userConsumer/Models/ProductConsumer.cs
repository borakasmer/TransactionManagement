using System.Collections.Generic;
using userConsumer.DB;

public class ProductConsumer : Products
{
    public int UserId { get; set; }
    public List<TransactionHistory> TransactionList { get; set; }
}