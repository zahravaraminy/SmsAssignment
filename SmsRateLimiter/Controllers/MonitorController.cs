using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SmsRateLimiter.Controllers
{
    [Route("monitor")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        /*
        // Mock data for testing
        private static readonly List<AccountMonitorModel> AccountData = new List<AccountMonitorModel>
        {
            new AccountMonitorModel { Date = DateTime.Now.AddMinutes(-10), MessagesSent = 50 },
            new AccountMonitorModel { Date = DateTime.Now.AddMinutes(-5), MessagesSent = 70 },
            new AccountMonitorModel { Date = DateTime.Now, MessagesSent = 80 }
        };

        private static readonly List<NumberMonitorModel> NumberData = new List<NumberMonitorModel>
        {
            new NumberMonitorModel { PhoneNumber = "1234567890", Date = DateTime.Now.AddMinutes(-10), MessagesSent = 10 },
            new NumberMonitorModel { PhoneNumber = "1234567890", Date = DateTime.Now.AddMinutes(-5), MessagesSent = 20 },
            new NumberMonitorModel { PhoneNumber = "1234567890", Date = DateTime.Now, MessagesSent = 30 },
            new NumberMonitorModel { PhoneNumber = "0987654321", Date = DateTime.Now.AddMinutes(-10), MessagesSent = 15 },
            new NumberMonitorModel { PhoneNumber = "0987654321", Date = DateTime.Now.AddMinutes(-5), MessagesSent = 25 },
            new NumberMonitorModel { PhoneNumber = "0987654321", Date = DateTime.Now, MessagesSent = 35 }
        };
        */
        private readonly SlidingWindow _accountLimit;
        private readonly ConcurrentDictionary<string, SlidingWindow> _perNumberLimits;
        private readonly ConcurrentDictionary<string, AccountMonitorModel> _realTimeAccountData;
        private readonly ConcurrentDictionary<string, List<NumberMonitorModel>> _realTimeNumberData;

        public MonitorController(
            SlidingWindow accountLimit,
            ConcurrentDictionary<string, SlidingWindow> perNumberLimits,
            ConcurrentDictionary<string, AccountMonitorModel> realTimeAccountData,
            ConcurrentDictionary<string, List<NumberMonitorModel>> realTimeNumberData)
        {
            _accountLimit = accountLimit;
            _perNumberLimits = perNumberLimits;
            _realTimeAccountData = realTimeAccountData;
            _realTimeNumberData = realTimeNumberData;
        }

        // GET /monitor/account
        [HttpGet("account")]
        public IActionResult GetAccountMonitor([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // test for Mock data
            //var filteredData = AccountData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            
            if (startDate == default || endDate == default)
                return BadRequest("StartDate and EndDate are required.");

            var filteredData = _realTimeAccountData.Values
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .Select(x => new { x.AccountId, x.Date, x.MessagesSent })
                .ToList();

            return Ok(filteredData);
        }

        // GET /monitor/number
        [HttpGet("number")]
        public IActionResult GetNumberMonitor([FromQuery] string phoneNumber, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // test for Mock data
            //var filteredData = NumberData.Where(x => x.PhoneNumber == phoneNumber && x.Date >= startDate && x.Date <= endDate).ToList();

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("PhoneNumber is required.");
            if (startDate == default || endDate == default)
                return BadRequest("StartDate and EndDate are required.");

            if (!_realTimeNumberData.TryGetValue(phoneNumber, out var numberData))
                return NotFound("No data found for this phone number.");

            var filteredData = numberData
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .ToList();

            return Ok(filteredData);
        }
    }

}
