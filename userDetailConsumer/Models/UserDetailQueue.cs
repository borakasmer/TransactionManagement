using System.Collections.Generic;
using userDetailConsumer.DB;

public class UserDetailQueue : UserDetails
{
    public List<TransactionHistory> TransactionList { get; set; }
}