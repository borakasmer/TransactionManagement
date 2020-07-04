public class TransactionHistory
{
    public int ID { get; set; }
    public string TableName { get; set; }
    public TransactionState State { get; set; }
    public TransactionStep Step { get; set; }
    public TransactionType Type { get; set; }
}

public enum TransactionStep
{
    User = 0,
    Product = 1,
    UserDetail = 2
}
public enum TransactionState
{
    Pending = 0,
    Completed = 1,
    WaitingForAction = 2,
    WaitingForRollback = 3,
    RollbackDone = 4,
    ActionDone = 5
}
public enum TransactionType
{
    SqlDB = 1,
    OracleDB = 2,
    Akamai = 3,
    Redis = 4
}
