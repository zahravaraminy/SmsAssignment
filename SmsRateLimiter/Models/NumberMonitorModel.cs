public class NumberMonitorModel
{
    private string _phoneNumber;
    private DateTime _date;
    private int _messagesSent;

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
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
