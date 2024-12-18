using Xunit;
using SmsRateLimiter;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;



public class SmsControllerTests
{

    [Fact]
    public void Test_PerNumberLimit_NotExceeded()
    {
        // Arrange
    var perNumberLimits = new ConcurrentDictionary<string, SlidingWindow>();
    var accountLimit = new SlidingWindow(10);
    var controller = new SmsController(perNumberLimits, accountLimit);

    var phoneNumber = "1234567890";
    var request = new SmsRequest { PhoneNumber = phoneNumber };

    // Act
    var result = controller.CanSend(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal("Message can be sent.", okResult.Value);
     }

    [Fact]
    public void Test_PerNumberLimit_Exceeded()
    {
        var controller = new SmsController();

        var request = new SmsRequest { PhoneNumber = "1234567890" };

        for (int i = 0; i < 5; i++) controller.CanSend(request);

        var exceededResult = controller.CanSend(request);
        Assert.IsType<BadRequestObjectResult>(exceededResult);
    }

    [Fact]
    public void Test_AccountLimit_Exceeded()
    {
        
        var controller = new SmsController();

        for (int i = 0; i < 10; i++)
        {
            var request = new SmsRequest { PhoneNumber = $"123456789{i}" };
            controller.CanSend(request);
        }

        var exceededResult = controller.CanSend(new SmsRequest { PhoneNumber = "extra" });
        Assert.IsType<BadRequestObjectResult>(exceededResult);
    }
}
