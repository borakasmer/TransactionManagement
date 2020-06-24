using System.Collections.Generic;
using productConsumer.DB;

public class UserDetailQueue : UserDetails
{
    public List<TransactionHistory> TransactionList { get; set; }
}