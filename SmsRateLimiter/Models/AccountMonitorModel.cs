public class AccountMonitorModel
{
    private string _accountId; // Backing field for AccountId
    private DateTime _date;    // Backing field for Date
    private int _messagesSent; // Backing field for MessagesSent

    // Public properties to access the private fields
    public string AccountId
    {
        get => _accountId;
        set => _accountId = value;
    }

    public DateTime Date
    {
        get => _date;
        set => _date = value;
    }

    public int MessagesSent
    {
        get => _messagesSent;
        set => _messagesSent = value;
    }
}
