public class AccountMonitorModel
{
    public string AccountId { get; set; } // Unique identifier for the account
    public DateTime Date { get; set; }    // Time of the update
    public int MessagesSent { get; set; } // Total messages sent by the account
}
