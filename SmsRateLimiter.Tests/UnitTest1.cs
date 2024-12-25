using Xunit;
using SmsRateLimiter;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;



public class SmsControllerTests
{   
    private readonly ConcurrentDictionary<string, List<NumberMonitorModel>> _realTimeNumberData;
    private readonly ConcurrentDictionary<string, AccountMonitorModel> _realTimeAccountData;
    private readonly ConcurrentDictionary<string, SlidingWindow> _perNumberLimits;
    private readonly SlidingWindow _accountLimit;

    public SmsControllerTests()
    {
        // Initialize the shared data once for all tests
        _realTimeNumberData = new ConcurrentDictionary<string, List<NumberMonitorModel>>();
        _realTimeAccountData = new ConcurrentDictionary<string, AccountMonitorModel>();
        _perNumberLimits = new ConcurrentDictionary<string, SlidingWindow>();
        _accountLimit = new SlidingWindow(10); // Account limit (10 messages per second)
    }


    [Fact]
    public void Test_PerNumberLimit_NotExceeded()
    {
        
       // Arrange
        var controller = new SmsController(
            _realTimeNumberData,
            _realTimeAccountData,
            _perNumberLimits,
            _accountLimit,
            numberLimit: 5
        );


        var phoneNumber = "1234567890";
        var request = new SmsRequest {  AccountId = "12345",PhoneNumber = phoneNumber };

         // Act
        var result = controller.CanSend(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Message can be sent.", okResult.Value);
    }

    [Fact]
    public void Test_PerNumberLimit_Exceeded()
    {
       
        // Arrange
        var controller = new SmsController(
            _realTimeNumberData,
            _realTimeAccountData,
            _perNumberLimits,
            _accountLimit,
            numberLimit: 5
        );

        var request = new SmsRequest { AccountId = "12345",PhoneNumber = "1234567890" };

        for (int i = 0; i < 5; i++) controller.CanSend(request);

        var exceededResult = controller.CanSend(request);
        Assert.IsType<BadRequestObjectResult>(exceededResult);
    }

    [Fact]
    public void Test_AccountLimit_Exceeded()
    {
        
           // Arrange
        var controller = new SmsController(
            _realTimeNumberData,
            _realTimeAccountData,
            _perNumberLimits,
            _accountLimit,
            numberLimit: 5
        );
        for (int i = 0; i < 10; i++)
        {
            var request = new SmsRequest { AccountId = $"12345_{i}",PhoneNumber = $"123456789{i}" };
            controller.CanSend(request);
        }

        var exceededResult = controller.CanSend(new SmsRequest {  AccountId = "12345_extra", PhoneNumber = "extra" });
        Assert.IsType<BadRequestObjectResult>(exceededResult);
    }
}
