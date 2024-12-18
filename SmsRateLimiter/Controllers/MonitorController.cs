using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SmsRateLimiter.Controllers
{
    [Route("monitor")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
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

        // GET /monitor/account
        [HttpGet("account")]
        public IActionResult GetAccountMonitor([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var filteredData = AccountData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            return Ok(filteredData);
        }

        // GET /monitor/number
        [HttpGet("number")]
        public IActionResult GetNumberMonitor([FromQuery] string phoneNumber, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var filteredData = NumberData.Where(x => x.PhoneNumber == phoneNumber && x.Date >= startDate && x.Date <= endDate).ToList();
            return Ok(filteredData);
        }
    }
}
